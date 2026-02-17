using System.Text.Json;

namespace SchoolAgentsDemo.Agents;

public static class AgentValidation
{
    public static void EnsureSafeTool(string toolName)
    {
        var allowed = new[] { "GetDepartmentSummary", "GetEmployeesByDepartment", "GetHighEarners" };
        if (!allowed.Contains(toolName))
            throw new InvalidOperationException($"Tool not allowed: {toolName}");
    }

    public static decimal EnsureSalary(object? value)
    {
        if (value is null) throw new InvalidOperationException("minSalary is required");

        // handles JsonElement
        if (value is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.Number && je.TryGetDecimal(out var d))
                return d;
        }

        if (decimal.TryParse(value.ToString(), out var parsed)) return parsed;
        throw new InvalidOperationException("Invalid minSalary");
    }

    public static string EnsureNonEmptyString(object? value, string fieldName)
    {
        var s = value?.ToString()?.Trim();
        if (string.IsNullOrWhiteSpace(s))
            throw new InvalidOperationException($"{fieldName} is required");
        return s!;
    }
}
