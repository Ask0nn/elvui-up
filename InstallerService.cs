using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElvUiUpdater;

internal class InstallerService
{
    public static bool CheckUiVersion(string path, string version)
    {
        path = Path.Combine(path, "ElvUI");
        if (Directory.Exists(path))
            if (File.Exists(Path.Combine(path, "ElvUI_Mainline.toc")))
                return version.Equals(GetVersion(Path.Combine(path, "ElvUI_Mainline.toc")));
        return false;
    }

    private static string GetVersion(string path)
    {
        foreach (var line in File.ReadAllLines(path))
            if (line.Contains("## Version:"))
                return line.Replace("## Version:", "").Trim();
        return "";
    }
}