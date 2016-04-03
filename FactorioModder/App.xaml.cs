using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FactorioModder.Managers;
using FactorioModder.Models;
using FactorioModder.Utility;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace FactorioModder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
            var args = Utilities.GetCommandLineArgsList();
            if (args.Count > 0)
            {
                if (Process.GetProcessesByName("FactorioModder").Length > 0)
                {
                    Messenger.Default.Send(new UriMessage
                    {
                        Base64EncodedModJson = args[0]
                    });

                    Shutdown();
                }
            }

            SimpleIoc.Default.Register<ISettingsManager, SettingsManager>(true);
        }
    }
}
