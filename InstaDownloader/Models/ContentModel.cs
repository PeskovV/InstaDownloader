using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Utils;

namespace InstaDownloader.Models
{
    public class ContentModel
    {
        public Owner Owner { get; set; }
        public Edge EdgeMediaToCaption { get; set; }
        public Location Location { get; set; }
        public MediaType MediaType { get; set; }
        public List<DisplayResource> DisplayResources { get; set; }
        public string VideoUrl { get; set; }
        public List<EdgeSidecarToChildrenEdge> Sidecars { get; set; }

        public static ContentModel Create(InstaMedia model)
        {
            return new ContentModel
            {
                MediaType = model.Graphql.ShortcodeMedia.Typename,
                Owner = model.Graphql.ShortcodeMedia.Owner,
                EdgeMediaToCaption = model.Graphql.ShortcodeMedia.EdgeMediaToCaption,
                Location = model.Graphql.ShortcodeMedia.Location,
                VideoUrl = model.Graphql.ShortcodeMedia.VideoUrl,
                Sidecars = model.Graphql.ShortcodeMedia.EdgeSidecarToChildren?.Edges,
                DisplayResources = model.Graphql.ShortcodeMedia.DisplayResources
            };
        }
    }
}
