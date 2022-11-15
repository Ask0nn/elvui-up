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
        return version.Equals(GetVersion(path));
    }

    public static string GetVersion(string path)
    {
        path = Path.Combine(path, "ElvUI");
        if (Directory.Exists(path))
            if (File.Exists(Path.Combine(path, "ElvUI_Mainline.toc")))
                foreach (var line in File.ReadAllLines(Path.Combine(path, "ElvUI_Mainline.toc")))
                    if (line.Contains("## Version:"))
                        return line.Replace("## Version:", "").Trim();
        return "";
    }
}