namespace Auto.io.Flows.Application.Services;
public interface IParameterService
{
    public const string KEY_ITERATION = "{i}";
    void SetVariable(string key, string value);
    string ReplaceVariables(string value);
}
public class ParameterService : IParameterService
{
    private readonly Dictionary<string, string> _variables;

    public ParameterService()
    {
        _variables = new Dictionary<string, string>();
    }

    public void SetVariable(string key, string value)
    {
        if (_variables.ContainsKey(key))
        {
            _variables[key] = value;
        }
        else
        {
            _variables.Add(key, value);
        }
    }

    public string ReplaceVariables(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        int overflow = 0;
        bool complete = false;
        while (!complete && overflow < 1000)
        {
            foreach (var variable in _variables)
            {
                if (value.Contains(variable.Key))
                {
                    value = value.Replace(variable.Key, variable.Value);
                }
            }
            overflow++;
        }

        return value;
    }
}
