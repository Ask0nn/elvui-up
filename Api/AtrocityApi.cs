using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElvUiUpdater.Api
{
    internal class AtrocityApi
    {
        public const string PATH_OF_ADDONS = "Interface\\Addons";
        public const string AUI = "AtrocityUI";
        public const string INTERFACE = "Interface";
        public const string FONTS = "Fonts";
        public const string AUI_TOC_FILENAME = "AtrocityUI.toc";
        public static readonly string[] FOLDERS_TO_EXTRACT = { "Interface", "Fonts"};
        public static readonly string[] FOLDERS_TO_CHECK = { "Interface", "Fonts", "WTF" };

        public static readonly string AUI_LOAD_URL = "https://github.com/Ask0nn/aui-load/archive/main.zip";
        public static readonly string AUI_BASE_URL = "https://raw.githubusercontent.com/Ask0nn/aui-load/main";

        private readonly HttpClient _client;

        public AtrocityApi()
        {
            _client = new HttpClient();
        }

        public bool ValidateInstallation(VersionType type)
        {
            switch (type)
            {
                case VersionType._retail_:
                    return Directory.Exists(Path.Combine(Properties.Settings.Default.Retail, PATH_OF_ADDONS, AUI));
                case VersionType._classic_:
                    return Directory.Exists(Path.Combine(Properties.Settings.Default.Classic, PATH_OF_ADDONS, AUI));
                case VersionType._ptr_:
                    return Directory.Exists(Path.Combine(Properties.Settings.Default.PTR, PATH_OF_ADDONS, AUI));
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

            return ExtractVersionFromContent(File.ReadAllText(Path.Combine(Properties.Settings.Default.WoWPath, type.ToString(), PATH_OF_ADDONS, AUI, AUI_TOC_FILENAME)));
        }

        public async Task<string> GetOnlineVersion()
        {
            string url = $"{AUI_BASE_URL}/Interface/Addons/{AUI}/{AUI_TOC_FILENAME}";
            string fileContent = await _client.GetStringAsync(url);
            return ExtractVersionFromContent(fileContent);
        }

        public async Task<string> DownloadAUI(string rootVersionPath)
        {
            var path = Path.Combine(rootVersionPath, Path.GetFileName(new Uri(AUI_LOAD_URL).LocalPath));
            var response = await _client.GetByteArrayAsync(AUI_LOAD_URL);
            await File.WriteAllBytesAsync(path, response);
            return path;
        }

        public async Task<string> GetChangelog()
        {
            string url = $"{AUI_BASE_URL}/changelog.txt";
            return await _client.GetStringAsync(url);
        }

        public async Task<string> GetKey()
        {
            string url = $"{AUI_BASE_URL}/key.txt";
            return await _client.GetStringAsync(url);
        }
    }
}
