namespace InstaDownloader.ViewModels
{
    using Microsoft.Toolkit.Mvvm.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Factory;
    using Models;
    using Utils;

    public class InstaDownloaderViewModel : BaseViewModel
    {
        private ObservableCollection<ContentViewModel> _contents;
        //private ContentViewModel _content;
        private string _contentPath;
        private bool _contentLoaded;
        private readonly HttpClient _client;
        private bool _busy;
        private bool _defaultCopy;

        public InstaDownloaderViewModel()
        {
            DownloadCommand = new AsyncRelayCommand<string>(DownloadCommandExecute, DownloadCommandCanExecute);
            SaveCommand = new RelayCommand<IList<ContentViewModel>>(SaveCommandExecute, SaveCommandCanExecute);
            CopyCommand = new RelayCommand<IList<ContentViewModel>>(CopyCommandExecute, CopyCommandCanExecute);
            DefaultCopy = true;
            _client = new HttpClient();
        }

        public IAsyncRelayCommand<string> DownloadCommand { get; }
        public IRelayCommand<IList<ContentViewModel>> CopyCommand { get; }
        public IRelayCommand<IList<ContentViewModel>> SaveCommand { get; }

        public bool DefaultCopy
        {
            get => _defaultCopy;
            set => SetProperty(ref _defaultCopy, value);
        }

        public ObservableCollection<ContentViewModel> Contents
        {
            get => _contents;
            set => SetProperty(ref _contents, value);
        }

        public ContentViewModel Content => Contents?.FirstOrDefault();

        public string ContentPath
        {
            get => _contentPath;
            set
            {
                SetProperty(ref _contentPath, value);
                OnPropertyChanged(nameof(IsCorrectUrl));
            }
        }

        public bool ContentLoaded
        {
            get => _contentLoaded;
            set => SetProperty(ref _contentLoaded, value);
        }

        public bool Busy
        {
            get => _busy;
            set => SetProperty(ref _busy, value);
        }

        public bool IsCorrectUrl
        {
            get
            {
                if (ContentPath == null || ContentPath.Length < 40)
                    return false;
                var domen = ContentPath?.Substring(0, 28);
                var link = ContentPath?.Substring(28, 11);
                return domen.Equals("https://www.instagram.com/p/") && 
                       link.Length == 11 && 
                       ContentPath.EndsWith('/');
            }
        }

        private async Task DownloadCommandExecute(string contentPath)
        {
            ContentLoaded = false;
            Busy = true;
            await GetReferences(contentPath);
            await DownloadBytes();
            ContentLoaded = true;
            if(Contents.FirstOrDefault(x => x.MediaType == MediaType.GraphImage) != null && DefaultCopy)
                CopyCommandExecute(Contents);
            Busy = false;
        }

        private bool DownloadCommandCanExecute(string contentPath) => !string.IsNullOrWhiteSpace(contentPath) && IsCorrectUrl;

        private async Task DownloadBytes()
        {
            foreach (var content in Contents)
                await content.DownloadBytes(_client);
        }

        private async Task GetReferences(string url)
        {
            var index = url.LastIndexOf('/');
            url = url.Substring(0, index + 1);
            if (string.IsNullOrWhiteSpace(url))
                return;
            var contentModel = await GetModel(url);
            Contents = contentModel.MediaType switch
            {
                MediaType.GraphSidecar => new ObservableCollection<ContentViewModel>(
                    ContentFactory.Create(contentModel)),
                MediaType.GraphImage => new ObservableCollection<ContentViewModel>(ContentFactory.Create(contentModel)),
                MediaType.GraphVideo => new ObservableCollection<ContentViewModel>(ContentFactory.Create(contentModel)),
                _ => throw new ArgumentOutOfRangeException()
            };
            OnPropertyChanged(nameof(Content));
        }

        private async Task<ContentModel> GetModel(string url)
        {
            try
            {
                var result = await _client.GetStringAsync($"{url}?__a=1");
                var instaMedia = InstaMedia.FromJson(result);
                return ContentModel.Create(instaMedia);
            }
            catch (UriFormatException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SaveCommandExecute(IList<ContentViewModel> contents)
        {
            Busy = true;
            if(Contents?.Count == 1)
            {
                Contents.First().Save();
                Busy = false;
                return;
            }

            SaveAllContent();
            Busy = false;
        }

        private void SaveAllContent()
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) 
                return;
            var folder = dlg.SelectedPath;
            foreach (var content in Contents)
                content.SaveWithoutDialog(folder, Contents.IndexOf(content));
        }

        private bool SaveCommandCanExecute(IList<ContentViewModel> contents) => contents?.Any(x => x.Data != null) ?? false;

        private void CopyCommandExecute(IList<ContentViewModel> contents) => contents?.FirstOrDefault()?.CopyContent();

        private bool CopyCommandCanExecute(IList<ContentViewModel> contents) => 
            contents?.Count == 1 && 
            contents.FirstOrDefault()?.Data != null &&
            contents.FirstOrDefault()?.MediaType == MediaType.GraphImage;
    }
}
