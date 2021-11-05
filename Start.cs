using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static MelonLoader.MelonLogger;

namespace Dawn.PostProcessing
{
    internal sealed class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";

        internal const string Name = "PostProcessing+";

        internal const string Version = "2.0.9";
    }
    internal sealed class Start : MelonMod
    {
        public override void OnApplicationStart()
        {
            Core.RegisterSettings();
            
            if (MelonHandler.Mods.All(mod => mod.Info.Name != "UI Expansion Kit")) UIXAdvert();
        }
        

        // private bool VersionCheck(string modVersion, string greaterOrEqual)
        // {
        //     if (Version.TryParse(modVersion, out var owo) && Version.TryParse(greaterOrEqual, out var uwu)) return uwu.CompareTo(owo) <= 0;
        //     return false;
        // }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (buildIndex != 0) return; // PPR = <PostProcessResources>
            // Hopefully throwing a Resource lookup in here so early will negate having it called later as a fallback
            // As the game's PPR should be priority 1 and my own priority 2 as a fallback if the home world upon first launch doesn't contain PPR
            
            CustomPostProcessing.CachedResources = CustomPostProcessing.CachedResources ??= Resources.FindObjectsOfTypeAll<PostProcessResources>().FirstOrDefault(n => n.name == "DefaultPostProcessResources");
        }

        [Credit("DDAkebono", "https://github.com/ddakebono/BTKSAImmersiveHud/blob/8b5968a7cf35398217ad14b86b316dc93fb705fe/BTKSAImmersiveHud.cs#L52")]
        [Modified]
        private static byte? scenesLoaded = 0;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName) // World Join
        {
            switch (buildIndex) //Prevents being called 3x
            {
                case 0:
                case 1:
                    break;
                default:
                    Core.WorldJoinedCoroutine().Coroutine();
                    break;
            }
            
            if (scenesLoaded is null or > 2) return;
            scenesLoaded++;
            if (scenesLoaded != 2) return;
            VRChat_OnUiManagerInit();
            scenesLoaded = null;
        }

        internal static void OnWorldJoin()
        {
            Core.LayerChange().Coroutine(); // This Changes Toggles it if there's a layer before I create one.
            WorldVolumes.WorldJoin();
            CustomPostProcessing.GrabLayer().Coroutine(); // Grabs Current Volume Render Layer (Some Worlds use different layers)
            CustomPostProcessing.WorldJoin().Coroutine(); // This creates one.
            
        }

        private new static void VRChat_OnUiManagerInit()
        {
            Core.s_UICreated = true;
            Msg("VRChat_OnUiManagerInit Sucessfully Called.");
        #if QM
            QuickMenus.InitQM();
        #endif

            Assets.LoadAssets().Coroutine();
            CustomPostProcessing.CreateProcessingObjects();
            Core.InternalSettingsRefresh(); // Object Sync
        }

        public override void OnPreferencesSaved() { if (GracefulExit) return; Core.InternalSettingsRefresh(); }
        public override void OnApplicationQuit() => GracefulExit = true;
        // Thanks NekoMdNight#1401 saying "postprocessing+ seems to not exit very well when you exit vrchat." 
        // Hopefully this will solve potential race conditions for AppQuit if AppQuit destroys all GameObjects in DoNotDestroy;
        private bool GracefulExit;

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