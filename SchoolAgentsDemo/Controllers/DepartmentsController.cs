using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAgentsDemo.Data;
using SchoolAgentsDemo.Domain;
using SchoolAgentsDemo.Dtos;

namespace SchoolAgentsDemo.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public DepartmentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<List<DepartmentViewDto>> GetAll()
    {
        return await _db.Departments
            .Select(d => new DepartmentViewDto(d.Id, d.Name, d.Employees.Count))
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentViewDto>> Get(int id)
    {
        var d = await _db.Departments.Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == id);
        if (d is null) return NotFound();
        return new DepartmentViewDto(d.Id, d.Name, d.Employees.Count);
    }

    [HttpPost]
    public async Task<ActionResult> Create(DepartmentCreateDto dto)
    {
        var dep = new Department { Name = dto.Name.Trim() };
        _db.Departments.Add(dep);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dep.Id }, new { dep.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, DepartmentUpdateDto dto)
    {
        var dep = await _db.Departments.FindAsync(id);
        if (dep is null) return NotFound();
        dep.Name = dto.Name.Trim();
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var dep = await _db.Departments.FindAsync(id);
        if (dep is null) return NotFound();
        _db.Departments.Remove(dep);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
