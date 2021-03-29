using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dawn.PostProcessing.PostProcessObjects;
using MelonLoader;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
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
        internal static CustomVolume s_DarkMode;
        internal static CustomVolume s_Saturation;
        internal static CustomVolume s_Bloom;
        internal static CustomVolume s_Contrast;
        internal static CustomVolume s_Temperature;

        internal static PostProcessProfile m_DarkMode;
        internal static PostProcessProfile m_Saturation;
        internal static PostProcessProfile m_Bloom;
        internal static PostProcessProfile m_Contrast;
        internal static PostProcessProfile m_Temperature;

        // Better Solutions to this are welcome!
        internal static float m_BrightnessValue
        {
            set
            {
                foreach (var x in s_DarkMode.m_PostProcessProfile.settings) { x.TryCast<ColorGrading>().brightness.value = value; }
            }
        }
        internal static float m_SaturationValue
        {
            set
            {
                foreach (var x in s_Saturation.m_PostProcessProfile.settings) { x.TryCast<ColorGrading>().saturation.value = value; }
            }
        }
        internal static float m_ContrastValue
        {
            set
            {
                foreach (var x in s_Contrast.m_PostProcessProfile.settings) { x.TryCast<ColorGrading>().contrast.value = value; }
            }
        }
        internal static float m_TemperatureValue
        {
            set
            {
                foreach (var x in s_Temperature.m_PostProcessProfile.settings) { x.TryCast<ColorGrading>().temperature.value = value; }
            }
        }
        #endregion
        private static void CreateCustomObjects() // Priority Limit: 1223 -> 1250 (For Volume ID Purposes)
        {
            s_DarkMode = new CustomVolume("Dark-Mode", m_DarkMode, 0.2f);
            s_DarkMode.m_PostProcessVolume.priority = 1224; // Should be the Highest in this case.
            
            s_Saturation = new CustomVolume("Saturation", m_Saturation, 1f);

            s_Bloom = new CustomVolume("Bloom", m_Bloom, 1f);
            
            s_Contrast = new CustomVolume("Contrast", m_Contrast, 0.2f);

            s_Temperature = new CustomVolume("Temperature", m_Temperature, 1f);
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
            
            PPL.hideFlags = HideFlags.DontUnloadUnusedAsset;
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