using API_JWT.DAL;
using API_JWT.DTOs;
using API_JWT.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Departments()
        {
            return Ok(await _context.Departments.ToListAsync());
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] DepartmentDTO depart)
        {
            await _context.Departments.AddAsync(new()
            {
                Name = depart.Name,
            });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] DepartmentDTO depart)
        {
            Department? department = await _context.Departments.FindAsync(id);
            if (department is null) return NotFound();
            department.Name = depart.Name is null ? department.Name : depart.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            Department? department = await _context.Departments.FindAsync(id);
            if (department is null) return NotFound();
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
