using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaDownloader.ViewModels
{
    public class SidecarViewModel : ContentViewModel
    {
        private List<string> _urls;

        public List<string> Urls
        {
            get => _urls;
            set
            {
                _urls = value;
                OnPropertyChanged(nameof(Urls));
            }
        }
    }
}
