using System.Collections.Generic;

namespace ElvUiUpdater.Models.CurseForge
{
    public class Addon
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public int Status { get; set; }
        public int PrimaryCategoryId { get; set; }
        public Category[] Categories { get; set; }
        public Author[] Authors { get; set; }
        public Logo Logo { get; set; }
        public int MainFileId { get; set; }

    }
}
