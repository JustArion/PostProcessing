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

        internal const string Version = "1.1.6";

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
            GetPrefs();

            Msg("Settings can be configured in UserData/modprefs.ini or through UIExpansionKit");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) // World Join
        {
            switch (buildIndex) //Prevents being called 3x
            {
                case 0:
                case 1:
                    break;
                default:
                    Msg($"Grabbing PostProcessVolumes, ToggleHandler is set to {ToggleHandler}");
                    GrabWorldVolumes();
                    GetPrefs();
                    ToggleMethod(ToggleHandler);
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
                    OriginalVolumes.Add(new OriginalVolume { postProcessVolume = volume, defaultState = volume.enabled });
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