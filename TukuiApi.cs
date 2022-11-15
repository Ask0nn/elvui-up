using ElvUiUpdater.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ElvUiUpdater
{
    internal class TukuiApi
    {
        private static TukuiApi? _instance;
        public static TukuiApi? GetInstance => _instance ??= new TukuiApi();

        private const string TukuiEndpoint = "https://www.tukui.org/api.php";
        private static readonly HttpClient Client = new HttpClient();

        private TukuiApi()
        {
        }

        public UiModel GetElvUi()
        {
            UiModel? result = new UiModel();
            var response = Client.GetAsync(TukuiEndpoint + "?ui=elvui")
                .ContinueWith((task) =>
                {
                    var response = task.Result;
                    var jsonString = response.Content.ReadAsStringAsync();
                    jsonString.Wait();
                    result = JsonConvert.DeserializeObject<UiModel>(jsonString.Result);
                });
            response.Wait();
            return result;
        }

        public string DownloadUI(string url, string path)
        {
            path = Path.Combine(path, "ELVUI.zip");
            if (!File.Exists(path))
                File.Create(path).Close();
            var response = Client.GetByteArrayAsync(url)
                .ContinueWith((task) =>
                {
                    var response = task.Result;
                    File.WriteAllBytes(path, response);
                });
            response.Wait();
            return path;
        }
    }
}
