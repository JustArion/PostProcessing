#if QM
using MelonLoader;
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
        private static bool QMInit;

        internal static void InitQM()
        {
            
            TPPQM = new QMToggleButton("ShortcutMenu", 1.24f, 1.75f, "Post\nProcessing", () =>
            {
                if (WorldVolumes.WorldQMToggle)
                {
                    MelonPreferences.SetEntryValue(Core.ModID, "WorldPostProcessing", true);
                    WorldVolumes.WorldPostProcessing = true;
                    WorldVolumes.ToggleWorldVolumes();
                    return;
                }
                MelonPreferences.SetEntryValue(Core.ModID, "PostProcessing", true);
                Core.s_PostProcessing = true;
                Core.LayerChange().Coroutine();
            }, "OFF", () =>
            {
                if (WorldVolumes.WorldQMToggle)
                {
                    MelonPreferences.SetEntryValue(Core.ModID, "WorldPostProcessing", false);
                    WorldVolumes.WorldPostProcessing = false;
                    WorldVolumes.ToggleWorldVolumes();
                    return;
                }
                MelonPreferences.SetEntryValue(Core.ModID, "PostProcessing", false);
                Core.s_PostProcessing = false;
                Core.LayerChange().Coroutine();
            }, "Post Processing", Color.black);
            
            TPPQM.btnOff.SetSizeButtonfor(QMX, QMY);
            TPPQM.btnOn.SetSizeButtonfor(QMX, QMY);
            TPPQM.getGameObject().SetSizeButtonfor(1.9f, 1.8f);
            TPPQM.getGameObject().GetComponent<Image>().enabled = false;
            TPPQM.getGameObject().AddComponent<BoxCollider>();
            TPPQM.getGameObject().AddComponent<VRC_UiShape>();
            TPPQM.getGameObject().AddComponent<GraphicRaycaster>();
            TPPQM.btnOn.transform.Find("Text_ON").GetComponent<RectTransform>().localPosition = new Vector3(0, 65, 0);
            TPPQM.btnOn.transform.Find("Text_ON").GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
            TPPQM.btnOff.transform.Find("Text_ON").GetComponent<RectTransform>().localPosition = new Vector3(0, 65, 0);
            TPPQM.btnOff.transform.Find("Text_ON").GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
        
            TPPQM.getGameObject().GetComponent<RectTransform>().localScale -= new Vector3(0, 0.2f, 0);
            TPPQM.getGameObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(-107.2f, -1340.8f);
            TPPQM.setToggleState(Core.s_PostProcessing);
            TPPQM.setActive(Core.s_QuickMenu);
            TPPQM.getGameObject().name = "Post-Processing";
            QMInit = true;
        }
        
        internal static void QMPrefsRefresh()
        {
            if (!QMInit) return;
            if (!Core.s_UICreated) return;
            TPPQM.setActive(Core.s_QuickMenu);
            if (WorldVolumes.WorldQMToggle)
            {
                TPPQM.setToggleState(WorldVolumes.WorldPostProcessing);
                return;
            }
            TPPQM.setToggleState(Core.s_PostProcessing);
        }
    }
}
#endif