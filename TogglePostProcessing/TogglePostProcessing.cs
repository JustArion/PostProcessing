using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using RubyButtonAPI;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TogglePostProcessing.DawnRefs;
using UnityEngine;
using UnityEngine.UI;
using VRCSDK2;
using static MelonLoader.MelonLogger;

namespace TogglePostProcessing
{
    internal sealed class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";

        internal const string Name = "TogglePostProcessing";

        internal const string Version = "1.1.5";

    }
    public sealed class TogglePostProcessing : MelonMod
    {
        private static bool ToggleHandler;
        private static bool QMToggle;
        private static bool UIManagerStarted;
        private static List<OriginalVolume> OriginalVolumes;
        private struct OriginalVolume
        {
            public PostProcessVolume postProcessVolume;
            public bool defaultState;
        }
        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("TogglePostProcessing", "Toggle Post Processing");
            MelonPreferences.CreateEntry("TogglePostProcessing", "DisablePostProcessing", false, "Disable Post Processing");
            //QM Stuff
            MelonPreferences.CreateEntry("TogglePostProcessing", "QMToggle", true, "QuickMenu Toggle Button");
            #region 2.0
#if TwoPointZero
            MelonPrefs.RegisterFloat("TigglePostProcessing", "NightMode1", 0, "Night Mode - 1");
            MelonPrefs.RegisterFloat("TigglePostProcessing", "NightMode2", 0, "Night Mode - 2");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "NightModeCustomLevel", 0, "Night Mode - Custom: Darkness Level");
            ExpansionKitApi.RegisterSimpleMenuButton(ExpandedMenu.SettingsMenu, "Night Mode - Custom", //action);

            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomLow", false, "Bloom - Low");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomMedium", false, "Bloom - Medium");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomHigh", false, "Bloom - High");
            MelonPrefs.RegisterBool("TogglePostProcessing", "BloomCustom", false, "Bloom - Custom");
            MelonPrefs.RegisterFloat("TogglePostProcessing", "BloomCustomLevel", 0, "Bloom - Custom: Bloom Level");
#endif
            #endregion
            GetPrefs();

            Msg("Settings can be configured in UserData/modprefs.ini or through UIExpansionKit");
        }
#if TwoPointZero

        internal readonly NightMode NightMode = new NightMode();
        internal readonly Bloom Bloom = new Bloom();
#endif
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)
            {
                case 0:
                case 1:
                break;
                default:
                    try
                    {
                        GrabWorldVolumes();
                        GetPrefs();
                        ToggleMethod(ToggleHandler);
                    #if TwoPointZero
                        NightMode.ApplyNightMode();
                        Bloom.ApplyBloom();
                    #endif
                    }
                    catch (Exception e)
                    {
                        Error($"MelonMod Error: {e}");
                    }
                    break;
            }
        }

        public override void OnPreferencesSaved()
        {
            if (!UIManagerStarted) return;
            try
            {
                GetPrefs();
                ToggleMethod(ToggleHandler);
#if QM
                TPPQM.setActive(QMToggle);
                TPPQM.setToggleState(!ToggleHandler);
#endif
#if TwoPointZero
                NightMode.ApplyNightMode(); 
                Bloom.ApplyBloom();
#endif
            }
            catch (Exception e)
            {
                Error($"MelonMod Error: {e}");
            }
        }
#if QM
        #region QuickMenu
        private static QMToggleButton TPPQM;
        private const float QMX = 2.7f;
        private const float QMY = 1.8f;
        public override void VRChat_OnUiManagerInit()
        {
            UIManagerStarted = true;
            try
            {
                TPPQM = new QMToggleButton("ShortcutMenu", 1.24f, 1.75f, "TPP", () =>
                {
                    MelonPreferences.SetEntryValue("TogglePostProcessing", "DisablePostProcessing", false);
                    ToggleHandler = false;
                    ToggleMethod(ToggleHandler);
                }, "OFF", () =>
                {
                    MelonPreferences.SetEntryValue("TogglePostProcessing", "DisablePostProcessing", true);
                    ToggleHandler = true;
                    ToggleMethod(ToggleHandler);
                }, "Toggle Post Processing");
                TPPQM.btnOff.SetSizeButtonfor(QMX, QMY);
                TPPQM.btnOn.SetSizeButtonfor(QMX, QMY);
                TPPQM.getGameObject().SetSizeButtonfor(1.9f, 1.8f);
                TPPQM.getGameObject().GetComponent<Image>().enabled = false;
                TPPQM.getGameObject().AddComponent<BoxCollider>();
                TPPQM.getGameObject().AddComponent<VRC_UiShape>();
                TPPQM.getGameObject().AddComponent<GraphicRaycaster>();
                TPPQM.setToggleState(!ToggleHandler);
                TPPQM.setActive(QMToggle);
                TPPQM.getGameObject().name = "TPP Toggle";
            }
            catch (Exception e)
            {
                Error($"MelonMod Error: {e}");
            }
        }
        #endregion
        #endif
        #region Toggle
        internal static void GetPrefs()
        {
            try
            {
                ToggleHandler = MelonPreferences.GetEntryValue<bool>("TogglePostProcessing", "DisablePostProcessing");
                QMToggle = MelonPreferences.GetEntryValue<bool>("TogglePostProcessing", "QMToggle");

                #region 2.0
#if TwoPointZero
                NightMode.NightMode1Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightMode1");
                NightMode.NightMode2Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightMode2");
                NightMode.NightMode3Bool = MelonPrefs.GetBool("TogglePostProcessing", "NightModeCustom");
                NightMode.NightMode3Float = MelonPrefs.GetFloat("TogglePostProcessing", "NightModeCustomLevel");

                Bloom.Bloom1Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomLow");
                Bloom.Bloom2Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomMedium");
                Bloom.Bloom3Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomHigh");
                Bloom.Bloom4Bool = MelonPrefs.GetBool("TogglePostProcessing", "BloomCustom");
                Bloom.Bloom4Float = MelonPrefs.GetFloat("TogglePostProcessing", "BloomCustomLevel");
#endif
                #endregion
            }
            catch (Exception e)
            { 
                Error($"GetPrefs Error: {e}"); 
            }

        }
        private static void GrabWorldVolumes() //Credits to Psychloor for Method
        {
            try
            {
                OriginalVolumes = new List<OriginalVolume>();
                foreach (var volume in Resources.FindObjectsOfTypeAll<PostProcessVolume>())
                {
                    OriginalVolumes.Add(new OriginalVolume() { postProcessVolume = volume, defaultState = volume.enabled });
                }
            }
            catch (Exception e)
            {
                Error($"GrabWorldVolumes Error: {e}"); 
            }

        }
        private static void Reset() //Credits to Psychloor for Method
        {
            try
            {
                foreach (var originalVolume in OriginalVolumes.Where(originalVolume => originalVolume.postProcessVolume))
                {
                    originalVolume.postProcessVolume.enabled = originalVolume.defaultState;
                }
            }
            catch (Exception e)
            {
                Error($"Reset Error: {e}"); 
            }
        }
        private static void ToggleMethod(bool disable)
        {
            if (Refs.CurrentUser == null) return;
            try
            {
                if (disable)
                {
                    if (OriginalVolumes == null) return;
                    foreach (var originalVolume in OriginalVolumes.Where(originalVolume => originalVolume.postProcessVolume))
                    {
                        originalVolume.postProcessVolume.enabled = false;
                    }
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            { 
                Error($"ToggleMethod Error: {e}");
            }
        }
        #endregion
    }
}