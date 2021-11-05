using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Dawn.PostProcessing.PostProcessObjects
{
    public class CustomVolume  // Priority Limit: 1223 - 1250
    {
        public CustomVolume(string name, PostProcessProfile profile, float defaultWeight = 0f, bool enabled = true)
        {
            gameObject = new GameObject(name) {layer = 21};
            // Post Processing seems to work on Layers:
            // 4, 21, 29
            gameObject.SetParent(CustomPostProcessing.Base);
            
            m_PostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
            m_PostProcessVolume.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            m_PostProcessVolume.priority = 1223;
            m_PostProcessVolume.isGlobal = true;
            m_PostProcessVolume.tag = name + "_Volume";
            m_PostProcessVolume.weight = defaultWeight;
            m_PostProcessVolume.useGUILayout = true;
            m_PostProcessVolume.enabled = enabled;

            m_PostProcessProfile = profile;
            m_PostProcessProfile.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            m_PostProcessProfile.name = name;
            instances.Add(this);
            Core.Log($"Successfully Created {name}.");
        }

        internal static readonly HashSet<CustomVolume> instances = new();
        public readonly GameObject gameObject;

        public bool enabled
        {
            get
            {
                if (m_PostProcessVolume != null) return m_PostProcessVolume.enabled;
                throw new NullReferenceException("m_PostProcessVolume is null!");
            }
            set
            {
                if (m_PostProcessVolume != null) m_PostProcessVolume.enabled = value;
                else throw new NullReferenceException("m_PostProcessVolume is null!");
                
            }
        }
        public PostProcessVolume m_PostProcessVolume { get; }
        public PostProcessProfile m_PostProcessProfile
        {
            get
            {
                if (m_PostProcessVolume != null)
                {
                   return m_PostProcessVolume.sharedProfile;
                }
                throw new NullReferenceException("m_PostProcessVolume");
            }
            private set
            {
                if (m_PostProcessVolume != null)
                {
                    m_PostProcessVolume.profile = value;
                    m_PostProcessVolume.sharedProfile = value;
                }
                else
                {
                    throw new NullReferenceException("m_PostProcessVolume");
                }
            }
        }
    }
    }


/*
 #1: https://forum.unity.com/threads/which-is-best-for-performance-enable-disable-renderer-or-gameobject.198575 | Comment #3
*/