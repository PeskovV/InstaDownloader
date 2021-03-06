﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using InstaDownloader.Utils;
using Microsoft.Win32;

namespace InstaDownloader.ViewModels
{
    public class ImageViewModel : ContentViewModel
    {
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

        public override void AddAuthor()
        {
            if(!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Author))
            {
                Description += Environment.NewLine;
                Description = string.Concat(Description, $"Автор фото: @{Author}");
            }
        }

        public override void Save()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg",
                FileName = $"photo_{Author}"
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                File.WriteAllBytes(dialog.FileName, Data);
            }
        }

        public override void CopyContent()
        {
            Clipboard.Clear();
            var ms = new MemoryStream(Data);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.EndInit();
            Clipboard.SetImage(bmp);
        }
    }
}
