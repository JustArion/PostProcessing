using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Dawn.PostProcessing.PostProcessObjects
{
    internal class CustomVolume : CustomPostProcessing // Priority Limit: 1223 - 1250
    {
        internal CustomVolume(string name, PostProcessProfile profile, float defaultWeight = 0f, bool enabled = true)
        {
            gameObject = new GameObject(name) {layer = 21};
            // Post Processing seems to work on Layers:
            // 4, 21, 29
            gameObject.SetParent(Base);
            
            m_PostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
            m_PostProcessVolume.priority = 1223;
            m_PostProcessVolume.isGlobal = true;
            m_PostProcessVolume.tag = name + "_Volume";
            m_PostProcessVolume.weight = defaultWeight;
            m_PostProcessVolume.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            m_PostProcessVolume.useGUILayout = true;
            m_PostProcessVolume.enabled = enabled;

            m_PostProcessProfile = profile;
            m_PostProcessProfile.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            m_PostProcessProfile.name = name;
            instances.Add(this);
            Core.Log($"Successfully Created {name}.");
        }

        internal static readonly List<CustomVolume> instances = new();
        internal readonly GameObject gameObject;

        internal bool enabled
        {
            get => m_PostProcessVolume.enabled;
            set => m_PostProcessVolume.enabled = value;
        }
        internal PostProcessVolume m_PostProcessVolume { get; }
        internal PostProcessProfile m_PostProcessProfile
        {
            get => m_PostProcessVolume.sharedProfile;
            private set
            {
                m_PostProcessVolume.profile = value;
                m_PostProcessVolume.sharedProfile = value;
            }
        }
    }
    }


/*
 #1: https://forum.unity.com/threads/which-is-best-for-performance-enable-disable-renderer-or-gameobject.198575 | Comment #3
*/