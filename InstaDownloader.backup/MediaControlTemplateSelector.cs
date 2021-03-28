using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InstaDownloader.Utils;
using InstaDownloader.ViewModels;

namespace InstaDownloader
{
    public class MediaControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate SidecarTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ContentViewModel content)
            {
                switch (content.MediaType)
                {
                    case MediaType.GraphSidecar:
                        return SidecarTemplate;
                    case MediaType.GraphImage:
                        return ImageTemplate;
                    case MediaType.GraphVideo:
                        return VideoTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
