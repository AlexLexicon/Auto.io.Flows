namespace Auto.io.Flows.Application.Models.Parameters;
public class FilePathParameter : IParameter
{
    public string Identifier => "FilePathV1";
    public UserInterfaces UserInterface => UserInterfaces.FilePathBrowser;
    public required object? Argument { get; init; }
    public required object? InitalValue { get; init; }

    public required string DisplayName { get; init; }

    public bool Validate(object? value)
    {
        string? valueString = value?.ToString();

        if (string.IsNullOrWhiteSpace(valueString))
        {
            return false;
        }
        try
        {
            var fileInfo = new FileInfo(valueString);

            return Directory.Exists(fileInfo.DirectoryName);
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
