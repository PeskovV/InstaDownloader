using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Models;
using InstaDownloader.Utils;
using InstaDownloader.ViewModels;

namespace InstaDownloader.Factory
{
    public static class ContentFactory
    {
        public static ImageCreator _imageCreator = new ImageCreator();
        public static VideoCreator _videoCreator = new VideoCreator();
        public static SidecarCreator _sidecarCreator = new SidecarCreator();

        public static IEnumerable<ContentViewModel> Create(ContentModel model)
        {
            switch (model.MediaType)
            {
                case MediaType.GraphSidecar:
                    return _sidecarCreator.Create(model);
                case MediaType.GraphImage:
                    return _imageCreator.Create(model);
                case MediaType.GraphVideo:
                    return _videoCreator.Create(model);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
