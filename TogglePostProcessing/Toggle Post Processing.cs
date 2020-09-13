using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using RubyButtonAPI;
using UnityEngine;
using UnityEngine.UI;

namespace TogglePostProcessing
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";

        public const string Name = "TogglePostProcessing-[Zinnia";

        public const string Version = "1.0.4";

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
            //QM Stuff
            MelonPrefs.RegisterBool("TogglePostProcessing", "QMToggle", true, "QuickMenu Toggle Button");
            //Did not register floats x,y since 0.1 = quite a move and ExpansionKit has no

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
        public readonly NightMode NightMode = new NightMode();
        public readonly Bloom Bloom = new Bloom();
        public override void OnLevelWasLoaded(int level)
        {
            try
            {
                GrabWorldVolumes();
                ToggleMethod(ToggleHandler);
                //NightMode.ApplyNightMode();
                //Bloom.ApplyBloom();
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }
        public override void OnModSettingsApplied()
        {
            try
            {
                GetPrefs();
                ToggleMethod(ToggleHandler);
                QMToggleMethod(QMToggle);
                TogglePostProcessingQM.setToggleState(!ToggleHandler);
                //NightMode.ApplyNightMode();
                //Bloom.ApplyBloom();
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"MelonMod Error: {e}");
            }
        }

        #region QuickMenu
        public static QMToggleButton TogglePostProcessingQM;
        public override void VRChat_OnUiManagerInit()
        {
            try
            {
                TogglePostProcessingQM = new QMToggleButton("ShortcutMenu", 0.23f, 1.2f, "TPP", new Action(() =>
                {
                    MelonPrefs.SetBool("TogglePostProcessing", "DisablePostProcessing", true);
                    GetPrefs();
                    ToggleMethod(false);
                }), "OFF", new Action(() =>
                {
                    MelonPrefs.SetBool("TogglePostProcessing", "DisablePostProcessing", false);
                    GetPrefs();
                    ToggleMethod(true);
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
        public void QMToggleMethod(bool QMToggle)
        {
            TogglePostProcessingQM.setActive(QMToggle);
        }
        #endregion

        #region Toggle
        public void GetPrefs()
        {
            try
            {
                ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
                QMToggle = MelonPrefs.GetBool("TogglePostProcessing", "QMToggle");
                NightMode.NightMode1Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightMode1");
                NightMode.NightMode2Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightMode2");
                NightMode.NightMode3Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightModeCustom");
                NightMode.NightMode3Float = MelonPrefs.GetFloat("TogglePostProcessing", "NightModeCustomLevel");

                Bloom.Bloom1Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomLow");
                Bloom.Bloom2Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomMedium");
                Bloom.Bloom3Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomHigh");
                Bloom.Bloom4Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomCustom");
                Bloom.Bloom4Float = MelonPrefs.GetFloat("TogglePostProcessing", "BloomCustomLevel");
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
        #endregion
    }
}