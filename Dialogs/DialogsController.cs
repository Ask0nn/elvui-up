using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace ElvUiUpdater.Dialogs
{
    internal class DialogsController
    {
        public static async Task ShowAlertDialog(string message, string dialogHostId)
        {
            try
            {
                CloseMessages();
                var dialog = new Alert(message);
                await DialogHost.Show(dialog, dialogHostId);
            }
            catch
            {
                CloseMessages();
            }
        }

        public static async Task ShowLoadingDialog(string dialogHostId, string message = "Executing...")
        {
            try
            {
                CloseMessages();
                var dialog = new Loading(message);
                await DialogHost.Show(dialog, dialogHostId);
            }
            catch
            {
                CloseMessages();
            }
        }

        public static void CloseMessages() => DialogHost.CloseDialogCommand.Execute(null, null);
    }
}
