using System.Collections.Generic;
using System.Linq;

namespace CustomAssetImporter.Util
{
    internal static class FrameworkUtil
    {
        internal static bool CheckForDuplicates<T>(IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T> comparer)
        {
            var duplicates = source.Intersect(target, comparer);
            if (duplicates.Any())
            {
                return true;
            }

            return false;
        }
    }
}
