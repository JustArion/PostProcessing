using MelonLoader;
using RubyButtonAPI;
using UnityEngine;
using UnityEngine.UI;
using VRCSDK2;

namespace Dawn.PostProcessing
{
    internal static class QuickMenus
    {
        private const float QMX = 2.7f;
        private const float QMY = 1.8f;
        
        private static QMToggleButton TPPQM;

        internal static void InitQM()
        {
            TPPQM = new QMToggleButton("ShortcutMenu", 1.24f, 1.75f, "PP", () =>
            {
                if (WorldVolumes.WorldQMToggle)
                {
                    MelonPreferences.SetEntryValue(Core.ModID, "WorldPostProcessing", true);
                    WorldVolumes.WorldPostProcessing = true;
                    WorldVolumes.ToggleWorldVolumes();
                    return;
                }
                MelonPreferences.SetEntryValue(Core.ModID, "Enable PostProcessing", true);
                Core.s_PostProcessing = true;
                Core.LayerChange();
            }, "OFF", () =>
            {
                if (WorldVolumes.WorldQMToggle)
                {
                    MelonPreferences.SetEntryValue(Core.ModID, "WorldPostProcessing", false);
                    WorldVolumes.WorldPostProcessing = false;
                    WorldVolumes.ToggleWorldVolumes();
                    return;
                }
                MelonPreferences.SetEntryValue(Core.ModID, "Enable PostProcessing", false);
                Core.s_PostProcessing = false;
                Core.LayerChange();
            }, "PostProcessing");
            TPPQM.btnOff.SetSizeButtonfor(QMX, QMY);
            TPPQM.btnOn.SetSizeButtonfor(QMX, QMY);
            TPPQM.getGameObject().SetSizeButtonfor(1.9f, 1.8f);
            TPPQM.getGameObject().GetComponent<Image>().enabled = false;
            TPPQM.getGameObject().AddComponent<BoxCollider>();
            TPPQM.getGameObject().AddComponent<VRC_UiShape>();
            TPPQM.getGameObject().AddComponent<GraphicRaycaster>();
            TPPQM.setToggleState(Core.s_PostProcessing);
            TPPQM.setActive(Core.s_QuickMenu);
            TPPQM.getGameObject().name = "[Mod] Post-Processing";
        }

        internal static void QMPrefsRefresh()
        {
            TPPQM.setActive(Core.s_QuickMenu);
            TPPQM.setToggleState(Core.s_PostProcessing);
        }
    }
}