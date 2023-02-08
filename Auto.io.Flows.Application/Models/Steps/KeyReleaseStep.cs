using Auto.io.Flows.Application.Models.Parameters;

namespace Auto.io.Flows.Application.Models.Steps;
public class KeyReleaseStep : IStep
{
    public string Identifier => "KeyRelease";
    public string Description => "Releases the provided key on the keyboard.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new KeyParameter
        {
            DisplayName = "Key",
        }
    };

    public Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        throw new NotImplementedException();
    }
}
