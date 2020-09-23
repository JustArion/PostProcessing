using MelonLoader;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TogglePostProcessing
{
    public class Bloom
    {
        public static GameObject BloomObj = new GameObject();
        private static EnumBloom BloomEnum;

        public static bool Bloom1Bool;
        public static bool Bloom2Bool;
        public static bool Bloom3Bool;
        public static bool Bloom4Bool;
        public static float Bloom4Float;
        public static void ApplyBloomEnum()
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
        public void ApplyBloom()
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
        public void UnTickAllExcept(bool Bloom)
        {
            Bloom1Bool = false;
            Bloom2Bool = false;
            Bloom3Bool = false;
            Bloom4Bool = false;
            Bloom = true;
        }
        public enum EnumBloom
        {
            //Null,
            BloomLow,
            BloomMedium,
            BloomHigh,
            BloomCustom
        }
    }
}