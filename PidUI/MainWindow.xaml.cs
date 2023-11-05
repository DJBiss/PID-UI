using PidUI.ViewModel;
using System.Windows;

namespace PidUI
{

    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
            DataContext = vm;
        }
    }
}
