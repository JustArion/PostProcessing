using MelonLoader;
using System;
using UnityEngine;

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
                else { UnityEngine.Object.Destroy(BloomObj); }
                if (Bloom2Bool) { BloomEnum = EnumBloom.BloomMedium; }
                else { UnityEngine.Object.Destroy(BloomObj); }
                if (Bloom3Bool) { BloomEnum = EnumBloom.BloomHigh; }
                else { UnityEngine.Object.Destroy(BloomObj); }
                if (Bloom4Bool) { BloomEnum = EnumBloom.BloomCustom; }
                else { UnityEngine.Object.Destroy(BloomObj); }
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        public void ApplyBloom()
        {
            ApplyBloomEnum();
            switch (BloomEnum)
            {
                case EnumBloom.BloomLow:
                    MelonLogger.Log("Bloom - Low Applied.");
                    break;
                case EnumBloom.BloomMedium:
                    MelonLogger.Log("Bloom - Medium Applied.");
                    break;
                case EnumBloom.BloomHigh:
                    MelonLogger.Log("Bloom - High Applied.");
                    break;
                case EnumBloom.BloomCustom:
                    MelonLogger.Log("Bloom - Custom Applied.");
                    break;
                default:
                    break;
            }
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