using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

namespace TogglePostProcessing
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";
        public const string Name = "Toggle Post Processing";

        public const string Version = "1.1.0";
    }
    public class TogglePostProcessing : MelonMod
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

            MelonPrefs.RegisterBool("TogglePostProcessing", "NightMode1", false, "Night Mode - 1");
            MelonPrefs.RegisterBool("TogglePostProcessing", "NightMode2", false, "Night Mode - 2");
            MelonPrefs.RegisterBool("TogglePostProcessing", "NightModeCustom", false, "Night Mode - Custom");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "NightModeCustomLevel", 0, "Night Mode - Custom: Darkness Level");

            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomLow", false, "Bloom - Low");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomMedium", false, "Bloom - Medium");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomHigh", false, "Bloom - High");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomCustom", false, "Bloom - Custom");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "BloomCustomLevel", 0, "Bloom - Custom: Bloom Level");

            GetPrefs();

            MelonLogger.Log("Settings can be configured in UserData\\modprefs.ini or through UIExpansionKit");
            MelonLogger.Log("It is highly recommended that [UIExpansionKit] be used though.");
        }

        private NightMode NightMode = new NightMode();
        private Bloom Bloom = new Bloom();
        public override void OnLevelWasLoaded(int level)
        {
            try
            {
                NightMode.ApplyNightMode();
                Bloom.ApplyBloom();
                GrabWorldVolumes();
                ToggleMethod(ToggleHandler);
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        public override void OnModSettingsApplied()
        {
            GetPrefs();
            ToggleMethod(ToggleHandler);
            NightMode.ApplyNightMode();
            Bloom.ApplyBloom();
        }
        public void GetPrefs()
        {
            try
            {
                ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");

                NightMode.NightMode1Bool = MelonPrefs.GetBool("PostProcessing", "NightMode1");
                NightMode.NightMode2Bool = MelonPrefs.GetBool("PostProcessing", "NightMode2");
                NightMode.NightMode3Bool = MelonPrefs.GetBool("PostProcessing", "NightModeCustom");
                NightMode.NightMode3Float = MelonPrefs.GetFloat("PostProcessing", "NightModeCustomLevel");

                Bloom.Bloom1Bool = MelonPrefs.GetBool("PostProcessing", "BloomLow");
                Bloom.Bloom2Bool = MelonPrefs.GetBool("PostProcessing", "BloomMedium");
                Bloom.Bloom3Bool = MelonPrefs.GetBool("PostProcessing", "BloomHigh");
                Bloom.Bloom4Bool = MelonPrefs.GetBool("PostProcessing", "BloomCustom");
                Bloom.Bloom4Float = MelonPrefs.GetFloat("PostProcessing", "BloomCustomLevel");
            }
            catch (Exception e)
            { MelonLogger.LogError("GetPrefs Error: " + e); }

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
