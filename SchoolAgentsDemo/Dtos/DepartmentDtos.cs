namespace SchoolAgentsDemo.Dtos;

public record DepartmentCreateDto(string Name);
public record DepartmentUpdateDto(string Name);

public record DepartmentViewDto(int Id, string Name, int EmployeesCount);
