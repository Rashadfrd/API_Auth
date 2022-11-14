using API_JWT.DAL;
using API_JWT.DTOs;
using API_JWT.Entities;
using API_JWT.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration, RoleManager<AppRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDTO userDto)
        {
            AppUser user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user is not null) return StatusCode(StatusCodes.Status403Forbidden, new { Message = "This user has already been registered" });
            AppUser newUser = new()
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                UserName = userDto.Email.Substring(0, userDto.Email.IndexOf("@"))
            };
            IdentityResult result = await _userManager.CreateAsync(newUser,userDto.Password);
            if (!result.Succeeded) return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Message = "Register error",
                result.Errors
            });

            IdentityResult resultAddRole = await _userManager.AddToRoleAsync(newUser, Roles.Member.ToString());
            if (!resultAddRole.Succeeded) return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Message = "Register error",
                resultAddRole.Errors
            });

            return Ok(new
            {
                Message = "Register success"
            });
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleAsync()
        {
            foreach (object item in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new AppRole
                    {
                        Name = item.ToString(),
                    });
                }
            }
            return NoContent();
        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDTO userLoginDto)
        {

            AppUser user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if (user is null) return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Email or password is wrong" });
            if (!await _userManager.CheckPasswordAsync(user, userLoginDto.Password)) return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Email or password is wrong" });

            var roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id),
                    new Claim("FullName",user.FullName)
                };
            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["JWTToken:securityKey"]));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            DateTime expiringDate = DateTime.Now.AddMinutes(45);

            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: _configuration["JWTToken:issuer"],
                audience: _configuration["JWTToken:audience"],
                claims: claims,
                notBefore: null,
                expires: expiringDate,
                signingCredentials: signingCredentials);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return Ok(new
            {
                token,
                Message = "Login success"
            });
        }
    }
}
