namespace SchoolAgentsDemo.Dtos;

public record HrAskRequest(string Question);

public record HrAskResponse(
    string RoutedTool,
    object? ToolArgs,
    object? ToolResult,
    string FinalAnswer
);
