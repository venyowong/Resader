using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resader.Common.Extensions
{
    public static class ListExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list) => list == null || !list.Any();
    }
}
