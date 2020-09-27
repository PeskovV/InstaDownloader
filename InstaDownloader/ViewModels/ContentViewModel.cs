using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InstaDownloader.Commands;
using InstaDownloader.Models;
using InstaDownloader.Utils;

namespace InstaDownloader.ViewModels
{
    public abstract class ContentViewModel : BaseViewModel
    {
        private MediaType _mediaType;
        private byte[] _data;
        private string _url;
        private string _author;
        private string _description;
        private string _location;
        private bool _isVideo;

        protected ContentViewModel()
        {
            CopyTextCommand = new RelayCommand(CopyText);
        }

        private ICommand _copyTextCommand;
        public ICommand CopyTextCommand
        {
            get => _copyTextCommand;
            set
            {
                _copyTextCommand = value;
                OnPropertyChanged(nameof(CopyTextCommand));
            }
        }

        public MediaType MediaType
        {
            get => _mediaType;
            set
            {
                _mediaType = value;
                OnPropertyChanged(nameof(MediaType));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
                OnPropertyChanged(nameof(ModifiedAuthor));
            }
        }

        public string ModifiedAuthor => $"@{Author}";

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public virtual void AddAuthor() {}

        public virtual void Save() {}

        public virtual void CopyContent() { }

        public virtual void CopyText(object obj)
        {
            if(obj is string str)
                Clipboard.SetText(str);
        }

        public string Url
        {
            get => _url;
            set
            {
                _url = value.Replace("https", "http");
                OnPropertyChanged(nameof(Url));
            }
        }

        public bool IsVideo
        {
            get => _isVideo;
            set
            {
                _isVideo = value;
                OnPropertyChanged(nameof(IsVideo));
            }
        }

        public byte[] Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public virtual async Task DownloadBytes(HttpClient client)
        {
            var data = await client.GetByteArrayAsync(new Uri(Url));
            Data = data;
        }

        public virtual void AddDescription(string description, bool clearAll = false)
        {
            if (!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(description))
            {
                Description += Environment.NewLine;
                Description = string.Concat(Description, description);
            }
        }
    }
}
