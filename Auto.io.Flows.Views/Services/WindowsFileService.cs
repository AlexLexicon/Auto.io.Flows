using Auto.io.Flows.Application.Services;
using System.IO;

namespace Auto.io.Flows.Views.Services;
public class WindowsFileService : IFileService
{
    public void WriteAllText(string filePath, string text)
    {
        File.WriteAllText(filePath, text);
    }

    public string ReadAllText(string filePath)
    {
        return File.ReadAllText(filePath);
    }
}
