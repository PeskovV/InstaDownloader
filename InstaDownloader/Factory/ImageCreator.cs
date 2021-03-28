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
    public class ImageCreator : IContentCreator
    {
        public IEnumerable<ContentViewModel> Create(ContentModel model)
        {
            return CreateImageViewModel(model).ToSingleList();
        }

        private ImageViewModel CreateImageViewModel(ContentModel model)
        {
            return new ImageViewModel
            {
                IsVideo = false,
                MediaType = MediaType.GraphImage,
                Url = GetImageReference(model.DisplayResources),
                Author = model.Owner.Username,
                Description = model.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text,
                Location = model.Location?.Name
            };
        }

        private  string GetImageReference(IReadOnlyCollection<DisplayResource> displayResources)
        {
            if (displayResources.LastOrDefault() != null)
                return displayResources.Last().Src.ToString();

            return string.Empty;
        }
    }
}
