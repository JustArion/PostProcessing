using System;
using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static MelonLoader.MelonLogger;

namespace Dawn.PostProcessing
{
    internal sealed class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";

        internal const string Name = "PostProcessing";

        internal const string Version = "2.0.0";
    }
    internal sealed class Start : MelonMod
    {
        public override void OnApplicationStart()
        {
            Core.RegisterSettings();
            
            UIXAdvert();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (buildIndex != 0) return; // PPR = <PostProcessResources>
            // Hopefully throwing a Resource lookup in here so early will negate having it called later as a fallback
            // As the game's PPR should be priority 1 and my own priority 2 as a fallback if the home world upon first launch doesn't contain PPR
            CustomPostProcessing.CachedResources = Resources.FindObjectsOfTypeAll<PostProcessResources>().FirstOrDefault(n => n.name == "DefaultPostProcessResources");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) // World Join
        {
            switch (buildIndex) //Prevents being called 3x
            {
                case 0:
                case 1:
                    break;
                default:
                    MelonCoroutines.Start(Core.WorldJoinedCoroutine());
                    break;
            }
        }

        internal static void OnWorldJoin()
        {
            Core.LayerChange(); // This Changes Toggles it if there's a layer before I create one.
            WorldVolumes.WorldJoin();
            CustomPostProcessing.GrabLayer(); // Grabs Current Volume Render Layer (Some Worlds use different layers)
            CustomPostProcessing.WorldJoin(); // This creates one.
        }

        public override void VRChat_OnUiManagerInit()
        { Core.s_UICreated = true;
        #if QM
            QuickMenus.InitQM();
        #endif
            
            MelonCoroutines.Start(Assets.LoadAssets());
            CustomPostProcessing.CreateProcessingObjects();
            Core.InternalSettingsRefresh(); // Object Sync
        }

        public override void OnPreferencesSaved() => Core.InternalSettingsRefresh();

        private static void UIXAdvert()
        {
            Msg(@"Settings can be configured in VRChat\UserData\MelonPreferences.cfg or through 'UI Expansion Kit'");
            
            if (MelonHandler.Mods.Any(mod => mod.Info.Name == "UI Expansion Kit")) return;
            Warning("'UI Expansion Kit' was not detected and could lead to a less optimal experience.");
            Warning("The mod can be found on Github at: https://github.com/knah/VRCMods/releases");
            Warning("or alternatively under the #finished-mods section in the VRChat Modding Group Discord.");
        }

    }
}