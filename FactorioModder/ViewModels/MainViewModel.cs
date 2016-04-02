using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FactorioModder.Annotations;
using FactorioModder.Models;
using FactorioModder.Utility;

namespace FactorioModder.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private ModsAPIHandler _handler;

        public MainViewModel()
        {
            _handler = new ModsAPIHandler();
            LoadMods();
        }

        #region Properties

        private List<Mod> _modList;
        public List<Mod> ModList
        {
            get { return _modList; }
            set
            {
                _modList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private async Task LoadMods()
        {
            ModList = await _handler.GetModsByPageAsync(1);
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
