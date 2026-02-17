using SchoolAgentsDemo.Dtos;

namespace SchoolAgentsDemo.Agents;

public interface IHrAgent
{
    Task<HrAskResponse> AskAsync(HrAskRequest request);
}
