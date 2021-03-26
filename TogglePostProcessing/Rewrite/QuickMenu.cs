using MelonLoader;
using RubyButtonAPI;
using UnityEngine;
using UnityEngine.UI;
using VRCSDK2;

namespace Dawn.TPP
{
    internal static class QuickMenu
    {
        private const float QMX = 2.7f;
        private const float QMY = 1.8f;
        
        private static QMToggleButton TPPQM;

        internal static void InitQM()
        {
            TPPQM = new QMToggleButton("ShortcutMenu", 1.24f, 1.75f, "TPP", () =>
            {
                MelonPreferences.SetEntryValue("TogglePostProcessing", "DisablePostProcessing", false);
                Core.m_PostProcessing = true;
                Core.LayerChange();
            }, "OFF", () =>
            {
                MelonPreferences.SetEntryValue("TogglePostProcessing", "DisablePostProcessing", true);
                Core.m_PostProcessing = false;
                Core.LayerChange();
            }, "Toggle Post Processing");
            TPPQM.btnOff.SetSizeButtonfor(QMX, QMY);
            TPPQM.btnOn.SetSizeButtonfor(QMX, QMY);
            TPPQM.getGameObject().SetSizeButtonfor(1.9f, 1.8f);
            TPPQM.getGameObject().GetComponent<Image>().enabled = false;
            TPPQM.getGameObject().AddComponent<BoxCollider>();
            TPPQM.getGameObject().AddComponent<VRC_UiShape>();
            TPPQM.getGameObject().AddComponent<GraphicRaycaster>();
            TPPQM.setToggleState(Core.m_PostProcessing);
            TPPQM.setActive(Core.m_QuickMenu);
            TPPQM.getGameObject().name = "[Mod] TPP Toggle";
        }

        internal static void QMPrefsRefresh()
        {
            TPPQM.setActive(Core.m_QuickMenu);
            TPPQM.setToggleState(Core.m_PostProcessing);
        }
    }
}