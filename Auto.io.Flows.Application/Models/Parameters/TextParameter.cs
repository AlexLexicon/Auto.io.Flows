namespace Auto.io.Flows.Application.Models.Parameters;
public class TextParameter : IParameter
{
    public string Identifier => "TestV1";
    public UserInterfaces UserInterface => UserInterfaces.TextBox;
    public object? Argument => null;
    public object? InitalValue => null;

    public required string DisplayName { get; init; }

    public bool Validate(object? value)
    {
        if (value is string valueString)
        {
            return !string.IsNullOrEmpty(valueString);
        }

        return false;
    }
}
