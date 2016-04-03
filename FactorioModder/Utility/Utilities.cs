using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using FactorioModder.Models;
using Ionic.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;

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

        public static string GetFactorioModPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Factorio", "mods");
        }

        private static InstalledMods _installedMods;
        public static InstalledMods GetInstalledMods()
        {
            if (_installedMods != null) return _installedMods;

            var json = File.ReadAllText(Path.Combine(GetFactorioModPath(), "mod-list.json"));
            _installedMods = JsonConvert.DeserializeObject<InstalledMods>(json);
            return _installedMods;
        }

        public static List<Mod> GetInstalledModList()
        {
            var modList = new List<Mod>();

            foreach (var file in
                    Directory.EnumerateFiles(Utilities.GetFactorioModPath(), "*.zip", SearchOption.TopDirectoryOnly))
            {
                using (ZipFile zip = ZipFile.Read(file))
                {
                    var folderName = Path.GetFileName(file).Replace(".zip", "/");

                    ZipEntry e = zip[$"{folderName}info.json"];

                    if (e == null) continue;

                    using (var ms = new MemoryStream())
                    {
                        e.Extract(ms);

                        ms.Position = 0;
                        var sr = new StreamReader(ms);
                        var json = sr.ReadToEnd();

                        var theMod = JsonConvert.DeserializeObject<Mod>(json);
                        theMod.Enabled = IsModEnabled(theMod.Name);
                        modList.Add(theMod);
                    }
                }
            }

            return modList;
        }

        private static bool IsModEnabled(string name)
        {
            return GetInstalledMods().Mods.FirstOrDefault(x => x.Name == name)?.Enabled ?? false;
        }

        public static async Task<string> GetFactorioInstallPath()
        {
            return await Task.Run(() =>
            {
                foreach (var folder in LibraryFolders())
                {
                    var files = Directory.EnumerateFiles(folder, "Factorio.exe", SearchOption.AllDirectories);
                    if (files.ToList().Count > 0)
                    {
                        return files.FirstOrDefault();
                    }
                }
                return "";
            });
        }

        private static string SteamFolder()
        {
            RegistryKey steamKey = Registry.LocalMachine.OpenSubKey("Software\\Valve\\Steam") ?? Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam");
            return steamKey.GetValue("InstallPath").ToString();
        }

        private static List<string> LibraryFolders()
        {
            List<string> folders = new List<string>();

            string steamFolder = SteamFolder();
            folders.Add(steamFolder);

            string configFile = steamFolder + "\\config\\config.vdf";

            Regex regex = new Regex("BaseInstallFolder[^\"]*\"\\s*\"([^\"]*)\"");
            using (StreamReader reader = new StreamReader(configFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        folders.Add(Regex.Unescape(match.Groups[1].Value));
                    }
                }
            }

            return folders;
        }
    }
}
