using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using InstaDownloader.Models;
using InstaDownloader.Utils;
using InstaDownloader.ViewModels;

namespace InstaDownloader.Factory
{
    public class VideoCreator : IContentCreator
    {
        public IEnumerable<ContentViewModel> Create(ContentModel model)
        {
            return CreateVideoViewModel(model).ToSingleList();
        }

        private VideoViewModel CreateVideoViewModel(ContentModel model)
        {
            return new VideoViewModel
            {
                IsVideo = true,
                MediaType = MediaType.GraphVideo,
                Url = GetVideoReference(model),
                Author = model.Owner.Username,
                Description = model.EdgeMediaToCaption.Edges.FirstOrDefault()?.Node.Text,
                Location = model.Location?.Name
            };
        }

        private string GetVideoReference(ContentModel model)
        {
            return !string.IsNullOrEmpty(model.VideoUrl) ? model.VideoUrl : string.Empty;
        }
    }
}
