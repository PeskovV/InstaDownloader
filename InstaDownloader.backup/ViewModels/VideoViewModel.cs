using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Models;
using InstaDownloader.Utils;
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
            var fileInfo = FileExtension.GetVideoSaveInfo(Author);
            var dialog = new SaveFileDialog
            {
                Filter = fileInfo.Filter,
                FileName = fileInfo.FileName
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                File.WriteAllBytes(dialog.FileName, Data);
            }
        }

        public override void SaveWithoutDialog(string pathFolder, int index)
        {
            if (!Directory.Exists(pathFolder))
                Directory.CreateDirectory(pathFolder);
            var fileName = $"video_{Author}_{index}.mp4";
            File.WriteAllBytes(Path.Combine(pathFolder, fileName), Data);
        }

        private static string GetVideoReference(InstaMedia model)
        {
            if (model.Graphql.ShortcodeMedia.DisplayResources.LastOrDefault() != null)
                return model.Graphql.ShortcodeMedia.DisplayResources.LastOrDefault()?.Src.ToString();

            return string.Empty;
        }
    }
}
