using Microsoft.EntityFrameworkCore;
using SchoolAgentsDemo.Data;

namespace SchoolAgentsDemo.Tools;

public class HrTool : IHrTool
{
    private readonly AppDbContext _db;
    public HrTool(AppDbContext db) => _db = db;

    public async Task<object> GetDepartmentSummaryAsync()
    {
        var data = await _db.Departments
            .Select(d => new
            {
                d.Id,
                d.Name,
                EmployeesCount = d.Employees.Count,
                AvgSalary = d.Employees.Any() ? d.Employees.Average(e => e.Salary) : 0
            })
            .ToListAsync();

        return data;
    }

    public async Task<object> GetEmployeesByDepartmentAsync(string departmentName)
    {
        var dept = departmentName.Trim().ToLowerInvariant();

        var data = await _db.Employees
            .Include(e => e.Department)
            .Where(e => e.Department.Name.ToLower() == dept)
            .Select(e => new { e.Id, e.Name, e.Role, e.Salary, Department = e.Department.Name })
            .ToListAsync();

        return data;
    }

    public async Task<object> GetHighEarnersAsync(decimal minSalary)
    {
        var data = await _db.Employees
            .Include(e => e.Department)
            .Where(e => e.Salary >= minSalary)
            .Select(e => new { e.Id, e.Name, e.Role, e.Salary, Department = e.Department.Name })
            .OrderByDescending(e => e.Salary)
            .ToListAsync();

        return data;
    }
}
