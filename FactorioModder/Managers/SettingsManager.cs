using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioModder.Models;
using FactorioModder.Utility;
using Newtonsoft.Json;

namespace FactorioModder.Managers
{
    public class SettingsManager : ISettingsManager
    {
        public SettingsManager()
        {
            if (!File.Exists("settings.json"))
            {
                SetupSettings();
            }
            else
            {
                Settings = LoadSettings();
            }
        }

        private async void SetupSettings()
        {
            File.Create("settings.json").Close();
            Settings = new Settings {FactorioInstallPath = await Utilities.GetFactorioInstallPath()};
            SaveSettings(Settings);
        }

        public Settings Settings { get; set; }

        public void SaveSettings(Settings settings)
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }

        public Settings LoadSettings()
        {
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
        }
    }
}
