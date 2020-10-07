using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaDownloader.Models;
using InstaDownloader.ViewModels;

namespace InstaDownloader.Factory
{
    interface IContentCreator
    {
        IEnumerable<ContentViewModel> Create(ContentModel model);
    }
}
