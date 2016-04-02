using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FactorioModder.ViewModels;

namespace FactorioModder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel;
        }

        private MainViewModel _mainViewModel;
        public MainViewModel MainViewModel
        {
            get
            {
                return _mainViewModel ?? new MainViewModel();
            }
            set { _mainViewModel = value; }
        }
    }
}
