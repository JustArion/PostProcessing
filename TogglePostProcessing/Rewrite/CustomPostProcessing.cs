using System;
using System.Collections.Generic;
using System.Linq;
using Dawn.TPP.PostProcessObjects;
using MelonLoader;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;
using static Dawn.TPP.Core;
using Bloom = Dawn.TPP.PostProcessObjects.Bloom;

namespace Dawn.TPP
{
    internal static class CustomPostProcessing
    {
        internal static GameObject Base;
        
        internal static GameObject m_Saturation;
        internal static GameObject m_DarkMode;
        internal static void CreateProcessingObjects() // OnUIManagerInit
        {
            Base = new GameObject("[Mod] PostProcessing");
            Object.DontDestroyOnLoad(Base);

            var m_Bloom =  new Bloom("Bloom");
            m_Bloom.gameObject.SetParent(Base);

            m_Saturation = new GameObject("Saturation");
            m_Saturation.SetParent(Base);

            m_DarkMode = new GameObject("Dark Mode");
            m_DarkMode.SetParent(Base);
        }

        internal static void WorldJoin()
        {
            if (MainCamera.gameObject.GetComponent<PostProcessLayer>() != null) return;
            Log("World is detected to contain no PostProcessLayer. Adding one manually.");
            
            SetupPostProcessing();
        }

        internal static PostProcessResources CachedResources;
        private static void SetupPostProcessing()
        {
            CustomPostProcessing.CachedResources = Resources.FindObjectsOfTypeAll<PostProcessResources>().FirstOrDefault(n => n.name == "DefaultPostProcessResources");
            if (CustomPostProcessing.CachedResources != null)
            {
                Core.Log("Caching <PostProcessResources>");
            }
            else
            {
                MelonLogger.Error("Could not find the Resources necessary to construct Post Processing in a Non-PostProcessing World!");
                return;
            }
            var PPL = MainCamera.gameObject.AddComponent<PostProcessLayer>();
            
            PPL.hideFlags = HideFlags.None;
            PPL.volumeTrigger = MainCamera.transform;
            PPL.m_ShowCustomSorter = true;
            PPL.m_Resources = CachedResources;

            PPL.volumeLayer = -1; //LayerMask.GetMask(m_LayerArray);
            PPL.enabled = m_PostProcessing;
        }

        private static string[] m_LayerArrayCache;
        static string[] m_LayerArray //Should Return everything except UI layers.
        {
            get
            {
                if (m_LayerArrayCache != null) return m_LayerArrayCache;
                var List = new List<string>();
                for (var i = 0; i < 50; i++)
                {
                    // If the layer != -1 or Doesnt Contain "UI"
                    if (LayerMask.NameToLayer(LayerMask.LayerToName(i)) != -1 || !LayerMask.LayerToName(i).ToUpper().Contains("UI"))
                    {
                        List.Add(LayerMask.LayerToName(i));
                    }
                }
                m_LayerArrayCache = List.ToArray();
                return m_LayerArrayCache;
            }
        }

        [Flags]
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