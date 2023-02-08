namespace Auto.io.Flows.Application.Models;
public interface IStep
{
    string Identifier { get; }
    string Description { get; }

    IReadOnlyList<IParameter> Parameters { get; }

    Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters);
}
