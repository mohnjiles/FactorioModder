using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModder.Models
{
    public class ModFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mirror { get; set; }
        public string Url { get; set; }
    }
}
