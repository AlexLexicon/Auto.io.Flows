namespace Auto.io.Flows.Application.Services;
public interface IFileService
{
    void WriteAllText(string filePath, string text);
    string ReadAllText(string filePath);
}
