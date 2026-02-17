using System.Text.Json;
using OpenAI.Chat;
using SchoolAgentsDemo.Dtos;
using SchoolAgentsDemo.Tools;

namespace SchoolAgentsDemo.Agents;

public class HrAgent : IHrAgent
{
    private readonly IConfiguration _config;
    private readonly PromptStore _prompts;
    private readonly IHrTool _tool;

    public HrAgent(IConfiguration config, PromptStore prompts, IHrTool tool)
    {
        _config = config;
        _prompts = prompts;
        _tool = tool;
    }

    public async Task<HrAskResponse> AskAsync(HrAskRequest request)
    {
        // 1) ROUTE: choose tool (LLM or fallback)
        var route = await RouteToolAsync(request.Question);

        AgentValidation.EnsureSafeTool(route.Tool);

        // 2) EXECUTE TOOL securely
        object toolResult = route.Tool switch
        {
            "GetDepartmentSummary" => await _tool.GetDepartmentSummaryAsync(),

            "GetEmployeesByDepartment" => await _tool.GetEmployeesByDepartmentAsync(
                AgentValidation.EnsureNonEmptyString(route.Args.GetValueOrDefault("departmentName"), "departmentName")
            ),

            "GetHighEarners" => await _tool.GetHighEarnersAsync(
                AgentValidation.EnsureSalary(route.Args.GetValueOrDefault("minSalary"))
            ),

            _ => throw new InvalidOperationException("Unsupported tool.")
        };

        // 3) FINAL ANSWER (LLM or fallback)
        var finalAnswer = await FinalAnswerAsync(request.Question, route.Tool, route.Args, toolResult);

        return new HrAskResponse(route.Tool, route.Args, toolResult, finalAnswer);
    }

    private async Task<(string Tool, Dictionary<string, object?> Args)> RouteToolAsync(string question)
    {
        var apiKey = _config["OpenAI:ApiKey"]?.Trim();
        if (string.IsNullOrEmpty(apiKey))
        {
            // fallback: simple router for learning
            var q = question.ToLowerInvariant();
            if (q.Contains("highest") || q.Contains("average") || q.Contains("avg"))
                return ("GetDepartmentSummary", new());

            if (q.Contains("department") || q.Contains("it") || q.Contains("hr"))
            {
                var dept = q.Contains("it") ? "IT" : "HR";
                return ("GetEmployeesByDepartment", new() { ["departmentName"] = dept });
            }

            if (q.Contains("above") || q.Contains(">") || q.Contains("salary"))
                return ("GetHighEarners", new() { ["minSalary"] = 150000 });

            return ("GetDepartmentSummary", new());
        }

        var routerPrompt = _prompts.Load("HrRouterPrompt.txt");
        var client = new ChatClient("gpt-4.1-mini", apiKey);

        var msg = $"{routerPrompt}\n\nUser Question:\n{question}";
        var res = await client.CompleteChatAsync(msg);
        var text = res.Value.Content[0].Text;

        // Must be strict JSON
        using var doc = JsonDocument.Parse(text);
        var root = doc.RootElement;

        var tool = root.GetProperty("tool").GetString() ?? "";
        var args = new Dictionary<string, object?>();

        if (root.TryGetProperty("args", out var argsEl) && argsEl.ValueKind == JsonValueKind.Object)
        {
            foreach (var p in argsEl.EnumerateObject())
                args[p.Name] = p.Value.ValueKind == JsonValueKind.Number ? p.Value.ToString() : p.Value.GetString();
        }

        return (tool, args);
    }

    private async Task<string> FinalAnswerAsync(string question, string tool, Dictionary<string, object?> args, object toolResult)
    {
        var apiKey = _config["OpenAI:ApiKey"]?.Trim();
        if (string.IsNullOrEmpty(apiKey))
        {
            return $"(Fallback) Tool used: {tool}. Result prepared. Add OpenAI key for natural language final answer.";
        }

        var finalPrompt = _prompts.Load("HrFinalAnswerPrompt.txt");
        var client = new ChatClient("gpt-4.1-mini", apiKey);

        var payload = JsonSerializer.Serialize(new { tool, args, toolResult }, new JsonSerializerOptions { WriteIndented = true });

        var msg = $"{finalPrompt}\n\nUser Question:\n{question}\n\nTool Result JSON:\n{payload}";
        var res = await client.CompleteChatAsync(msg);
        return res.Value.Content[0].Text;
    }
}
