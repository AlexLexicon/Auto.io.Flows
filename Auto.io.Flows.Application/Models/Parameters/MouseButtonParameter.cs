using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Parameters;
public class MouseButtonParameter : IParameter
{
    public string Identifier => "MouseButtonV1";
    public UserInterfaces UserInterface => UserInterfaces.ComboBox;
    public object? Argument => IMouseService.MOUSE_BUTTONS;
    public object? InitalValue => IMouseService.MOUSE_BUTTONS.First();

    public required string DisplayName { get; init; }

    public bool Validate(object? value)
    {
        if (value is string valueString)
        {
            if (!string.IsNullOrWhiteSpace(valueString))
            {
                return IMouseService.MOUSE_BUTTONS.Contains(valueString);
            }
        }

        return false;
    }
}