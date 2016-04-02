using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FactorioModder.Models
{
    public class Release
    {
        public int Id { get; set; }
        public string Version { get; set; }
        [JsonProperty("released_at")]
        public string ReleasedAt { get; set; }
        [JsonProperty("game_versions")]
        public List<string> GameVersions { get; set; }
        // API does not have this implemented
        //public List<object> dependencies { get; set; }
        public List<ModFile> Files { get; set; }
    }
}
