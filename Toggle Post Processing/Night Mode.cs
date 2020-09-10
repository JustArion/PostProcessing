using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Threading.Tasks;

namespace TogglePostProcessing
{
    public class NightMode
    {
        //1 Night Mode Object is possible but some people might want to stack their Night Modes to be really dark.
        public static GameObject NightMode1 = new GameObject();
        public static GameObject NightMode2 = new GameObject();
        public static GameObject NightMode3 = new GameObject();

        private static EnumNightMode NightModeEnum;

        public static bool NightMode1Bool;
        public static bool NightMode2Bool;
        public static bool NightMode3Bool;
        public static float NightMode3Float;
        public void ApplyNightEnum()
        {
            try
            {
                //if (!NightMode1Bool && !NightMode2Bool && !NightMode3Bool) { NightModeEnum = EnumNightMode.Null; }
                //else 
                if (NightMode1Bool) { NightModeEnum = EnumNightMode.NightMode1; }
                else { UnityEngine.Object.Destroy(NightMode1); }
                if (NightMode2Bool) { NightModeEnum = EnumNightMode.NightMode2; }
                else { UnityEngine.Object.Destroy(NightMode2); }
                if (NightMode3Bool) { NightModeEnum = EnumNightMode.NightMode3; }
                else { UnityEngine.Object.Destroy(NightMode3); }
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        public void ApplyNightMode()
        {
            ApplyNightEnum();
            NightMode1.layer = 8; // Setting the layer to 8 is the PostProcessing layer.
            NightMode2.layer = 8;
            NightMode3.layer = 8;
            switch (NightModeEnum)
            {
                case EnumNightMode.NightMode1:
                    MelonLogger.Log("Night Mode - 1 Applied.");
                    break;
                case EnumNightMode.NightMode2:
                    MelonLogger.Log("Night Mode - 2 Applied.");
                    break;
                case EnumNightMode.NightMode3:
                    MelonLogger.Log("Night Mode - Custom Applied.");
                    break;
                default:
                    break;
            }
        }
    }
}
public enum EnumNightMode
{
    //Null,
    NightMode1,
    NightMode2,
    NightMode3
}