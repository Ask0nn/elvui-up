using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElvUiUpdater.Models.CurseForge
{
    public class Logo
    {
        public int Id { get; set; }
        public int ModId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ThumbnailUrl { get; set; }
        public Uri Url { get; set; }
    }
}
