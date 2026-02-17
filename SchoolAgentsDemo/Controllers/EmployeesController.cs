using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAgentsDemo.Data;
using SchoolAgentsDemo.Domain;
using SchoolAgentsDemo.Dtos;

namespace SchoolAgentsDemo.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;
    public EmployeesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<List<EmployeeViewDto>> GetAll()
    {
        return await _db.Employees
            .Include(e => e.Department)
            .Select(e => new EmployeeViewDto(e.Id, e.Name, e.Role, e.Salary, e.DepartmentId, e.Department.Name))
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeViewDto>> Get(int id)
    {
        var e = await _db.Employees.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return NotFound();
        return new EmployeeViewDto(e.Id, e.Name, e.Role, e.Salary, e.DepartmentId, e.Department.Name);
    }

    [HttpPost]
    public async Task<ActionResult> Create(EmployeeCreateDto dto)
    {
        var depExists = await _db.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!depExists) return BadRequest("Invalid DepartmentId");

        var emp = new Employee
        {
            Name = dto.Name.Trim(),
            Role = dto.Role.Trim(),
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId
        };

        _db.Employees.Add(emp);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = emp.Id }, new { emp.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, EmployeeUpdateDto dto)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp is null) return NotFound();

        var depExists = await _db.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!depExists) return BadRequest("Invalid DepartmentId");

        emp.Name = dto.Name.Trim();
        emp.Role = dto.Role.Trim();
        emp.Salary = dto.Salary;
        emp.DepartmentId = dto.DepartmentId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp is null) return NotFound();
        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
