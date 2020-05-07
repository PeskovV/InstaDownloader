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
        private readonly HttpClient _client;
        private bool _busy;

        public InstaDownloaderViewModel()
        {
            DownloadCommand = new RelayCommand(x => DownloadCommandExecute(), x => DownloadCommandCanExecute());
            SaveCommand = new RelayCommand(x => SaveCommandExecute(), x => SaveCommandCanExecute());
            CopyCommand = new RelayCommand(CopyCommandExecute, x => CopyCommandCanExecute());
            _client = new HttpClient();
        }

        public ICommand DownloadCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand SaveCommand { get; set; }

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

        private async void DownloadCommandExecute()
        {
            ContentLoaded = false;
            Busy = true;
            await GetReferences(ContentPath);
            switch (Content.MediaType)
            {
                case MediaType.GraphSidecar:
                    break;
                case MediaType.GraphImage:
                case MediaType.GraphVideo:
                    await Content.DownloadBytes(_client);
                    ContentLoaded = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Busy = false;
        }

        private bool DownloadCommandCanExecute() => !string.IsNullOrWhiteSpace(ContentPath);

        private async Task GetReferences(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;
            _instaMedia = await GetModel(url);
            switch (_instaMedia.Graphql.ShortcodeMedia.Typename)
            {
                case MediaType.GraphSidecar:
                    GetSidecarReferences(_instaMedia);
                    break;
                case MediaType.GraphImage:
                    Content = new ImageViewModel
                    {
                        MediaType = MediaType.GraphImage,
                        Url = GetImageReference(_instaMedia),
                        Author = _instaMedia.Graphql.ShortcodeMedia.Owner.Username,
                        Description = _instaMedia.Graphql.ShortcodeMedia.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text
                    };
                    break;
                case MediaType.GraphVideo:
                    Content = new VideoViewModel
                    {
                        MediaType = MediaType.GraphVideo,
                        Url = GetVideoReference(_instaMedia),
                        Author = _instaMedia.Graphql.ShortcodeMedia.Owner.Username,
                        Description = _instaMedia.Graphql.ShortcodeMedia.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text
                    };
                    break;
            }
            Content.ConcatAuthor();
            OnPropertyChanged(nameof(Content));
        }

        private async Task<InstaMedia> GetModel(string url)
        {
            var result = await _client.GetStringAsync($"{url}?__a=1");
            return InstaMedia.FromJson(result);
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
            var dialog = new SaveFileDialog
            {
                Filter = Content.MediaType == MediaType.GraphImage
                    ? "JPEG Image (.jpeg)|*.jpeg"
                    : "MP4 Video (.mp4)|*.mp4"
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var data = GetMediaBytes();
                File.WriteAllBytes(dialog.FileName, data);
            }
        }

        private bool SaveCommandCanExecute() => Content?.Data != null;

        private void CopyCommandExecute(object element)
        {
            Clipboard.Clear();
            var ms = new MemoryStream(Content.Data);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.EndInit();
            Clipboard.SetImage(bmp);
        }

        private bool CopyCommandCanExecute() => Content?.Data != null && Content?.MediaType == MediaType.GraphImage;

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
