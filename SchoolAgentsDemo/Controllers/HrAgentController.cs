using Microsoft.AspNetCore.Mvc;
using SchoolAgentsDemo.Agents;
using SchoolAgentsDemo.Dtos;

namespace SchoolAgentsDemo.Controllers;

[ApiController]
[Route("api/agent/hr")]
public class HrAgentController : ControllerBase
{
    private readonly IHrAgent _agent;
    public HrAgentController(IHrAgent agent) => _agent = agent;

    [HttpPost("ask")]
    public async Task<ActionResult<HrAskResponse>> Ask([FromBody] HrAskRequest req)
    {
        var result = await _agent.AskAsync(req);
        return Ok(result);
    }
}
