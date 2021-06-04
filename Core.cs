using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.Core;
using VRC.UserCamera;
using Object = UnityEngine.Object;

namespace Dawn.PostProcessing
{
    internal static class Core
    {
        internal const string ModID = "PostProcessing";
        internal static bool s_UICreated = false;
        internal static bool s_PostProcessing;
        internal static bool s_QuickMenu;
        internal static void Coroutine(this IEnumerator routine) => MelonCoroutines.Start(routine);
        private static bool isInstantiated => CurrentUser != null && IsInWorld;
        internal static bool IsInWorld => currentRoom != null || currentWorldInstance != null;
        
        internal static void RegisterSettings()
        {
            MelonPreferences.CreateCategory(ModID, "Post Processing+");
            MelonPreferences.CreateEntry(ModID, "PostProcessing", true, "Enable Post Processing");
    #if QM
            MelonPreferences.CreateEntry(ModID, "QMToggle", true, "QuickMenu Toggle Button");
            MelonPreferences.CreateEntry(ModID, "WorldQMToggle", false, "QuickMenu Toggle Toggles for World PostProcessing Only");
    #endif
            MelonPreferences.CreateEntry(ModID, "WorldPostProcessing", true, "World's Default PostProcessing");

            #region Object States
            MelonPreferences.CreateEntry(ModID, "Dark-Mode", false);
            MelonPreferences.CreateEntry(ModID, "Bloom", false);
            MelonPreferences.CreateEntry(ModID, "Saturation", false);
            MelonPreferences.CreateEntry(ModID, "Contrast", false);
            MelonPreferences.CreateEntry(ModID, "Temperature", false);
            #endregion
            #region Volume Weights
            MelonPreferences.CreateEntry(ModID, "Dark-Weight", 50f, "Darkness Level (0 -> 100) (WARNING: 50 is enough");
            MelonPreferences.CreateEntry(ModID, "Bloom-Weight", 75f, "Bloom Level (0 -> 100)");
            MelonPreferences.CreateEntry(ModID, "Saturation-Weight", 100f, "Saturation Level (0 -> 100)");
            MelonPreferences.CreateEntry(ModID, "Contrast-Weight", 50f, "Contrast Level (0 -> 100)");
            MelonPreferences.CreateEntry(ModID, "Temperature-Weight", 50f, "Temperature Level (0 -> 100)");
            #endregion
            #region Profile Values
            MelonPreferences.CreateEntry(ModID, "DarknessValue", 50f, "Advanced: Darkness Value (0 -> 100)");
            MelonPreferences.CreateEntry(ModID, "BloomValue", 20f, "Advanced: Bloom Value (0 -> 100)");
            MelonPreferences.CreateEntry(ModID, "ContrastValue", 25f, "Advanced: Contrast Value (-90 -> 90)");
            MelonPreferences.CreateEntry(ModID, "SaturationValue", 50f, "Advanced: Saturation Value (-100 -> 100)");
            MelonPreferences.CreateEntry(ModID, "TemperatureValue", 45f, "Advanced: Temperature Value (-100 (Blue) -> 100 (Red))");
            #endregion

            InternalSettingsRefresh();
        }
        internal static void InternalSettingsRefresh()
        {
            var debugList = new List<string>();
            try
            {
                s_PostProcessing = MelonPreferences.GetEntryValue<bool>(ModID, "PostProcessing");
                s_QuickMenu = MelonPreferences.GetEntryValue<bool>(ModID, "QMToggle");
                
                if (!s_UICreated) return; //Prevents Errors when other mods call OnPreferencesSaved();
                var ProcessLayer = MainCamera != null ? MainCamera.gameObject != null ? MainCamera.gameObject.GetComponent<PostProcessLayer>() : null : null;
                if (ProcessLayer != null) ProcessLayer.enabled = s_PostProcessing; debugList.Add("Camera");
                WorldVolumes.WorldQMToggle = MelonPreferences.GetEntryValue<bool>(ModID, "WorldQMToggle");
                WorldVolumes.WorldPostProcessing = MelonPreferences.GetEntryValue<bool>(ModID, "WorldPostProcessing");
                WorldVolumes.ToggleWorldVolumes(); debugList.Add("Volumes");
                if (!CustomPostProcessing.m_ObjectsCreated) return;
                UpdateWeights(); debugList.Add("Weights");
                UpdateStates(); debugList.Add("States");
                UpdateProfiles(); debugList.Add("Profiles");

            #if QM
                QuickMenus.QMPrefsRefresh();
            #endif
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                MelonLogger.Error("Please Post your Latest.Log in the VRCMG Discord and @Mention arion#1223 Please.");
                MelonLogger.Error("Debug Dump:");
                
                if (debugList.Count == 0)
                {
                    MelonLogger.Error("Pre Camera");
                    return;
                }
                foreach (var debugEntry in debugList)
                {
                    MelonLogger.Error( "Entry: " + debugEntry + "Checked.");
                    if (debugEntry == "Profiles")
                    {
                        MelonLogger.Error("QMPrefs!!");
                    }
                }
            }
        }

        private static void UpdateWeights()
        {
            CustomPostProcessing.s_DarkMode.m_PostProcessVolume.weight = (MelonPreferences.GetEntryValue<float>(ModID, "Dark-Weight") / 100).Stabalize(0, 90f);
            CustomPostProcessing.s_Bloom.m_PostProcessVolume.weight = (MelonPreferences.GetEntryValue<float>(ModID, "Bloom-Weight") / 100).Stabalize(0, 100f);
            CustomPostProcessing.s_Saturation.m_PostProcessVolume.weight = (MelonPreferences.GetEntryValue<float>(ModID, "Saturation-Weight") / 100).Stabalize(0, 100f);
            CustomPostProcessing.s_Contrast.m_PostProcessVolume.weight = (MelonPreferences.GetEntryValue<float>(ModID, "Contrast-Weight") / 100).Stabalize(0, 90f);
            CustomPostProcessing.s_Temperature.m_PostProcessVolume.weight = (MelonPreferences.GetEntryValue<float>(ModID, "Temperature-Weight") / 100).Stabalize(0, 100f);
        }

        private static void UpdateStates()
        {
            CustomPostProcessing.s_DarkMode.enabled = MelonPreferences.GetEntryValue<bool>(ModID, "Dark-Mode");
            CustomPostProcessing.s_Bloom.enabled = MelonPreferences.GetEntryValue<bool>(ModID, "Bloom");
            CustomPostProcessing.s_Saturation.enabled = MelonPreferences.GetEntryValue<bool>(ModID, "Saturation");
            CustomPostProcessing.s_Contrast.enabled = MelonPreferences.GetEntryValue<bool>(ModID, "Contrast");
            CustomPostProcessing.s_Temperature.enabled = MelonPreferences.GetEntryValue<bool>(ModID, "Temperature");
        }

        private static void UpdateProfiles()
        {
            CustomPostProcessing.m_DarknessValue = MelonPreferences.GetEntryValue<float>(ModID, "DarknessValue").Stabalize(0, 100);
            CustomPostProcessing.m_BloomValue = MelonPreferences.GetEntryValue<float>(ModID, "BloomValue").Stabalize(0, 100);
            CustomPostProcessing.m_ContrastValue = MelonPreferences.GetEntryValue<float>(ModID, "ContrastValue").Stabalize(-90, 90);
            CustomPostProcessing.m_SaturationValue = MelonPreferences.GetEntryValue<float>(ModID, "SaturationValue").Stabalize(-100, 100);
            CustomPostProcessing.m_TemperatureValue = MelonPreferences.GetEntryValue<float>(ModID, "TemperatureValue").Stabalize(-100, 100);
        }

        private static float Stabalize(this float InputValue, float MinValue, float MaxValue) // An attempt to prevent "Why my screen brack!?" Posts in #bug-report 
        {
            if (InputValue >= MaxValue) // People will attempt max value most likely and get a black screen for dark mode, so we install safeties
            {
                return MaxValue;
            }
            return InputValue <= MinValue ? MinValue : InputValue;
        }

        private static UserCameraController cachedController;
        private static UserCameraController controller => cachedController ??= Object.FindObjectOfType<UserCameraController>();

        private static Transform tryGetUserCamera
        {
            get
            {
                try
                {
                    if (controller != null) return controller.transform;
                }
                catch (TypeLoadException) // In-Case the class gets Obfuscated.
                {
                    MelonLogger.Warning("<UserCameraController> Could not be Found, utilizing fallback.");
                    return GameObject.Find("_Application/TrackingVolume/PlayerObjects/UserCamera").transform;
                }
                MelonLogger.Error("Unable to Find UserCameraController GameObject.");
                return null;
            }
        }
        private static Transform cachedPhotoCamera;
        private static Transform PhotoCamera => cachedPhotoCamera ??= tryGetUserCamera.Find("PhotoCamera");
        private static Transform cachedVideoCamera;
        private static Transform VideoCamera => cachedVideoCamera ??= PhotoCamera.Find("VideoCamera");
        internal static IEnumerator LayerChange()
        {
            // 25 Second Timeout
            for (int i = 0; i < 25; i++)
            {
                var ProcessLayer = MainCamera.gameObject != null ? MainCamera.gameObject.GetComponent<PostProcessLayer>() : null;
                if (ProcessLayer == null) { yield return new WaitForSeconds(1); continue; }
                ProcessLayer.enabled = s_PostProcessing;
                if (CustomPostProcessing.hasNativePostProcessing)
                {
                    if (controller == null) continue; if (PhotoCamera == null) continue; if (VideoCamera == null) continue;
                    
                    var photoCameraPostProcessLayer = PhotoCamera.gameObject != null ? PhotoCamera.gameObject.GetComponent<PostProcessLayer>() : null;
                    var videoCameraPostProcessLayer = VideoCamera.gameObject != null ? VideoCamera.gameObject.GetComponent<PostProcessLayer>() : null;
                    if (photoCameraPostProcessLayer != null)
                    {
                        photoCameraPostProcessLayer.enabled = s_PostProcessing;
                        if (videoCameraPostProcessLayer != null) videoCameraPostProcessLayer.enabled = s_PostProcessing;
                    }
                }
                break;
            }
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
        private static ApiWorldInstance currentWorldInstance => FindInstance(typeof(RoomManager), typeof(ApiWorldInstance)).TryCast<ApiWorldInstance>();
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
        private static QuickMenu QuickMenuInstanceCache;
        
        internal static QuickMenu instance
        {
            get
            {
                if (QuickMenuInstanceCache != null) return QuickMenuInstanceCache;
                QuickMenuInstanceCache = FindInstance(typeof(QuickMenu), typeof(QuickMenu))?.TryCast<QuickMenu>();
                return QuickMenuInstanceCache;
            }
        }

        internal static bool Running;
        internal static DateTime CoroutineInitiationTime;
        internal static IEnumerator WorldJoinedCoroutine()
        {
            if (CoroutineInitiationTime > CoroutineInitiationTime.AddSeconds(25)) Running = false; // Timeout for Running to reset.
            CoroutineInitiationTime = DateTime.Now;
            if (Running) yield break; // Prevents Coroutine Running multiple times if WorldJoin is diverted.
            Running = true;
            var sw = new Stopwatch();
            sw.Start();
            for (;;)
            {
                if (isInstantiated)
                {
                    Running = false;
                    yield return new WaitForSeconds(1);
                    {
                        Start.OnWorldJoin();
                    }
                    sw.Stop();
                    yield break;
                }

                if (sw.Elapsed.Seconds >= 100) // This should never happen but a check for it is in place just in case.
                {
                    Running = false;
                    MelonLogger.Warning("WorldJoinedCoroutine took too long and was stopped.");
                    yield break;
                }

                yield return new WaitForSeconds(1); // IEnumerator Speed Control
            }
        }
        internal static void Log(object obj, LogType type = LogType.Log)
        {

            switch (type)
            {
                case LogType.Log:
                    MelonLogger.Msg(obj);
                    break;
                case LogType.Warning:
                    MelonLogger.Warning(obj);
                    break;
                case LogType.Error:
                    MelonLogger.Error(obj);
                    break;
                default:
                    MelonLogger.Msg(obj);
                    break;
            }
        }

        internal enum LogType
        {
            Log,
            Warning,
            Error
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
                if (MainCameraCache is not null) return MainCameraCache;
                if (Camera.main is not null && Camera.main.gameObject.GetComponent<SteamVR_Camera>() != null)
                {
                    return Camera.main;
                }
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