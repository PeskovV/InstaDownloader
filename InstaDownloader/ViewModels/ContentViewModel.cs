namespace InstaDownloader.ViewModels
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Utils;
    using Microsoft.Toolkit.Mvvm.ComponentModel;
    using Microsoft.Toolkit.Mvvm.Input;

    public class ContentViewModel : ObservableObject
    {
        private string _author;
        private string _description;
        private string _location;
        private MediaType _mediaType;
        private byte[] _data;
        private string _url;
        private bool _isVideo;

        protected ContentViewModel()
        {
            CopyTextCommand = new RelayCommand<string>(CopyText);
        }

        private ICommand _copyTextCommand;
        public ICommand CopyTextCommand
        {
            get => _copyTextCommand;
            set => SetProperty(ref _copyTextCommand, value);
        }
        public MediaType MediaType
        {
            get => _mediaType;
            set => SetProperty(ref _mediaType, value);
        }

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value.Replace("https", "http"));
        }

        public bool IsVideo
        {
            get => _isVideo;
            set => SetProperty(ref _isVideo, value);
        }

        public byte[] Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public virtual async Task DownloadBytes(HttpClient client)
        {
            var data = await client.GetByteArrayAsync(new Uri(Url));
            Data = data;
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Author
        {
            get => _author;
            set
            {
                SetProperty(ref _author, value);
                OnPropertyChanged(nameof(ModifiedAuthor));
            }
        }

        public string ModifiedAuthor => $"@{Author}";

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public virtual void AddAuthor() {}

        public virtual void SaveWithoutDialog(string pathFolder, int index) {}

        public virtual void Save() {}

        public virtual void CopyContent() { }

        public virtual void CopyText(string str)
        {
            Clipboard.SetText(str);
        }

        public virtual void AddDescription(string description, bool clearAll = false)
        {
            if (string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(description))
                return;
            Description += Environment.NewLine;
            Description = string.Concat(Description, description);
        }
    }
}
