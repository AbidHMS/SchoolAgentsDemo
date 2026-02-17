namespace SchoolAgentsDemo.Tools;

public interface IHrTool
{
    Task<object> GetDepartmentSummaryAsync();
    Task<object> GetEmployeesByDepartmentAsync(string departmentName);
    Task<object> GetHighEarnersAsync(decimal minSalary);
}
