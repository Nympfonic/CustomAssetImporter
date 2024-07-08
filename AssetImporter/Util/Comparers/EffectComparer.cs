using System.Collections.Generic;
using Systems.Effects;

namespace CustomAssetImporter.Util.Comparers
{
    public class EffectComparer : IEqualityComparer<Effects.Effect>
    {
        public bool Equals(Effects.Effect x, Effects.Effect y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Effects.Effect x)
        {
            return x.Name.GetHashCode();
        }
    }
}
