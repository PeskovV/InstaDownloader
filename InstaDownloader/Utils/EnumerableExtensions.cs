using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaDownloader.Utils
{
    public static class EnumerableExtensions
    {
        public static List<T> ToSingleList<T>(this T obj) =>
            new List<T> { obj };
    }
}
