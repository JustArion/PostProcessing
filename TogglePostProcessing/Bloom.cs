using MelonLoader;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TogglePostProcessing
{
    internal class Bloom
    {
        private static GameObject BloomObj = new GameObject();
        private static EnumBloom BloomEnum;

        internal static bool Bloom1Bool;
        internal static bool Bloom2Bool;
        internal static bool Bloom3Bool;
        internal static bool Bloom4Bool;
        internal static float Bloom4Float;
        internal static void ApplyBloomEnum()
        {
            try
            {
                if (Bloom1Bool) { BloomEnum = EnumBloom.BloomLow; }
                else { Object.Destroy(BloomObj); }
                if (Bloom2Bool) { BloomEnum = EnumBloom.BloomMedium; }
                else { Object.Destroy(BloomObj); }
                if (Bloom3Bool) { BloomEnum = EnumBloom.BloomHigh; }
                else { Object.Destroy(BloomObj); }
                if (Bloom4Bool) { BloomEnum = EnumBloom.BloomCustom; }
                else { Object.Destroy(BloomObj); }
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        internal void ApplyBloom()
        {
            GameObject BloomObj = new GameObject();
            BloomObj.layer = 8; // Setting the layer to 8 is the PostProcessing layer.
            ApplyBloomEnum();
            switch (BloomEnum)
            {
                case EnumBloom.BloomLow:
                    UnTickAllExcept(Bloom1Bool);
                    MelonLogger.Log("Bloom - Low Applied.");
                    break;
                case EnumBloom.BloomMedium:
                    UnTickAllExcept(Bloom2Bool);
                    MelonLogger.Log("Bloom - Medium Applied.");
                    break;
                case EnumBloom.BloomHigh:
                    UnTickAllExcept(Bloom3Bool);
                    MelonLogger.Log("Bloom - High Applied.");
                    break;
                case EnumBloom.BloomCustom:
                    UnTickAllExcept(Bloom4Bool);
                    MelonLogger.Log("Bloom - Custom Applied.");
                    break;
                default:
                    break;
            }
        }
        internal void UnTickAllExcept(bool Bloom)
        {
            Bloom1Bool = false;
            Bloom2Bool = false;
            Bloom3Bool = false;
            Bloom4Bool = false;
            Bloom = true;
        }
        internal enum EnumBloom
        {
            //Null,
            BloomLow,
            BloomMedium,
            BloomHigh,
            BloomCustom
        }
    }
}