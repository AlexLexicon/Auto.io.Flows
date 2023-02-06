using Auto.io.Flows.Application.Models;

namespace Auto.io.Flows.Application.Services;
public interface IStepService
{
    IReadOnlyList<string> GetAllStepIdentifiers();
    IStep GetStepByIdentifier(string identifier);
}
public class StepService : IStepService
{
    private readonly IReadOnlyList<IStep> _steps;

    public StepService(IEnumerable<IStep> steps)
    {
        _steps = steps.ToList();
    }

    public IReadOnlyList<string> GetAllStepIdentifiers()
    {
        return _steps
            .Select(s => s.Identifier)
            .ToList();
    }

    public IStep GetStepByIdentifier(string identifier)
    {
        IStep? step = _steps.FirstOrDefault(s => s.Identifier == identifier);

        if (step is null)
        {
            throw new Exception($"A step with the identifier '{identifier}' does not exist.");
        }

        return step;
    }
}
