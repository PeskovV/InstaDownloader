using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaDownloader.Models
{
    public class ContentModel
    {
        public ContentModel(string url)
        {
            Url = url;
        }

        public ContentModel(string url, byte[] data) : this(url)
        {
            Data = data;
        }

        public string Url { get; set; }
        public byte[] Data { get; set; }
    }
}
