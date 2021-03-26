using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.Core;

namespace Dawn.TPP
{
    internal static class Core
    {
        private const string ModID = "TogglePostProcessing";
        internal static bool m_UICreated = false;
        internal static bool m_PostProcessing;
        internal static bool m_QuickMenu;
        private static bool isInstantiated => CurrentUser != null && IsInWorld;
        private static bool IsInWorld => currentRoom != null || currentWorldInstance != null;

        internal static void RegisterSettings()
        {
            MelonPreferences.CreateCategory(ModID, "Toggle Post Processing");
            MelonPreferences.CreateEntry(ModID, "DisablePostProcessing", false, "Disable Post Processing");
    #if QM
            MelonPreferences.CreateEntry(ModID, "QMToggle", true, "QuickMenu Toggle Button");
    #endif

            MelonPreferences.CreateEntry(ModID, "Test1", 1f);
            MelonPreferences.CreateEntry(ModID, "Test2", 1f);
            MelonPreferences.CreateEntry(ModID, "Test3", false);
            
            InternalSettingsRefresh();
        }

        internal static float tmp_test1;
        internal static float tmp_test2;
        internal static bool tmp_test3;
        internal static void InternalSettingsRefresh()
        {
            m_PostProcessing = !MelonPreferences.GetEntryValue<bool>(ModID, "DisablePostProcessing");
            m_QuickMenu = MelonPreferences.GetEntryValue<bool>(ModID, "QMToggle");

            tmp_test1 = MelonPreferences.GetEntryValue<float>(ModID, "Test1");
            tmp_test2 = MelonPreferences.GetEntryValue<float>(ModID, "Test2");
            tmp_test3 = MelonPreferences.GetEntryValue<bool>(ModID, "Test3");
            

            if (!m_UICreated) return; //Prevents Errors when other mods call OnPreferencesSaved();
            var ProcessLayer = MainCamera.gameObject.GetComponent<PostProcessLayer>();
            if (ProcessLayer != null)
            {
                ProcessLayer.enabled = m_PostProcessing;
            }
        }

        internal static void LayerChange()
        {
            var ProcessLayer = MainCamera.gameObject.GetComponent<PostProcessLayer>();
            if (ProcessLayer == null) return;
            ProcessLayer.enabled = m_PostProcessing;
        }
        

        #region References
        private static Il2CppSystem.Object FindInstance(IReflect WhereLooking, Type WhatLooking) // Credits to Teo
        {
            try
            {
                var methodInfo = WhereLooking.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.ReturnType == WhatLooking && m.GetParameters().Length == 0);
                if (methodInfo != null)
                {
                    return (Il2CppSystem.Object) methodInfo.Invoke(null, null);
                }

                MelonLogger.Error("[FindInstance] MethodInfo for " + WhatLooking.Name + " is null");
            }
            catch (Exception e)
            {
                MelonLogger.Error($"[FindInstance] {e}");
            }

            return null;
        }

        internal static void SetParent(this GameObject obj, GameObject target)
        {
            obj.transform.parent = target.transform;
        }

        private static ApiWorld currentRoom => FindInstance(typeof(RoomManager), typeof(ApiWorld)).TryCast<ApiWorld>();

        private static ApiWorldInstance currentWorldInstance =>
            FindInstance(typeof(RoomManager), typeof(ApiWorldInstance)).TryCast<ApiWorldInstance>();

        /// <summary>
        /// Current User Instance Cache.
        /// </summary>
        private static VRCPlayer CachedCurrentUser;

        /// <summary>
        /// Returns the Current User aka the Player object.
        /// </summary>
        private static VRCPlayer CurrentUser
        {
            get
            {
                if (CachedCurrentUser != null) return CachedCurrentUser;
                CachedCurrentUser = FindInstance(typeof(VRCPlayer), typeof(VRCPlayer))?.TryCast<VRCPlayer>();
                return CachedCurrentUser;
            }
        }

        internal static IEnumerator WorldJoinedCoroutine()
        {
            for (;;)
            {
                var sw = new Stopwatch();
                sw.Start();
                if (isInstantiated)
                {
                    yield return new WaitForSeconds(1);
                    {
                        Start.OnWorldJoin();
                    }
                    sw.Stop();
                    yield break;
                }

                if (sw.Elapsed.Seconds >= 100) // This should never happen but a check for it is in place just in case.
                {
                    MelonLogger.Warning("WorldJoinedCoroutine took too long and was stopped.");
                    yield break;
                }

                yield return new WaitForSeconds(1); // IEnumerator Speed Control
            }
        }

        internal static void Log(object obj)
        {
            MelonLogger.Msg(obj);
        }

        
/*
         Testing revealed that Camera.allCameras is the fastest to grab the main camera.
         Conducted 3 Tests: 
         1: Resources.FindObjectsOfTypeAll<PostProcessLayer>() = +- 244k Ticks.
         2: Object.FindObjectOfType<PostProcessingLayer>() = +- 119k Ticks.
         3: Camera.allCameras = +- 2k Ticks (First Test yielded the highest tick count at 27k Ticks.
         
         Results: https://gist.github.com/aee1553b7c67cf6ec57154d488d68fc6
 */
        private static Camera MainCameraCache;
        internal static Camera MainCamera
        {
            get
            {
                if (MainCameraCache != null) return MainCameraCache;
                foreach (var cam in Camera.allCameras)
                {
                    if (cam.gameObject.GetComponent<SteamVR_Camera>() == null) continue;
                    MainCameraCache = cam;
                    return MainCameraCache;
                }
                return null;
            }
        }
        #endregion
    }
}