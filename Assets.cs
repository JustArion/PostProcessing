using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngineInternal;
using Object = UnityEngine.Object;
using Stream = Il2CppSystem.IO.Stream;

namespace Dawn.PostProcessing
{

    internal static class Assets
    {
        internal static IEnumerator LoadAssets()
        {
            var resourceStream = (byte[]) PostProcessAssets.ResourceManager.GetObject("dawn");
            if (resourceStream != null)
            {
                
                var x = AssetBundle.LoadFromMemoryAsync_Internal(resourceStream, 0);

                m_AssetBundle = x.assetBundle;

                Core.Log(
                    m_AssetBundle == x.assetBundle
                        ? "Asset Resource Stream Successfully Generated a Functional AssetBundle."
                        : "Asset Resource Stream Failed to Generate a Functional AssetBundle.",
                    m_AssetBundle == x.assetBundle ? Core.LogType.Log : Core.LogType.Error);

                if (m_AssetBundle == null)
                {
                    Core.Log("Asset Bundle is null!", Core.LogType.Error);
                    yield break;
                }
                m_AssetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                
                GenerateInstances();
            }
            else
            {
                Core.Log(
                    "Asset Resource Stream is null! Please Report this in the #bug-report channel in the VRC Modding Group.",
                    Core.LogType.Error);
            }
        }

        /*private static IEnumerator GenerateInstancesAsync() // Unused
        {
            var abR = m_AssetBundle.LoadAssetAsync_Internal("Assets/PostProcessResources.asset", Il2CppType.Of<PostProcessResources>());
            yield return abR;
            var abR2 = m_AssetBundle.LoadAssetAsync_Internal("Assets/Bloom_Profile.asset", Il2CppType.Of<PostProcessProfile>());
            yield return abR2;
            var abR3 = m_AssetBundle.LoadAssetAsync_Internal("Assets/DSC.asset", Il2CppType.Of<PostProcessProfile>());
            yield return abR3;
            
            CustomPostProcessing.CachedResources = abR.asset as PostProcessResources;
            tmp_Bloom = abR2.asset as PostProcessProfile;
            tmp_DSC = abR3.asset as PostProcessProfile;
        }*/
        private static void GenerateInstances()
        {
            try 
            {
                foreach (var obj in m_AssetBundle.LoadAllAssets()) //tmp prefix as it will auto-unload from memory soon.
                {
                    switch (obj.name)
                    {
                        case "Bloom":
                            tmp_Bloom = obj;
                            continue;
                        case "DarkMode":
                            tmp_Dark = obj;
                            break;
                        case "Contrast":
                            tmp_Contrast = obj;
                            break;
                        case "Saturation":
                            tmp_Saturation = obj;
                            break;
                        case "Temperature":
                            tmp_Temperature = obj;
                            break;
                        case "PostProcessResources":
                            tmp_CachedResources = obj;
                            break;
                    }
                }

                CustomPostProcessing.m_Bloom = tmp_Bloom.TryCast<PostProcessProfile>();
                CustomPostProcessing.m_Bloom.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                CustomPostProcessing.m_DarkMode = tmp_Dark.TryCast<PostProcessProfile>();
                CustomPostProcessing.m_DarkMode.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                CustomPostProcessing.m_Contrast = tmp_Contrast.TryCast<PostProcessProfile>();
                CustomPostProcessing.m_Contrast.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                CustomPostProcessing.m_Saturation = tmp_Saturation.TryCast<PostProcessProfile>();
                CustomPostProcessing.m_Saturation.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                CustomPostProcessing.m_Temperature = tmp_Temperature.TryCast<PostProcessProfile>();
                CustomPostProcessing.m_Temperature.hideFlags |= HideFlags.DontUnloadUnusedAsset;


                CustomPostProcessing.CachedResources = tmp_CachedResources.TryCast<PostProcessResources>();
                CustomPostProcessing.CachedResources.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            catch (Exception e)
            {
                const string inn = "is not null.";
                const string n = "is null.";
                
                Core.Log("Manual", Core.LogType.Error);
                Core.Log(tmp_CachedResources != null ? $"m_Resources {inn}" : $"m_Resources {n}" , Core.LogType.Error);
                Core.Log(tmp_Bloom != null ? $"m_Bloom {inn}" : $"m_Bloom {n}" , Core.LogType.Error);
                Core.Log(tmp_Dark != null ? $"m_DSC {inn}" : $"m_DSC {n}" , Core.LogType.Error);
                Core.Log(e, Core.LogType.Error);
            }
        }
        
        private static AssetBundle m_AssetBundle;

        private static Object tmp_CachedResources;
        private static Object tmp_Bloom;
        private static Object tmp_Dark;
        private static Object tmp_Saturation;
        private static Object tmp_Contrast;
        private static Object tmp_Temperature;
        
    }
}