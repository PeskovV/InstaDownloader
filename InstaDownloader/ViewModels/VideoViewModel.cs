using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace InstaDownloader.ViewModels
{
    public class VideoViewModel : ContentViewModel
    {
        public override void AddAuthor()
        {
            if (!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Author))
            {
                Description += Environment.NewLine;
                Description = string.Concat(Description, $"Автор видео: @{Author}");
            }
        }

        public override void Save()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "MP4 Video (.mp4)|*.mp4",
                FileName = $"video_{Author}"
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                File.WriteAllBytes(dialog.FileName, Data);
            }
        }
    }
}
