using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Dawn.PostProcessing
{
    internal static class WorldVolumes
    {
        internal static bool WorldPostProcessing;
        internal static bool WorldQMToggle;
        private static List<OriginalVolume> OriginalVolumes;
        
        private struct OriginalVolume // Credits to Psychloor for this Big Brain Snippet.
        {
            internal PostProcessVolume postProcessVolume;
            internal bool defaultState;
            
        }

        private static void GrabWorldVolumes() //Credits to Psychloor for Method
        {
            OriginalVolumes = new List<OriginalVolume>();
            foreach (var volume in Resources.FindObjectsOfTypeAll<PostProcessVolume>().Where(p => p.priority is < 1223 or > 1250 ))
            {
                OriginalVolumes.Add(new OriginalVolume { postProcessVolume = volume, defaultState = volume.enabled });
            }
        }

        internal static void WorldJoin()
        {
            GrabWorldVolumes();
            ToggleWorldVolumes();
        }
        internal static void ToggleWorldVolumes()
        {
            try
            {
                if (OriginalVolumes == null || !Core.IsInWorld) return;
                if (!WorldPostProcessing)
                {
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
                Core.Log($"ToggleMethod Error: {e}", Core.LogType.Error);
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
                Core.Log($"Reset Error: {e}", Core.LogType.Error); 
            }
        }
    }
}