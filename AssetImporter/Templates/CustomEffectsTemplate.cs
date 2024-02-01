using Systems.Effects;
using UnityEngine;

namespace CustomAssetImporter.Templates
{
    /**
     * Add this component to your root game object containing your effects, and populate EffectsArray.
     * Make sure you follow the same setup that BSG does for their effects.bundle (located in "EscapeFromTarkov_Data/StreamingAssets/Windows/assets/systems/effects/particlesystems/").
     * HOWEVER, do NOT add the Effects component that BSG adds to their effects.bundle - this component is meant to replace that component.
     */
    public class CustomEffectsTemplate : MonoBehaviour
    {
        public Effects.Effect[] EffectsArray;
    }
}
