using System.Collections.Generic;
using System.Linq;

namespace CustomAssetImporter.Util
{
    internal static class FrameworkUtil
    {
        internal static bool CheckForDuplicates<T>(IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T> comparer)
        {
            return source.Intersect(target, comparer).Any();
        }
    }
}
