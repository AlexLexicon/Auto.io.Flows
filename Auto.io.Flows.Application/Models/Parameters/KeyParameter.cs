using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Parameters;
public class KeyParameter : IParameter
{
    public string Identifier => "KeyV1";
    public UserInterfaces UserInterface => UserInterfaces.ComboBox;
    public object? Argument => IKeysService.KEYS;
    public object? InitalValue => IKeysService.KEYS.First();

    public required string DisplayName { get; init; }

    public bool Validate(object? value)
    {
        if (value is string valueString)
        {
            if (!string.IsNullOrWhiteSpace(valueString))
            {
                return IKeysService.KEYS.Contains(valueString);
            }
        }

        return false;
    }
}
