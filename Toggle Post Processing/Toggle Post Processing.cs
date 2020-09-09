using System.Linq;
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
        public bool ToggleHandler;
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("TogglePostProcessing", "Toggle Post Processing");
            MelonPrefs.RegisterBool("TogglePostProcessing", "DisablePostProcessing", false, "Disable Post Processing");
            ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
        }
        public override void OnLevelWasLoaded(int level)
        {
            ToggleMethod(ToggleHandler);
        }
        public override void OnModSettingsApplied()
        {
            ToggleHandler = MelonPrefs.GetBool("TogglePostProcessing", "DisablePostProcessing");
            ToggleMethod(ToggleHandler);
        }
        private static void ToggleMethod(bool value)
        {
            PostProcessVolume[] PostProcessingObjects = UnityEngine.Object.FindObjectsOfType<PostProcessVolume>().ToArray();
            foreach (PostProcessVolume List in PostProcessingObjects)
            {
                List.GetComponent<PostProcessVolume>().enabled = !value;
            }
        }
    }
}
