using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaDownloader.ViewModels
{
    public class SidecarViewModel : ContentViewModel
    {
        private ObservableCollection<ContentViewModel> _contents;
        private Dictionary<string, byte[]> _content;

        public ObservableCollection<ContentViewModel> Contents
        {
            get => _contents;
            set
            {
                _contents = value;
                OnPropertyChanged(nameof(Contents));
            }
        }

        public Dictionary<string, byte[]> Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public List<byte[]> Images => Content?.Values.ToList();
    }
}
