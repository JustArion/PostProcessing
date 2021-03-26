using System;
using MelonLoader;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using System.Diagnostics;
using RubyButtonAPI;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using MelonLoader.TinyJSON;
using Newtonsoft.Json;
using TogglePostProcessing.DawnRefs;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using VRCSDK2;

using static MelonLoader.MelonLogger;
using Bloom = Dawn.TPP.PostProcessObjects.Bloom;
using Object = UnityEngine.Object;

namespace Dawn.TPP
{
    internal sealed class BuildInfo
    {
        internal const string Author = "arion#1223";
        internal const string Company = null;
        internal const string DownloadLink = "https://github.com/Arion-Kun/TogglePostProcessing/releases/";

        internal const string Name = "TogglePostProcessing";

        internal const string Version = "1.2.0";
    }
    internal sealed class Start : MelonMod
    {
        public override void OnApplicationStart()
        {
            Core.RegisterSettings();
            Msg("Settings can be configured in UserData/modprefs.ini or through UIExpansionKit");
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
            Core.LayerChange();
            
            CustomPostProcessing.WorldJoin();

            // Add extra Post Processing here | Bloom / Dark Mode

        }

        public override void VRChat_OnUiManagerInit()
        { Core.m_UICreated = true;
        #if QM
            QuickMenu.InitQM();
        #endif
            
            
            CustomPostProcessing.CreateProcessingObjects();

        }

        public override void OnPreferencesSaved()
        {
            Core.InternalSettingsRefresh(); 
        #if QM
            QuickMenu.QMPrefsRefresh();
        #endif
        }

        public static PostProcessProfile USEMETEMPDEBUG;
    }
}