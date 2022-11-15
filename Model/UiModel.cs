using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElvUiUpdater.Model
{
    [AddINotifyPropertyChangedInterface]
    internal class UiModel
    {
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Url { get; set; }
        public string? Version { get; set; }
        public string? Changelog { get; set; }
        public string? Ticket { get; set; }
        public string? Git { get; set; }
        public int? Id { get; set; }
        public string? Patch { get; set; }
        public string? LastUpdate { get; set; }
        public string? WebUrl { get; set; }
        public string? LastDownload { get; set; }
        public string? DonateUrl { get; set; }
        public string? SmallDesc { get; set; }
        public string? ScreenshotUrl { get; set; }
        public long? Downloads { get; set; }
        public string? Category { get; set; }
    }
}
