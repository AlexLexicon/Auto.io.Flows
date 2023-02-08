namespace Auto.io.Flows.Application.Models.Steps;
public class PauseStep : IStep
{
    public string Identifier => "PauseV1";
    public string Description => "Pauses the execution of the current run.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>();

    public async Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        await runner.PauseAsync();
    }
}
