using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using InstaDownloader.Commands;
using InstaDownloader.Factory;
using InstaDownloader.Models;
using InstaDownloader.Utils;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace InstaDownloader.ViewModels
{
    public class InstaDownloaderViewModel : BaseViewModel
    {
        private ObservableCollection<ContentViewModel> _contents;
        //private ContentViewModel _content;
        private string _contentPath;
        private bool _contentLoaded;
        private string _finalPhrase;
        private readonly HttpClient _client;
        private bool _busy;
        private bool _defaultCopy;

        public InstaDownloaderViewModel()
        {
            DownloadCommand = new RelayCommand(DownloadCommandExecute, DownloadCommandCanExecute);
            SaveCommand = new RelayCommand(SaveCommandExecute, SaveCommandCanExecute);
            CopyCommand = new RelayCommand(CopyCommandExecute, CopyCommandCanExecute);
            DefaultCopy = true;
            _client = new HttpClient();
        }

        public ICommand DownloadCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public string FinalPhrase
        {
            get => _finalPhrase;
            set
            {
                _finalPhrase = value;
                OnPropertyChanged(nameof(FinalPhrase));
                Contents?.FirstOrDefault()?.AddDescription(_finalPhrase);
            }
        }

        public bool DefaultCopy
        {
            get => _defaultCopy;
            set
            {
                _defaultCopy = value;
                OnPropertyChanged(nameof(DefaultCopy));
            }
        }

        //public ContentViewModel Content
        //{
        //    get => _content;
        //    set
        //    {
        //        _content = value;
        //        OnPropertyChanged(nameof(Content));
        //    }
        //}
        
        public ObservableCollection<ContentViewModel> Contents
        {
            get => _contents;
            set
            {
                _contents = value;
                OnPropertyChanged(nameof(Contents));
                OnPropertyChanged(nameof(Content));
            }
        }

        public ContentViewModel Content => Contents?.FirstOrDefault();

        public string ContentPath
        {
            get => _contentPath;
            set
            {
                _contentPath = value;
                OnPropertyChanged(nameof(ContentPath));
            }
        }

        public bool ContentLoaded
        {
            get { return _contentLoaded; }
            set
            {
                _contentLoaded = value;
                OnPropertyChanged(nameof(ContentLoaded));
            }
        }

        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                OnPropertyChanged(nameof(Busy));
            }
        }

        private async void DownloadCommandExecute(object obj)
        {
            ContentLoaded = false;
            Busy = true;
            await GetReferences(ContentPath);
            await DownloadBytes();
            ContentLoaded = true;
            AddDescription();
            if(Contents.FirstOrDefault(x => x.MediaType == MediaType.GraphImage) != null && DefaultCopy)
                CopyCommandExecute(null);
            Busy = false;
        }

        private async Task DownloadBytes()
        {
            foreach (var content in Contents)
                await content.DownloadBytes(_client);
        }

        private void AddDescription() => Contents.ToList().ForEach(x => x.AddDescription(FinalPhrase));
        
        private bool DownloadCommandCanExecute(object obj) => !string.IsNullOrWhiteSpace(ContentPath);

        private async Task GetReferences(string url)
        {
            var index = url.LastIndexOf('/');
            url = url.Substring(0, index + 1);
            if (string.IsNullOrWhiteSpace(url))
                return;
            var contentModel = await GetModel(url);
            switch (contentModel.MediaType)
            {
                case MediaType.GraphSidecar:
                    Contents = new ObservableCollection<ContentViewModel>(ContentFactory.Create(contentModel));
                    break;
                case MediaType.GraphImage:
                    Contents = new ObservableCollection<ContentViewModel>(ContentFactory.Create(contentModel));
                    break;
                case MediaType.GraphVideo:
                    Contents = new ObservableCollection<ContentViewModel>(ContentFactory.Create(contentModel));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnPropertyChanged(nameof(Contents));
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

        private void SaveCommandExecute(object obj)
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
            var dlg = new CommonOpenFileDialog {Title = "Save all content", IsFolderPicker = true};
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) 
                return;
            var folder = dlg.FileName;
            foreach (var content in Contents)
                content.SaveWithoutDialog(folder, Contents.IndexOf(content));
        }

        private bool SaveCommandCanExecute(object obj) => Contents?.Any(x => x.Data != null) ?? false;

        private void CopyCommandExecute(object element) => Contents?.FirstOrDefault()?.CopyContent();

        private bool CopyCommandCanExecute(object obj) => Contents?.Count == 1 &&
                                                          Contents?.FirstOrDefault()?.Data != null && 
                                                          Contents?.FirstOrDefault()?.MediaType == MediaType.GraphImage;
    }
}
