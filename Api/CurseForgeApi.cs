using System;
using System.Net.Http;

namespace ElvUiUpdater.Api
{
    public class CurseForgeApi
    {
        private const string ENDPOINT = "https://api.curseforge.com";
        private const int GAME_ID = 1;
        private const string API_KEY = "$2a$10$hgRu9A.bHJ9HFjQb.7G/m.ZSl8DwS/RWwhH0rZ7lQgqEY0N4OlKaC";

        private readonly HttpClient _httpClient;

        public CurseForgeApi()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ENDPOINT);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", API_KEY);
        }
    }
}
