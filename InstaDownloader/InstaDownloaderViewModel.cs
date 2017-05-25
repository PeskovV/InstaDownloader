using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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
            SaveImageCommand = new DelegateCommand(SaveImageCommandExecute, SaveImageCommandCanExecute);
            ImageLoaded = true;
        }

        #region Commands

        #region DownloadCommand

        public ICommand DownloadCommand { get; set; }

        private async void DownloadCommandExecute()
        {
            ImageLoaded = false;
            Image = null;
            var document = new HtmlWeb().Load(Url);
            var imageNode = document.DocumentNode.SelectNodes("//meta")
                .FirstOrDefault(x => x.Attributes.Contains("property")
                                && x.Attributes["property"].Value == "og:image");
            var typeNode = document.DocumentNode.SelectNodes("//meta")
                .FirstOrDefault(x => x.Attributes.Contains("property")
                                && x.Attributes["property"].Value == "og:type");
            if (typeNode != null)
            {
                MediaType = typeNode.Attributes["content"].Value.ToLower().Contains("video")
                    ? MediaType.Video
                    : MediaType.Image;
            }

            if (imageNode != null)
            {
                var imageUrl = imageNode.Attributes["content"].Value;

                Image = LoadImage(await DownloadImage(imageUrl));
                ImageLoaded = true;
                OnPropertyChanged(nameof(ImageLoaded));
            }
            if (MediaType == MediaType.Video)
            {
                var videoNode = document.DocumentNode.SelectNodes("//meta")
                .FirstOrDefault(x => x.Attributes.Contains("property")
                                && x.Attributes["property"].Value == "og:video");
                VideoUrl = videoNode.Attributes["content"].Value;

            }

        }

        private async Task<byte[]> DownloadImage(string imageUrl)
        {
            return await Task.Run(() =>
            {
                using (var clien = new WebClient())
                {
                    return clien.DownloadData(imageUrl);
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

        #region SaveImageCommand

        public ICommand SaveImageCommand { get; set; }

        private void SaveImageCommandExecute()
        {
            var dialog = new SaveFileDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                byte[] data;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Image));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                File.WriteAllBytes(dialog.FileName, data);
            }

        }

        private bool SaveImageCommandCanExecute()
        {
            return ImageLoaded && Image != null;
        }

        #endregion


        #endregion

        #region Properties

        private BitmapImage _image;
        private string _url;
        private bool _imageLoaded;
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
        
        public bool ImageLoaded
        {
            get { return _imageLoaded; }
            set
            {
                _imageLoaded = value;
                OnPropertyChanged(nameof(ImageLoaded));
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