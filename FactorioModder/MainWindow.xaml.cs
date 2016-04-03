using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FactorioModder.ViewModels;
using MahApps.Metro.Controls;

namespace FactorioModder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel = new MainViewModel();

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


        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var listBox = (sender as ListBox);
            var scrollBar = GetVisualChild<ScrollViewer>(listBox);

            if (sender == null || scrollBar == null)
                return;

            if (scrollBar.VerticalOffset == scrollBar.ScrollableHeight)
                MainViewModel.ScrollBarScrolledCommand.Execute(null);

        }

        private T GetVisualChild<T>(UIElement parent) where T : UIElement
        {
            T child = null; // default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                UIElement element = (UIElement)VisualTreeHelper.GetChild(parent, i);
                child = element as T;
                if (child == null)
                    child = GetVisualChild<T>(element);
                if (child != null)
                    break;
            }

            return child;
        }
    }
}
