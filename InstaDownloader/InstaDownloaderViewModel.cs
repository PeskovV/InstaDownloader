using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;
using InstaDownloader.Annotations;
using Microsoft.Win32;

namespace InstaDownloader
{
    public class InstaDownloaderViewModel : INotifyPropertyChanged
    {
        public InstaDownloaderViewModel()
        {
            DownloadCommand = new DelegateCommand(DownloadCommandExecute, DownloadCommandCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            ContentLoaded = true;
        }

        #region Commands

        #region DownloadCommand

        public ICommand DownloadCommand { get; set; }

        private async void DownloadCommandExecute()
        {
            ContentLoaded = false;
            Image = null;
            Busy = true;
            var document = new HtmlWeb().Load(Url);
            var typeNode = document.DocumentNode.SelectNodes("//meta")
                .FirstOrDefault(x => x.Attributes.Contains("property")
                                && x.Attributes["property"].Value == "og:type");
            if (typeNode != null)
            {
                MediaType = typeNode.Attributes["content"].Value.ToLower().Contains("video")
                    ? MediaType.Video
                    : MediaType.Image;
            }

            if (MediaType == MediaType.Image)
            {
                var imageNode = document.DocumentNode.SelectNodes("//meta")
                .FirstOrDefault(x => x.Attributes.Contains("property")
                                && x.Attributes["property"].Value == "og:image");

                var imageUrl = imageNode.Attributes["content"].Value;

                Image = LoadImage(await DownloadBytes(imageUrl));
                ContentLoaded = true;
            }
            if (MediaType == MediaType.Video)
            {
                var videoNode = document.DocumentNode.SelectNodes("//meta")
                .FirstOrDefault(x => x.Attributes.Contains("property")
                                && x.Attributes["property"].Value == "og:video");
                VideoUrl = videoNode.Attributes["content"].Value;
                ContentLoaded = true;
            }
            Busy = false;

        }

        private async Task<byte[]> DownloadBytes(string url)
        {
            return await Task.Run(() =>
            {
                using (var clien = new WebClient())
                {
                    return clien.DownloadData(url);
                }
            });
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private bool DownloadCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Url);
        }

        #endregion

        #region SaveCommand

        public ICommand SaveCommand { get; set; }

        private async void SaveCommandExecute()
        {
            var dialog = new SaveFileDialog
            {
                Filter = MediaType == MediaType.Image
                    ? "JPEG Image (.jpeg)|*.jpeg"
                    : "MP4 Video (.mp4)|*.mp4"
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var data = await GetMediaBytes();
                File.WriteAllBytes(dialog.FileName, data);
            }

        }

        private async Task<byte[]> GetMediaBytes()
        {
            Busy = true;
            if (MediaType == MediaType.Image)
            {
                byte[] data;
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Image));
                using (var ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                return data;
            }
            var result = await DownloadBytes(VideoUrl);
            Busy = false;
            return result;
        }

        private bool SaveCommandCanExecute()
        {
            return ContentLoaded && (MediaType == MediaType.Image && Image != null
                                     || MediaType == MediaType.Video && !string.IsNullOrEmpty(VideoUrl));
        }

        #endregion


        #endregion

        #region Properties

        private BitmapImage _image;
        private string _url;
        private bool _contentLoaded;
        private MediaType _mediaType;

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
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

        public MediaType MediaType
        {
            get { return _mediaType; }
            set
            {
                _mediaType = value;
                OnPropertyChanged(nameof(MediaType));
                DownloadingMessage = value == MediaType.Image
                    ? "Downloadin image...."
                    : "Downloading video....";
            }
        }
        private string _downloadingMessage;

        public string DownloadingMessage
        {
            get { return _downloadingMessage; }
            set
            {
                _downloadingMessage = value;
                OnPropertyChanged(nameof(DownloadingMessage));
            }
        }

        private string _videoUrl;

        public string VideoUrl
        {
            get { return _videoUrl; }
            set
            {
                _videoUrl = value;
                OnPropertyChanged(nameof(VideoUrl));
            }
        }

        private bool _busy;

        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                OnPropertyChanged(nameof(Busy));
            }
        }


        #endregion

        #region INPC


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}