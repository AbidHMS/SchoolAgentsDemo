using Microsoft.EntityFrameworkCore;
using SchoolAgentsDemo.Agents;
using SchoolAgentsDemo.Data;
using SchoolAgentsDemo.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IHrTool, HrTool>();
builder.Services.AddSingleton<PromptStore>();
builder.Services.AddScoped<IHrAgent, HrAgent>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "API is running...");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.EnsureSeededAsync(db);
}

app.Run();
