using System;
using System.Linq;

namespace TogglePostProcessing.DawnRefs
{
    public static class Refs
    {
        private static QuickMenuDelegate QuickMenuInstance;

        public delegate QuickMenu QuickMenuDelegate();

        internal static QuickMenuDelegate QuickMenu
        {
            get
            {
                if (QuickMenuInstance != null) return QuickMenuInstance;
                var MethodInfo = typeof(QuickMenu).GetMethods().First(x => x.ReturnType == typeof(QuickMenu));
                QuickMenuInstance = (QuickMenuDelegate) Delegate.CreateDelegate(typeof(QuickMenuDelegate), MethodInfo);
                return QuickMenuInstance;
            }
        }
    }
}