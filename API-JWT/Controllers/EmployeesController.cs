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
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Employees()
        {
            return Ok(await _context.Employees.ToListAsync());
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] EmployeeDTO emp)
        {
            await _context.Employees.AddAsync(new()
            {
                Name = emp.Name,
                Surname = emp.Surname,
                Age = emp.Age,
                DepartmentId = emp.DepartmentId
            });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] EmployeeDTO emp)
        {
            Employee? employee = await _context.Employees.FindAsync(id);
            if (employee is null) return NotFound();
            employee.Name = emp.Name is null ? employee.Name : emp.Name;
            employee.Surname = emp.Surname;
            employee.Age = emp.Age;
            employee.DepartmentId = emp.DepartmentId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            Employee? emp = await _context.Employees.FindAsync(id);
            if(emp is null) return NotFound();
            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
