namespace Auto.io.Flows.Application.Models;
public class FlowStep
{
    public required string Identifer { get; init; }
    public required IEnumerable<FlowParameter?> Parameters { get; init; }
}
