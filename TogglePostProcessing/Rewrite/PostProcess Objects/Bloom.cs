using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Dawn.TPP.PostProcessObjects
{
    internal class Bloom
    {
        internal static Bloom s_Instance;

        internal Bloom(string name)
        {
            if (s_Instance != null) return; // Prevents this from ever being called more than once
            s_Instance = this; // Static Instance Setup;

            gameObject = new GameObject(name);
            m_PostProcessVolume =
                gameObject.AddComponent<PostProcessVolume>(); // Component Toggling is +- 15x faster (See #1)

            m_PostProcessVolume.isGlobal = true;
            m_PostProcessVolume.priority = 10f;
            m_PostProcessProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            m_PostProcessProfile.name = $"{name} Profile";

            m_ColorGrading = ScriptableObject.CreateInstance<ColorGrading>();
            var PostExposure = new FloatParameter {value = 5f};
            m_ColorGrading.postExposure = PostExposure;
            m_Brightness = new FloatParameter {value = 1f};
            //m_ColorGrading.brightness = m_Brightness;
            var ToneMapper = new TonemapperParameter {value = Tonemapper.ACES};
            m_ColorGrading.tonemapper = ToneMapper;

            

            
            m_PostProcessProfile.settings.Add(m_ColorGrading);
            m_PostProcessVolume.sharedProfile = m_PostProcessProfile;
        }

        internal GameObject gameObject;
        private PostProcessProfile m_PostProcessProfile;
        internal PostProcessVolume m_PostProcessVolume;
        private ColorGrading m_ColorGrading;

        internal FloatParameter m_Brightness
        {
            get => m_ColorGrading.brightness;
            set => m_ColorGrading.brightness = value;
        }

        internal bool enabled
        {
            get => m_PostProcessVolume.enabled;
            set => m_PostProcessVolume.enabled = value;
        }

        internal float m_VolumeWeight
        {
            get => m_PostProcessVolume.weight;
            set => m_PostProcessVolume.weight = value;
        }
    }
}


/*
 #1: https://forum.unity.com/threads/which-is-best-for-performance-enable-disable-renderer-or-gameobject.198575 | Comment #3
*/