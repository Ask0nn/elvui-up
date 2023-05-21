using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElvUiUpdater.Models.CurseForge
{
    public class Addon
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public int MyProperty { get; set; }
    }
}
