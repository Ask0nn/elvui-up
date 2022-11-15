using ElvUiUpdater.Properties;
using Microsoft.Win32;
using System.Security.Principal;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using ElvUiUpdater.ViewModel;

namespace ElvUiUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (string.IsNullOrEmpty(Settings.Default.WowPath))
                Settings.Default.WowPath = IsInstalled("World of Warcraft");
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        public static string? IsInstalled(string pName)
        {
            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            Console.WriteLine(WindowsIdentity.GetCurrent().User == sid);
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key?.GetSubKeyNames()!)
            {
                var subkey = key.OpenSubKey(keyName);
                var displayName = subkey?.GetValue("DisplayName") as string;
                if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
                    return subkey?.GetValue("InstallLocation") as string;
            }

            return "Root game directory";
        }
    }
}
