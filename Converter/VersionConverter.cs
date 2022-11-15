using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ElvUiUpdater.Converter
{
    internal class VersionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not string siteVersion) return "Tukui site version / installed : ";
            if (values[1] is not string path) return "Tukui site version / installed : ";
            if (values[2] is not string releaseType) return "Tukui site version / installed : ";
            if (string.IsNullOrEmpty(siteVersion) || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(releaseType))
                return "Tukui site version / installed : ";
            StringBuilder builder = new StringBuilder();
            builder.Append("Tukui site version / installed : ").Append(siteVersion).Append(" | ");
            builder.Append(InstallerService.GetVersion(Path.Combine(path, releaseType, "Interface", "Addons")));

            return builder.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[3];
        }
    }
}
