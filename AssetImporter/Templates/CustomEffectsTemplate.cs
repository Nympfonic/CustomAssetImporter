using Systems.Effects;
using UnityEngine;

namespace CustomAssetImporter.Templates
{
    /**
     * Custom Effects Template
     * 
     * Add this script to your root game object containing your effects, and populate the EffectsArray.
     * Make sure you set up your game objects like BSG does in their effects.bundle (located in "EscapeFromTarkov_Data/StreamingAssets/Windows/assets/systems/effects/particlesystems/").
     * HOWEVER, do NOT add the Effects script that BSG adds to their root game object - this script is meant to replace that script.
     */
    public class CustomEffectsTemplate : MonoBehaviour
    {
        public Effects.Effect[] EffectsArray;
    }
}
