﻿namespace InstaDownloader.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class SidecarViewModel : ContentViewModel
    {
        private ObservableCollection<ContentViewModel> _contents;
        private Dictionary<string, byte[]> _content;

        public ObservableCollection<ContentViewModel> Contents
        {
            get => _contents;
            set => SetProperty(ref _contents, value);
        }

        public Dictionary<string, byte[]> Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public List<byte[]> Images => Content?.Values.ToList();
    }
}
