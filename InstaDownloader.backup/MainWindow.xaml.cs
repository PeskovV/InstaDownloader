using System.Diagnostics;
using System.Windows.Navigation;
using InstaDownloader.ViewModels;

namespace InstaDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new InstaDownloaderViewModel();
        }
    }
}
