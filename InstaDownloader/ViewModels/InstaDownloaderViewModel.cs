using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using InstaDownloader.Commands;
using InstaDownloader.Models;
using InstaDownloader.Utils;
using Microsoft.Win32;

namespace InstaDownloader.ViewModels
{
    public class InstaDownloaderViewModel : BaseViewModel
    {
        private InstaMedia _instaMedia;
        private ContentViewModel _content;
        private string _contentPath;
        private bool _contentLoaded;
        private string _finalPhrase;
        private readonly HttpClient _client;
        private bool _busy;
        private bool _defaultCopy;

        public InstaDownloaderViewModel()
        {
            DownloadCommand = new RelayCommand(DownloadCommandExecute);
            SaveCommand = new RelayCommand(x => SaveCommandExecute());
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
                Content?.AddDescription(_finalPhrase);
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

        public ContentViewModel Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

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
            await Content.DownloadBytes(_client);
            ContentLoaded = true;
            Content.AddDescription(FinalPhrase);
            switch (Content.MediaType)
            {
                case MediaType.GraphSidecar:
                    break;
                case MediaType.GraphImage:
                    if (DefaultCopy)
                        CopyCommandExecute(null);
                    break;
                case MediaType.GraphVideo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Busy = false;
        }

        private bool DownloadCommandCanExecute() => !string.IsNullOrWhiteSpace(ContentPath);

        private async Task GetReferences(string url)
        {
            var index = url.LastIndexOf('/');
            url = url.Substring(0, index + 1);
            if (string.IsNullOrWhiteSpace(url))
                return;
            _instaMedia = await GetModel(url);
            switch (_instaMedia.Graphql.ShortcodeMedia.Typename)
            {
                case MediaType.GraphSidecar:
                    Content = CreateViewModel<SidecarViewModel>(MediaType.GraphSidecar, false);
                    break;
                case MediaType.GraphImage:
                    Content = CreateViewModel<ImageViewModel>(MediaType.GraphImage, false);
                    break;
                case MediaType.GraphVideo:
                    Content = CreateViewModel<VideoViewModel>(MediaType.GraphVideo, true);
                    Content.IsVideo = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnPropertyChanged(nameof(Content));
        }

        private async Task<InstaMedia> GetModel(string url)
        {
            var result = await _client.GetStringAsync($"{url}?__a=1");
            return InstaMedia.FromJson(result);
        }

        private T CreateViewModel<T>(MediaType type, bool isVideo) where T : ContentViewModel, new()
        {
            return new T
            {
                MediaType = type,
                Url = isVideo ? GetVideoReference(_instaMedia) : GetImageReference(_instaMedia),
                Author = _instaMedia.Graphql.ShortcodeMedia.Owner.Username,
                Description = _instaMedia.Graphql.ShortcodeMedia.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text,
                Location = _instaMedia.Graphql.ShortcodeMedia.Location?.Name
            };
        }

        private List<string> GetSidecarReferences(InstaMedia model)
        {
            var list = new List<string>();
            foreach (var edge in model.Graphql.ShortcodeMedia.EdgeSidecarToChildren.Edges)
                if (edge.Node.DisplayResources.LastOrDefault() != null)
                    list.Add(edge.Node.DisplayResources.LastOrDefault().Src.ToString());

            return list;
        }

        private string GetImageReference(InstaMedia model)
        {
            if (model.Graphql.ShortcodeMedia.DisplayResources.LastOrDefault() != null)
                return model.Graphql.ShortcodeMedia.DisplayResources.LastOrDefault().Src.ToString();

            return string.Empty;
        }

        private string GetVideoReference(InstaMedia model)
        {
            if (model.Graphql.ShortcodeMedia.DisplayResources.LastOrDefault() != null)
                return model.Graphql.ShortcodeMedia.VideoUrl;

            return string.Empty;
        }

        private void SaveCommandExecute()
        {
            Busy = true;
            Content.Save();
            Busy = true;
        }

        private bool SaveCommandCanExecute() => Content?.Data != null;

        private void CopyCommandExecute(object element) => Content.CopyContent();

        private bool CopyCommandCanExecute(object obj) => Content?.Data != null && Content?.MediaType == MediaType.GraphImage;

        private byte[] GetMediaBytes()
        {
            Busy = true;
            if (Content.MediaType == MediaType.GraphImage || Content.MediaType == MediaType.GraphVideo)
            {
                Busy = false;
                return Content.Data;
            }
            else if (Content.MediaType == MediaType.GraphSidecar && Content is SidecarViewModel sidecar)
            {

            }

            return new byte[0];
        }
    }
}
