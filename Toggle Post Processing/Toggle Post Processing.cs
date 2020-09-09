using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

namespace Toggle_PostProcessing
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";
        public const string Name = "Toggle Post Processing";

        public const string Version = "1.0.2";
    }
    public sealed class TogglePostProcessing : MelonMod
    {
        private static bool ToggleHandler;
        private static List<OriginalVolume> OriginalVolumes;
        public struct OriginalVolume
        {

            public PostProcessVolume postProcessVolume;

            public bool defaultState;

        }
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("TogglePostProcessing", "Toggle Post Processing");
            MelonPrefs.RegisterBool("TogglePostProcessing", "DisablePostProcessing", false, "Disable Post Processing");
            ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
            MelonLogger.Log("Settings can be configured in UserData\\modprefs.ini or through UIExpansionKit");
        }
        public override void OnLevelWasLoaded(int level)
        {
            GrabWorldVolumes();
            ToggleMethod(ToggleHandler);
        }
        public override void OnModSettingsApplied()
        {
            ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
            ToggleMethod(ToggleHandler);
        }

        private static void GrabWorldVolumes() //Credits to Psychloor for Method
        {
            try
            {
                OriginalVolumes = new List<OriginalVolume>();
                foreach (var volume in UnityEngine.Object.FindObjectsOfType<PostProcessVolume>())
                {
                    OriginalVolumes.Add(new OriginalVolume() { postProcessVolume = volume, defaultState = volume.enabled });
                }
            }
            catch (Exception e)
            { MelonLogger.LogError("GrabWorldVolumes Error: " + e); }

        }
        private static void Reset() //Credits to Psychloor for Method
        {
            try
            {
                foreach (OriginalVolume originalVolume in OriginalVolumes)
                {
                    originalVolume.postProcessVolume.enabled = originalVolume.defaultState;
                }
            }
            catch (Exception e)
            { MelonLogger.LogError("Reset Error: " + e); }
        }
        private static void ToggleMethod(bool value)
        {
            try
            {
                if (value)
                {
                    foreach (OriginalVolume originalVolume in OriginalVolumes)
                    {
                        originalVolume.postProcessVolume.enabled = !value;
                    }
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            { MelonLogger.LogError("ToggleMethod Error: " + e);}
        }
    }
}
