using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModder.Utility
{
    public class Utilities
    {
        public static List<string> GetCommandLineArgsList()
        {
            var list = Environment.GetCommandLineArgs().ToList();
            list.RemoveAt(0);
            return list;
        }

        public static string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }
    }
}
