using System.Linq;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;


namespace Toggle_PostProcessing
{
    public static class BuildInfo
    {
        public const string Author = "arion#1223";
        public const string Company = null;
        public const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";
        public const string Name = "Toggle Post Processing";

        public const string Version = "1.0.0";
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
            OriginalVolumes = new List<OriginalVolume>();
            foreach (var volume in UnityEngine.Object.FindObjectsOfType<PostProcessVolume>())
            {
                OriginalVolumes.Add(new OriginalVolume() { postProcessVolume = volume, defaultState = volume.enabled });
            }
        }
        private static void Reset() //Credits to Psychloor for Method
        {
            foreach (OriginalVolume originalVolume in OriginalVolumes)
            {
                originalVolume.postProcessVolume.enabled = originalVolume.defaultState;
            }
        }
        private static void ToggleMethod(bool value)
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
    }
}
