using ElvUiUpdater.Entity;
using ElvUiUpdater.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElvUiUpdater.Api
{
    public class TukuiApi
    {
        public const string PATH_OF_ADDONS = "Interface\\Addons";
        public const string ELVUI = "ElvUI";
        public const string ELVUI_OPTIONS = "ElvUI_Libraries";
        public const string ELVUI_LIBRARIES = "ElvUI_Options";
        public const string ELVUI_TOC_FILENAME = "ElvUI_Mainline.toc";
        public static readonly string[] FOLDERS_TO_EXTRACT = { "ElvUI", "ElvUI_Libraries", "ElvUI_Options" };

        private const string TUKUI_ENDPOINT = "https://www.tukui.org/api.php";
        private const string TUKUI_PTR = "https://github.com/tukui-org/ElvUI/archive/ptr.zip";
        private const string ELVUI_GITHUB_URL = "https://raw.githubusercontent.com/tukui-org/ElvUI";

        private readonly HttpClient _client;

        public TukuiApi()
        {
            _client = new HttpClient();
        }

        public bool ValidateInstallation(VersionType type)
        {
            switch (type)
            {
                case VersionType._retail_:
                    return Directory.Exists(Path.Combine(Properties.Settings.Default.Retail, PATH_OF_ADDONS, ELVUI));
                case VersionType._classic_:
                    return Directory.Exists(Path.Combine(Properties.Settings.Default.Classic, PATH_OF_ADDONS, ELVUI));
                case VersionType._ptr_:
                    return Directory.Exists(Path.Combine(Properties.Settings.Default.PTR, PATH_OF_ADDONS, ELVUI));
            }
            return false;
        }

        private string ExtractVersionFromContent(string fileContent)
        {
            Regex regex = new Regex(@"^\s*## Version:\s*(.*)$", RegexOptions.Multiline);
            Match match = regex.Match(fileContent);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return "ERROR";
        }

        public string GetVersion(VersionType type)
        {
            if (!ValidateInstallation(type))
                return "Not installed";

            return ExtractVersionFromContent(System.IO.File.ReadAllText(Path.Combine(Properties.Settings.Default.WoWPath, type.ToString(), PATH_OF_ADDONS, ELVUI, ELVUI_TOC_FILENAME)));
        }

        public async Task<ElvUI> GetElvUIInfo()
        {
            var response = await _client.GetFromJsonAsync<ElvUI>(TUKUI_ENDPOINT + "?ui=elvui");
            return response!;
        }

        public async Task<string> DownloadElvUI(string addonsPath)
        {
            var elvui = await GetElvUIInfo();
            var path = Path.Combine(addonsPath, Path.GetFileName(new Uri(elvui.Url!).LocalPath));
            var response = await _client.GetByteArrayAsync(elvui.Url);
            await System.IO.File.WriteAllBytesAsync(path, response);
            return path;
        }

        public async Task<string> DownloadElvUIPTR(string addonsPath)
        {
            var path = Path.Combine(addonsPath, Path.GetFileName(new Uri(TUKUI_PTR).LocalPath));
            var response = await _client.GetByteArrayAsync(TUKUI_PTR);
            await System.IO.File.WriteAllBytesAsync(path, response);
            return path;
        }

        public async Task<string> GetOnlineVersion(VersionType type)
        {
            string version = type == VersionType._retail_ || type == VersionType._classic_ ? "main" : "ptr";
            string url = $"{ELVUI_GITHUB_URL}/{version}/{ELVUI}/{ELVUI_TOC_FILENAME}";
            string fileContent = await _client.GetStringAsync(url);
            return ExtractVersionFromContent(fileContent);
        }
    }
}
