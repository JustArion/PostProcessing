using System;
using System.Collections.Generic;
using System.Linq;
using Dawn.PostProcessing.PostProcessObjects;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;
using static Dawn.PostProcessing.Core;

namespace Dawn.PostProcessing
{
    internal class CustomPostProcessing
    {
        internal static GameObject Base;
        internal static void CreateProcessingObjects() // OnUIManagerInit
        {
            Base = new GameObject("Post-Processing");
            Object.DontDestroyOnLoad(Base);

            CreateCustomObjects();
            m_ObjectsCreated = true;
        }

        internal static bool m_ObjectsCreated = false;
        #region Junk for Later
        internal static CustomVolume s_DarkMode; // Separate volumes as m_PostProcessVolume.weight is a factor.
        internal static CustomVolume s_Saturation;
        internal static CustomVolume s_Bloom;
        internal static CustomVolume s_Contrast;
        internal static CustomVolume s_Temperature;

        internal static PostProcessProfile m_DarkMode;
        internal static PostProcessProfile m_Saturation;
        internal static PostProcessProfile m_Bloom;
        internal static PostProcessProfile m_Contrast;
        internal static PostProcessProfile m_Temperature;

        internal static float m_DarknessValue // Shouldn't need a try/catch (let's hope)
        {
            set //Value is stabilized here to be manageable (0 -> 100)
            { // Example: 75 *- 0.0888 = -6.66 which is our intended value for 75%, doing this, 100 would be -8.88, 50 would be -4.44 all down to 0
                var cleanedValue = value *- 0.0888f;
                s_DarkMode.m_PostProcessProfile.GetSetting<ColorGrading>().postExposure.value = cleanedValue; 
                // Darkness value is intentionally set to be able to go VERY dark to be able to be used as a panic button to prevent seizures.
                // This will be more compatible when I can get Post Processing not to apply to the UI (See LayerArrayCache)
            }
        }

        internal static float m_BloomValue
        {
            set //Value is stabilized here to be manageable (0 -> 100)
            { // Example: 20 / 10 = 2 which is our intended value, the scale is 0-10 for clean range.
                var cleanedvalue = value / 10;
                s_Bloom.m_PostProcessProfile.GetSetting<Bloom>().intensity.value = cleanedvalue;
            }
        }
        internal static float m_SaturationValue // Doesn't need to be stabilized.
        {
            set => s_Saturation.m_PostProcessProfile.GetSetting<ColorGrading>().saturation.value = value;
        }
        internal static float m_ContrastValue // Doesn't need to be stabilized, -90 -> 90 is fine, -100 would leave a white screen so we don't give people that option.
        {
            set => s_Contrast.m_PostProcessProfile.GetSetting<ColorGrading>().contrast.value = value;
        }
        internal static float m_TemperatureValue// Doesn't need to be stabilized.
        {
            set => s_Temperature.m_PostProcessProfile.GetSetting<ColorGrading>().temperature.value = value;
        }
        #endregion
        private static void CreateCustomObjects() // Priority Limit: 1223 -> 1250 (For Volume ID Purposes)
        {
            s_DarkMode = new CustomVolume("Dark-Mode", m_DarkMode, 0.2f);
            s_DarkMode.m_PostProcessVolume.priority = 1224; // Should be the Highest in this case.
            m_DarkMode = null; // We unload this as we don't need it anymore, unloading any lower causes unity to force an unload. eg. Assets.cs #Region Loading
            
            s_Saturation = new CustomVolume("Saturation", m_Saturation, 1f);
            m_Saturation = null;

            s_Bloom = new CustomVolume("Bloom", m_Bloom, 1f);
            m_Bloom = null;

            s_Contrast = new CustomVolume("Contrast", m_Contrast, 0.2f);
            m_Contrast = null;

            s_Temperature = new CustomVolume("Temperature", m_Temperature, 1f);
            m_Temperature = null;
        }
        
        internal static void WorldJoin()
        {
            if (MainCamera.gameObject.GetComponent<PostProcessLayer>() != null) return;
            Log("World is detected to contain no PostProcessLayer. Adding one manually.");
            SetupPostProcessing();
        }

        private static PostProcessResources m_CachedResources;
        internal static PostProcessResources CachedResources
        {
            get => m_CachedResources != null ? m_CachedResources : null;
            set => m_CachedResources = value;
        }

        internal static void GrabLayer()
        {
            var ProcessLayer = MainCamera.gameObject.GetComponent<PostProcessLayer>();
            if (ProcessLayer == null) return;

            var postProcessVolume = Object.FindObjectsOfType<PostProcessVolume>().FirstOrDefault(v => v.priority is < 1223 or > 1250); // My Mod Range
            if (postProcessVolume == null) return;
            foreach (var x in CustomVolume.instances)
            {
                x.gameObject.layer = postProcessVolume.gameObject.layer;
            }
            Log($"PostProcessing  has been set to Layer {postProcessVolume.gameObject.layer}");
        }
        private static void SetupPostProcessing()
        {
            if (CachedResources != null)
            {
                Log("Using Cached <PostProcessResources>");
            }
            else
            {            
                CachedResources = Resources.FindObjectsOfTypeAll<PostProcessResources>().FirstOrDefault(n => n.name == "DefaultPostProcessResources"); // Remove / Improve
                if (CachedResources == null)
                {
                    MelonLogger.Error("Could not find the Resources necessary to construct Post Processing in a Non-PostProcessing World!");
                    return;
                }
            }
            var PPL = MainCamera.gameObject.AddComponent<PostProcessLayer>();

            PPL.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing; // FXAA
            var FXAA = new FastApproximateAntialiasing {fastMode = true, keepAlpha = true};
            PPL.fastApproximateAntialiasing = FXAA;
            PPL.volumeTrigger = MainCamera.transform;
            PPL.m_ShowCustomSorter = true;
            PPL.m_Resources = CachedResources;

            PPL.volumeLayer = -1; //LayerMask.GetMask(m_LayerArray); // Gonna Try To Make this work @ Some Point
            PPL.enabled = s_PostProcessing;
        }

        private static string[] m_LayerArrayCache;
        static string[] m_LayerArray //Should Return everything except UI layers.
        {
            get
            {
                if (m_LayerArrayCache != null) return m_LayerArrayCache;
                var List = new List<string>();
                for (var i = 0; i < 35; i++)
                {
                    // If the layer Doesnt Contain "UI"
                    if (!LayerMask.LayerToName(i).ToUpper().Contains("UI"))
                    {
                        List.Add(LayerMask.LayerToName(i));
                    }
                }
                m_LayerArrayCache = List.ToArray();
                return m_LayerArrayCache;

            }
        }

        [Flags] // Might use
        private enum Layers
        {
            Everything = -1,
            Nothing = 0,
            Default = 1 << 0,
            TransparentFX = 1 << 1,
            IgnoreRaycast = 1 << 2,
            Water = 1 << 4,
            UI = 1 << 5,
            Interactive = 1 << 8,
            Player = 1 << 9,
            PlayerLocal = 1 << 10,
            Environment = 1 << 11,
            UiMenu = 1 << 12,
            Pickup = 1 << 13,
            PickupNoEnvironment = 1 << 14,
            StereoLeft = 1 << 15,
            StereoRight = 1 << 16,
            Walkthrough = 1 << 17,
            MirrorReflection = 1 << 18,
            reserved2 = 1 << 19,
            reserved3 = 1 << 20,
            reserved4 = 1 << 21,
            user0 = 1 << 22,
            user1 = 1 << 23,
            user2 = 1 << 24,
            user3 = 1 << 25,
            user4 = 1 << 26,
            user5 = 1 << 27,
            user6 = 1 << 28,
            user7 = 1 << 29,
            user8 = 1 << 30,
            user9 = 1 << 31
        }
    }
}