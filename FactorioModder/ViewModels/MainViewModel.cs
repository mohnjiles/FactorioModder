using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using FactorioModder.Annotations;
using FactorioModder.Models;
using FactorioModder.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FactorioModder.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly ModsAPIHandler _handler;

        public MainViewModel()
        {
            _handler = new ModsAPIHandler();
            WindowLoadedCommand = new Command(OnLoaded);
            ScrollBarScrolledCommand = new Command(LoadAnotherPage);
            CheckedCommand = new ParameterCommand(param => OnModEnabled(param.ToString()));
            UncheckedCommand = new ParameterCommand(param => OnModDisabled(param.ToString()));
            InstallModCommand = new ParameterCommand(param => InstallMod((Mod)param));

            Messenger.Default.Register<UriMessage>(this, ReceiveMessage);
        }

        #region Properties

        public Command WindowLoadedCommand { get; private set; }
        public Command ScrollBarScrolledCommand { get; private set; }
        public ParameterCommand CheckedCommand { get; private set; }
        public ParameterCommand UncheckedCommand { get; private set; }
        public ParameterCommand InstallModCommand { get; private set; }

        public int PagesLoaded { get; set; }

        private double _modInstallProgress;
        public double ModInstallProgress
        {
            get { return _modInstallProgress; }
            set
            {
                _modInstallProgress = value;
                OnPropertyChanged();
            }
        }

        private string _modInstallMessage;
        public string ModInstallMessage
        {
            get { return _modInstallMessage; }
            set
            {
                _modInstallMessage = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Mod> _modList;
        public ObservableCollection<Mod> ModList
        {
            get { return _modList; }
            set
            {
                _modList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Mod> _installedModList;
        public ObservableCollection<Mod> InstalledModList
        {
            get { return _installedModList; }
            set
            {
                _installedModList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private async Task LoadModList()
        {
            ModList = new ObservableCollection<Mod>(await _handler.GetModsByPageAsync(1));
            PagesLoaded = 1;
        }

        private async Task LoadInstalledModList()
        {
            await Task.Run(() =>
            {
                var modList = new List<Mod>();

                foreach (var file in
                        Directory.EnumerateFiles(Utilities.GetFactorioModPath(), "*.zip", SearchOption.TopDirectoryOnly))
                {
                    using (ZipFile zip = ZipFile.Read(file))
                    {
                        var folderName = Path.GetFileName(file).Replace(".zip", "/");

                        ZipEntry e = zip[$"{folderName}info.json"];

                        if (e == null)
                        {
                            var regex = new Regex(Regex.Escape("_"));
                            var newText = regex.Replace(folderName, " ", 1);
                            e = zip[$"{newText}info.json"];
                            if (e == null)
                                continue;
                        }

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

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    InstalledModList = new ObservableCollection<Mod>(modList);
                });
            });
        }

        private bool IsModEnabled(string name)
        {
            return Utilities.GetInstalledMods().Mods.FirstOrDefault(x => x.Name == name)?.Enabled ?? false;
        }

        public async void LoadAnotherPage()
        {
            var tempList = new List<Mod>(ModList);
            var newList = await _handler.GetModsByPageAsync(++PagesLoaded);
            ModList = new ObservableCollection<Mod>(tempList.Concat(newList).ToList());
        }

        private void OnModEnabled(string modName)
        {
            var mods = Utilities.GetInstalledMods();
            var theMod = mods.Mods.FirstOrDefault(x => x.Name == modName);

            if (theMod != null)
            {
                theMod.Enabled = true;
                var json = JsonConvert.SerializeObject(mods, Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                File.WriteAllText(Path.Combine(Utilities.GetFactorioModPath(), "mod-list.json"), json);
            }
            else
            {
                var mod = new InstalledMod
                {
                    Name = modName,
                    Enabled = true
                };
                mods.Mods.Add(mod);
                var json = JsonConvert.SerializeObject(mods, Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                File.WriteAllText(Path.Combine(Utilities.GetFactorioModPath(), "mod-list.json"), json);
            }
        }

        private void OnModDisabled(string modName)
        {
            var mods = Utilities.GetInstalledMods();
            var theMod = mods.Mods.FirstOrDefault(x => x.Name == modName);

            if (theMod != null)
            {
                theMod.Enabled = false;
                var json = JsonConvert.SerializeObject(mods, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                File.WriteAllText(Path.Combine(Utilities.GetFactorioModPath(), "mod-list.json"), json);
            }
        }

        private async void InstallMod(Mod mod)
        {
            var modFile = mod.Releases[0].Files[0];
            var url = string.IsNullOrEmpty(modFile.Url) || (!string.IsNullOrEmpty(modFile.Mirror) && modFile.Url.Contains(".php")) ? modFile.Mirror : modFile.Url;
            var uri = new Uri(url);
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
                client.Headers.Add(HttpRequestHeader.Referer, "http://www.factoriomods.com/");

                var fileName = Path.GetFileName(uri.LocalPath);
                //if (fileName.Contains(".php"))
                //{
                //    await Task.Run(() =>
                //    {
                //        using (Stream stream = client.OpenRead(uri))
                //        {
                //            string contentDisposition = client.ResponseHeaders["content-disposition"];
                //            if (!string.IsNullOrEmpty(contentDisposition))
                //            {
                //                string lookFor = "filename*=UTF-8";
                //                int index = contentDisposition.IndexOf(lookFor,
                //                    StringComparison.CurrentCultureIgnoreCase);
                //                if (index >= 0)
                //                    fileName =
                //                        contentDisposition.Substring(index + lookFor.Length)
                //                            .Replace("\"", "")
                //                            .Replace("'", "");
                //            }
                //        }
                //    });
                //}
                client.DownloadFileAsync(uri, Path.Combine(Utilities.GetFactorioModPath(), fileName));
                client.DownloadProgressChanged += (sender, args) =>
                {
                    ModInstallProgress = (args.ProgressPercentage);
                    ModInstallMessage = $"Installing {mod.Title}, {args.ProgressPercentage}%";
                };
                client.DownloadFileCompleted += async (sender, args) =>
                {
                    var fileLocation = Path.Combine(Utilities.GetFactorioModPath(), fileName);
                    var zipFileFinalName = string.Empty;
                    try
                    {
                        using (ZipFile zip = ZipFile.Read(fileLocation))
                        {
                            ZipEntry e = zip[0];
                            var modFolderName = e.FileName;
                            zipFileFinalName = modFolderName.Replace("/", ".zip");
                        }

                        File.Move(fileLocation, Path.Combine(Utilities.GetFactorioModPath(), zipFileFinalName));
                    }
                    catch (Exception ex)
                    {
                        ModInstallMessage = $"Error installing mod: {ex.Message}";
                        File.Delete(fileLocation);
                        return;
                    }


                    ModInstallMessage = $"{mod.Name} installed.";
                    OnModEnabled(mod.Name);
                    mod.Enabled = true;
                    OnPropertyChanged(nameof(ModList));
                    await LoadInstalledModList();
                };

            }
        }

        private void ReceiveMessage(UriMessage msg)
        {
            MessageBox.Show(msg.Base64EncodedModJson);
        }

        private async void OnLoaded()
        {
            await LoadModList();
            await LoadInstalledModList();
        }





        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
