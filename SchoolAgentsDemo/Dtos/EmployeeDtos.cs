namespace SchoolAgentsDemo.Dtos;

public record EmployeeCreateDto(string Name, string Role, decimal Salary, int DepartmentId);
public record EmployeeUpdateDto(string Name, string Role, decimal Salary, int DepartmentId);

public record EmployeeViewDto(int Id, string Name, string Role, decimal Salary, int DepartmentId, string DepartmentName);
