using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace TogglePostProcessing
{
    public class NightMode
    {
        //1 Night Mode Object is possible but some people might want to stack their Night Modes to be really dark.
        public static GameObject PostProcessing = new GameObject();


        public static bool NightMode1Bool;
        public static bool NightMode2Bool;
        public static bool NightMode3Bool;
        public static float NightMode3Float;

        public void ApplyNightMode()
        {
            GameObject PostProcessing = new GameObject();
            PostProcessing.layer = 8; // Setting the layer to 8 is the PostProcessing layer.
        }
    }
}