using System;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace TogglePostProcessing.DawnRefs
{
    public static class Refs
    {
        private static QuickMenu QuickMenuInstanceCache;

        internal static QuickMenu instance
        {
            get
            {
                if (QuickMenuInstanceCache != null) return QuickMenuInstanceCache;
                QuickMenuInstanceCache = FindInstance(typeof(QuickMenu), typeof(QuickMenu))?.TryCast<QuickMenu>();
                return QuickMenuInstanceCache;
            }
        }
        /// <summary>
        /// Current User Instance Cache.
        /// </summary>
        private static VRCPlayer CurrentUserInstance;
        /// <summary>
        /// Returns the Current User aka the Player object.
        /// </summary>
        internal static VRCPlayer CurrentUser
        {
            get
            {
                if (CurrentUserInstance != null) return CurrentUserInstance;
                CurrentUserInstance = FindInstance(typeof(VRCPlayer), typeof(VRCPlayer))?.TryCast<VRCPlayer>();
                return CurrentUserInstance;
                
            }
        }
        private static Il2CppSystem.Object FindInstance(IReflect WhereLooking, Type WhatLooking) // Credits to Teo
        {
            try
            {
                var methodInfo = WhereLooking.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => m.ReturnType == WhatLooking && m.GetParameters().Length == 0);
                if (methodInfo != null)
                {
                    return (Il2CppSystem.Object)methodInfo.Invoke(null, null);
                }
                MelonLogger.Error("[FindInstance] MethodInfo for " + WhatLooking.Name + " is null");
            }
            catch (Exception e)
            {
                MelonLogger.Error($"[FindInstance] {e}");
            }
            return null;
        }
    }
}