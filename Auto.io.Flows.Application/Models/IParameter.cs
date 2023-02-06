namespace Auto.io.Flows.Application.Models;
public interface IParameter
{
    string Identifier { get; }
    public UserInterfaces UserInterface { get; }
    public object? Argument { get; }
    public object? InitalValue { get; }

    string DisplayName { get; init; }

    bool Validate(object? value);
}
