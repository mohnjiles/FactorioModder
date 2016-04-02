using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FactorioModder.Models;
using Newtonsoft.Json;

namespace FactorioModder.Utility
{
    public class ModsAPIHandler
    {
        public async Task<List<Mod>> GetModsByPageAsync(int page)
        {
            var list = new List<Mod>();

            using (var client = new WebClient())
            {
                var json = await client.DownloadStringTaskAsync($"http://api.factoriomods.com/mods?page={page}");

                list = JsonConvert.DeserializeObject<List<Mod>>(json);
            }


            return list;
        }
    }
}
