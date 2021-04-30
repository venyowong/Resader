using System.Collections.Generic;
using System.Linq;

namespace Resader.Extensions
{
    public static class ListExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list) => list == null || !list.Any();
    }
}