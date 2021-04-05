using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Models;

namespace InstaDownloader.Utils
{
    public static class FileExtension
    {
        public static SaveInfo GetImageSaveInfo(string fileName)
        {
            return new SaveInfo
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg",
                FileName = $"photo_{fileName}"
            };
        }

        public static SaveInfo GetVideoSaveInfo(string fileName)
        {
            return new SaveInfo
            {
                Filter = "MP4 Video (.mp4)|*.mp4",
                FileName = $"video_{fileName}"
            };
        }
    }
}
