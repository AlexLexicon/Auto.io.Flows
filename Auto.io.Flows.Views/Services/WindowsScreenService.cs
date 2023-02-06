using Auto.io.Flows.Application.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Auto.io.Flows.Views.Services;
public class WindowsScreenService : IScreenService
{
    public void Screenshot(int x1, int y1, int x2, int y2, string filePath)
    {
        Screen? primaryScreen = Screen.PrimaryScreen;

        if (primaryScreen is null)
        {
            throw new Exception("The primary screen was null when trying to take a screenshot");
        }

        var size = new Size(x2 - x1, y2 - y1);

        using var image = new Bitmap(size.Width, size.Height);

        Graphics graphics = Graphics.FromImage(image);

        graphics.CopyFromScreen(x1, y1, 0, 0, size);

        var fileInfo = new FileInfo(filePath);

        image.Save(fileInfo.FullName, ImageFormat.Png);
    }
}
