using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
#if Zinnia
using RubyButtonAPI;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace TogglePostProcessing
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";
#if NextRelease
        public const string Name = "TogglePostProcessing";
#endif
        public const string Name = "TogglePostProcessing_[Zinnia";

#if NextRelease
        public const string Version = "1.1.0";
#endif
        public const string Version = "1.0.3";
    }
    public class TogglePostProcessing : MelonMod
    {
        private static bool ToggleHandler;
        public static bool QMToggle;
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
            MelonPrefs.RegisterBool("TogglePostProcessing", "QMToggle", true, "QuickMenu Toggle Button");
#if NextRelease
            MelonPrefs.RegisterBool("TogglePostProcessing", "NightMode1", false, "Night Mode - 1");
            MelonPrefs.RegisterBool("TogglePostProcessing", "NightMode2", false, "Night Mode - 2");
            MelonPrefs.RegisterBool("TogglePostProcessing", "NightModeCustom", false, "Night Mode - Custom");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "NightModeCustomLevel", 0, "Night Mode - Custom: Darkness Level");

            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomLow", false, "Bloom - Low");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomMedium", false, "Bloom - Medium");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomHigh", false, "Bloom - High");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomCustom", false, "Bloom - Custom");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "BloomCustomLevel", 0, "Bloom - Custom: Bloom Level");
#endif
            GetPrefs();

            MelonLogger.Log("Settings can be configured in UserData\\modprefs.ini or through UIExpansionKit");
            MelonLogger.Log("It is highly recommended that [UIExpansionKit] be used though.");
        }
#if NextRelease
        private readonly NightMode NightMode = new NightMode();
        private readonly Bloom Bloom = new Bloom();
#endif
        public override void OnLevelWasLoaded(int level)
        {
            try
            {
#if NextRelease
                NightMode.ApplyNightMode();
                Bloom.ApplyBloom();
#endif
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
            QMToggleMethod(QMToggle);
            TogglePostProcessingQM.setToggleState(!ToggleHandler);
#if NextRelease
            NightMode.ApplyNightMode();
            Bloom.ApplyBloom();
#endif
        }
#if Zinnia
        public static QMToggleButton TogglePostProcessingQM;
        public override void VRChat_OnUiManagerInit()
        {
            try
            {
                TogglePostProcessingQM = new QMToggleButton("ShortcutMenu", /*-0.3*/0.23f, 1.2f/*2.1*/, "TPP", new Action(() =>
                {
                    MelonPrefs.SetBool("TogglePostProcessing", "DisablePostProcessing", true);
                    GetPrefs();
                    ToggleMethod(true);
                }), "OFF", new Action(() =>
                {
                    MelonPrefs.SetBool("TogglePostProcessing", "DisablePostProcessing", false);
                    GetPrefs();
                    ToggleMethod(false);
                }), "Toggle Post Processing", null, null, null, false, !ToggleHandler);
                Arion.SetSizeButtonfor(TogglePostProcessingQM.btnOff, 2.5f, 1.51f);
                Arion.SetSizeButtonfor(TogglePostProcessingQM.btnOn, 2.5f, 1.51f);
                Arion.SetSizeButtonfor(TogglePostProcessingQM.getGameObject(), 1.9f, 1.38f);
                QMToggleMethod(QMToggle);
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
#endif
        public void QMToggleMethod(bool QMToggle)
        {
            TogglePostProcessingQM.setActive(QMToggle);
        }

        public void GetPrefs()
        {
            try
            {
                ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
                QMToggle = MelonPrefs.GetBool("TogglePostProcessing", "QMToggle");
#if NextRelease
                NightMode.NightMode1Bool = MelonPrefs.GetBool("PostProcessing", "NightMode1");
                NightMode.NightMode2Bool = MelonPrefs.GetBool("PostProcessing", "NightMode2");
                NightMode.NightMode3Bool = MelonPrefs.GetBool("PostProcessing", "NightModeCustom");
                NightMode.NightMode3Float = MelonPrefs.GetFloat("PostProcessing", "NightModeCustomLevel");

                Bloom.Bloom1Bool = MelonPrefs.GetBool("PostProcessing", "BloomLow");
                Bloom.Bloom2Bool = MelonPrefs.GetBool("PostProcessing", "BloomMedium");
                Bloom.Bloom3Bool = MelonPrefs.GetBool("PostProcessing", "BloomHigh");
                Bloom.Bloom4Bool = MelonPrefs.GetBool("PostProcessing", "BloomCustom");
                Bloom.Bloom4Float = MelonPrefs.GetFloat("PostProcessing", "BloomCustomLevel");
#endif
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
