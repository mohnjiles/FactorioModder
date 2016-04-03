using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FactorioModder.Annotations;

namespace FactorioModder.Models
{
    public class Mod : INotifyPropertyChanged
    {
        private bool _enabled;
        private bool _installed;
        public int Id { get; set; }
        public string Url { get; set; }
        public List<string> Categories { get; set; }
        public string Author { get; set; }
        public string Contact { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Homepage { get; set; }
        public string Version { get; set; }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged();
            }
        }

        public bool Installed
        {
            get { return _installed; }
            set
            {
                _installed = value;
                OnPropertyChanged();
            }
        }

        public List<Release> Releases { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
