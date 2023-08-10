using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ElvUiUpdater.Entity
{
    public class ElvUI
    {
        public int? Id { get; set; }
        public string? Slug { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Url { get; set; }
        public string? Version { get; set; }
        [JsonPropertyName("changelog_url")]
        public string? ChangelogUrl { get; set; }
        [JsonPropertyName("ticket_url")]
        public string? TicketUrl { get; set; }
        [JsonPropertyName("git_url")]
        public string? GitUrl { get; set; }
        public dynamic? Patch { get; set; }
        [JsonPropertyName("last_update")]
        public string? LastUpdate { get; set; }
        [JsonPropertyName("web_url")]
        public string? WebUrl { get; set; }
        [JsonPropertyName("last_download")]
        public string? LastDownload { get; set; }
        [JsonPropertyName("donate_url")]
        public string? DonateUrl { get; set; }
        [JsonPropertyName("small_desc")]
        public string? SmallDesc { get; set; }
        [JsonPropertyName("screenshot_url")]
        public string? ScreenshotUrl { get; set; }
        public dynamic? Directories { get; set; }
    }
}
