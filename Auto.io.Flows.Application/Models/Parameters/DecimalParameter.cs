namespace Auto.io.Flows.Application.Models.Parameters;
public class DecimalParameter : IParameter
{
    private static readonly char[] DIGITS = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.' };

    public string Identifier => "DecimalV1";
    public UserInterfaces UserInterface => UserInterfaces.TextBox;
    public object? Argument => null;
    public object? InitalValue => 0.0;

    public required string DisplayName { get; init; }

    public bool Validate(object? value)
    {
        if (value is double)
        {
            return true;
        }

        if (value is string valueString)
        {
            if (!string.IsNullOrWhiteSpace(valueString))
            {
                if (valueString.All(DIGITS.Contains))
                {
                    return valueString.Count(c => c == '.') <= 1;
                }
            }
        }

        return false;
    }
}
