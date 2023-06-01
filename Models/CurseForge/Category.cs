using System;

namespace ElvUiUpdater.Models.CurseForge
{
    public class Category
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public Uri Url { get; set; }
        public Uri IconUrl { get; set; }
        public DateTime DateModified { get; set; }
        public int ParentCategoryId { get; set; }
    }
}
