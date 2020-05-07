using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Utils;

namespace InstaDownloader.ViewModels
{
    public class ContentViewModel : BaseViewModel
    {
        private MediaType _mediaType;
        private byte[] _data;
        private string _url;
        private string _author;
        private string _description;
        private bool _isVideo;

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
            }
        }

        public void ConcatAuthor()
        {
            if (!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Author))
            {
                Description += Environment.NewLine;
                Description = string.Concat(Description, $"Автор фото: @{Author}");
            }
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
    }
}
