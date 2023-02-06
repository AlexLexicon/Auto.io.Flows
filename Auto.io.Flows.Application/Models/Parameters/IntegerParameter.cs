namespace Auto.io.Flows.Application.Models.Parameters;
public class IntegerParameter : IParameter
{
    private static readonly char[] DIGITS = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-' };

    public string Identifier => "IntegerV1";
    public UserInterfaces UserInterface => UserInterfaces.TextBox;
    public object? Argument => null;
    public object? InitalValue => 0;

    public required string DisplayName { get; init; }

    public bool Validate(object? value)
    {
        if (value is int)
        {
            return true;
        }

        if (value is string valueString)
        {
            if (!string.IsNullOrWhiteSpace(valueString))
            {
                if (valueString.All(DIGITS.Contains))
                {
                    int negativeCount = valueString.Count(c => c == '-');
                    if (negativeCount <= 0)
                    {
                        return true;
                    }
                    else if (negativeCount == 1)
                    {
                        return valueString[0] == '-';
                    }
                }
            }
        }

        return false;
    }
}
