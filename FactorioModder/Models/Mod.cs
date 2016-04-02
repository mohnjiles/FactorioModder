using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModder.Models
{
    public class Mod
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public List<string> Categories { get; set; }
        public string Author { get; set; }
        public string Contact { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Homepage { get; set; }
        public List<Release> Releases { get; set; }
    }
}
