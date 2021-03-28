using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Models;
using InstaDownloader.Utils;
using InstaDownloader.ViewModels;

namespace InstaDownloader.Factory
{
    public class SidecarCreator : IContentCreator
    {
        public IEnumerable<ContentViewModel> Create(ContentModel model)
        {
            if (model.Sidecars == null)
                return new List<ContentViewModel>();
            var contents = new List<ContentViewModel>();
            foreach (var sidecar in model.Sidecars)
            {
                switch (sidecar.Node.Typename)
                {
                    case MediaType.GraphImage:
                        contents.Add(CreateImageViewModel(model, sidecar.Node));
                        break;
                    case MediaType.GraphVideo:
                        contents.Add(CreateVideoViewModel(model, sidecar.Node));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return contents;
        }

        private ImageViewModel CreateImageViewModel(ContentModel model, FluffyNode sidecar)
        {
            return new ImageViewModel
            {
                MediaType = MediaType.GraphImage,
                Url = GetImageReference(sidecar.DisplayResources),
                Author = model.Owner.Username,
                Description = model.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text,
                Location = model.Location?.Name,
                IsVideo = false
            };
        }

        private VideoViewModel CreateVideoViewModel(ContentModel model, FluffyNode sidecar)
        {
            return new VideoViewModel
            {
                MediaType = MediaType.GraphVideo,
                Url = GetVideoReference(sidecar),
                Author = model.Owner.Username,
                Description = model.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text,
                Location = model.Location?.Name,
                IsVideo = true
            };
        }

        private string GetImageReference(IReadOnlyCollection<DisplayResource> displayResources)
        {
            return displayResources.LastOrDefault() != null ? displayResources.Last().Src.ToString() : string.Empty;
        }

        private string GetVideoReference(FluffyNode sidecar)
        {
            return !string.IsNullOrEmpty(sidecar.VideoUrl) ? sidecar.VideoUrl : string.Empty;
        }
    }
}
