using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
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

        private static IEnumerator GrabWorldVolumes() //Credits to Psychloor for Method
        {
            OriginalVolumes = new List<OriginalVolume>();
            foreach (var volume in Resources.FindObjectsOfTypeAll<PostProcessVolume>())
            {
                OriginalVolumes.Add(new OriginalVolume { postProcessVolume = volume, defaultState = volume.enabled });
                yield return new OriginalVolume();
            }
            m_PreviousState = false; // Reset on World Join.
        }

        internal static void WorldJoin()
        {
            MelonCoroutines.Start(GrabWorldVolumes());
            ToggleWorldVolumes();
        }
        private static bool m_PreviousState;
        internal static void ToggleWorldVolumes()
        {
            try
            {
                if (!WorldPostProcessing && m_PreviousState != !WorldPostProcessing)
                {
                    if (OriginalVolumes == null) return;
                    foreach (var originalVolume in OriginalVolumes.Where(originalVolume => originalVolume.postProcessVolume))
                    {
                        originalVolume.postProcessVolume.enabled = false;
                    }
                    m_PreviousState = !WorldPostProcessing;
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