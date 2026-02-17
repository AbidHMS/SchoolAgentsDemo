namespace SchoolAgentsDemo.Agents;

public class PromptStore
{
    private readonly IWebHostEnvironment _env;

    public PromptStore(IWebHostEnvironment env) => _env = env;

    public string Load(string fileName)
    {
        var path = Path.Combine(_env.ContentRootPath, "Prompts", fileName);
        return File.ReadAllText(path);
    }
}
