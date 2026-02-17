using Microsoft.EntityFrameworkCore;
using SchoolAgentsDemo.Domain;

namespace SchoolAgentsDemo.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Departments.AnyAsync()) return;

        var it = new Department { Name = "IT" };
        var hr = new Department { Name = "HR" };

        db.Departments.AddRange(it, hr);

        db.Employees.AddRange(
            new Employee { Name = "Ali", Role = "Developer", Salary = 150000, Department = it },
            new Employee { Name = "Sara", Role = "QA", Salary = 120000, Department = it },
            new Employee { Name = "Hassan", Role = "HR Executive", Salary = 100000, Department = hr }
        );

        await db.SaveChangesAsync();
    }
}
