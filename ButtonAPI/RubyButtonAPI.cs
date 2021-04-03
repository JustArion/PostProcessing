using System;
using Il2CppSystem.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnhollowerRuntimeLib;
using Button = UnityEngine.UI.Button;

namespace RubyButtonAPI
{
    public static class QMButtonAPI
    {
    	//REPLACE THIS STRING SO YOUR MENU DOESNT COLLIDE WITH OTHER MENUS
    	public static string identifier = "PostProcessing";
        public static Color mBackground = Color.red;
        public static Color mForeground = Color.white;
        public static Color bBackground = Color.red;
        public static Color bForeground = Color.yellow;
        public static List<QMSingleButton> allSingleButtons = new List<QMSingleButton>();
        public static List<QMToggleButton> allToggleButtons = new List<QMToggleButton>();
        public static List<QMNestedButton> allNestedButtons = new List<QMNestedButton>();
    }
    public static class QM
    {
        //Added by Arion
        public static void SetSizeButtonfor(this GameObject Button, float xSize, float ySize)
        {
            Button.GetComponent<RectTransform>().sizeDelta /= new Vector2(xSize, ySize);
        }
    }

    public class QMButtonBase
    {
        protected GameObject button;
        protected string btnQMLoc;
        protected string btnType;
        protected string btnTag;
        protected readonly int[] initShift = { 0, 0 };
        protected Color OrigBackground;
        protected Color OrigText;

        public GameObject getGameObject()
        {
            return button;
        }

        public void setActive(bool isActive)
        {
            button.gameObject.SetActive(isActive);
        }

        public void setIntractable(bool isIntractable)
        {
            if (isIntractable)
            {
                setBackgroundColor(OrigBackground, false);
                setTextColor(OrigText, false);
            }
            else
            {
                setBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1), false);
                setTextColor(new Color(0.7f, 0.7f, 0.7f, 1), false); ;
            }
            button.gameObject.GetComponent<Button>().interactable = isIntractable;
        }

        public void setLocation(float buttonXLoc, float buttonYLoc)
        {
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.right * (420 * (buttonXLoc + initShift[0]));
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.down * (420 * (buttonYLoc + initShift[1]));

            btnTag = "(" + buttonXLoc + "," + buttonYLoc + ")";
            button.name = btnQMLoc + "/" + btnType + btnTag;
            button.GetComponent<Button>().name = btnType + btnTag;
        }

        public void setToolTip(string buttonToolTip)
        {
            button.GetComponent<UiTooltip>().field_Public_String_0 = buttonToolTip;
            button.GetComponent<UiTooltip>().field_Public_String_1 = buttonToolTip;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(button);
            }
            catch { }
        }

        public void SetParent(QMNestedButton Parent)
        {
            button.transform.SetParent(QuickMenu.prop_QuickMenu_0.transform.Find(Parent.getMenuName()));
        }
        public void SetParent(Transform Parent)
        {
            button.transform.SetParent(Parent);
        }

        public virtual void setBackgroundColor(Color buttonBackgroundColor, bool save = true) { }
        public virtual void setTextColor(Color buttonTextColor, bool save = true) { }
        public virtual void setTextColorOn(Color buttonTextColorOn, bool save = true) { }
        public virtual void setTextColorOff(Color buttonTextColorOff, bool save = true) { }
    }

    public class QMSingleButton : QMButtonBase
    {
        public QMSingleButton(QMNestedButton btnMenu, int btnXLocation, int btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            initButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        public QMSingleButton(string btnMenu, int btnXLocation, int btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null)
        {
            btnQMLoc = btnMenu;
            initButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        private void initButton(int btnXLocation, int btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null)
        {
            btnType = "SingleButton";
            button = UnityEngine.Object.Instantiate(QuickMenuStuff.SingleButtonTemplate(), QuickMenuStuff.GetQuickMenuInstance().transform.Find(btnQMLoc), true);

            initShift[0] = -1;
            initShift[1] = 0;
            setLocation(btnXLocation, btnYLocation);
            setButtonText(btnText);
            setToolTip(btnToolTip);
            setAction(btnAction);


            if (btnBackgroundColor != null)
                setBackgroundColor((Color)btnBackgroundColor);
            else
                OrigBackground = button.GetComponentInChildren<Image>().color;

            if (btnTextColor != null)
                setTextColor((Color)btnTextColor);
            else
                OrigText = button.GetComponentInChildren<Text>().color;

            setActive(true);
            QMButtonAPI.allSingleButtons.Add(this);
        }

        public void setButtonText(string buttonText)
        {
            button.GetComponentInChildren<Text>().text = buttonText;
        }

        public void setAction(Action buttonAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            if (buttonAction != null)
                button.GetComponent<Button>().onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(buttonAction));
        }

        public override void setBackgroundColor(Color buttonBackgroundColor, bool save = true)
        {
            //button.GetComponentInChildren<Image>().color = buttonBackgroundColor;
            if (save)
                OrigBackground = (Color)buttonBackgroundColor;
            //Image[] btnBgColorList = ((btnOn.GetComponentsInChildren<Image>()).Concat(btnOff.GetComponentsInChildren<Image>()).ToArray()).Concat(button.GetComponentsInChildren<Image>()).ToArray();
            //foreach (Image btnBackground in btnBgColorList) btnBackground.color = buttonBackgroundColor;
            button.GetComponentInChildren<UnityEngine.UI.Button>().colors = new ColorBlock()
            {
                colorMultiplier = 1f,
                disabledColor = Color.grey,
                highlightedColor = buttonBackgroundColor * 1.5f,
                normalColor = buttonBackgroundColor / 1.5f,
                pressedColor = Color.grey * 1.5f
            };
        }

        public override void setTextColor(Color buttonTextColor, bool save = true)
        {
            button.GetComponentInChildren<Text>().color = buttonTextColor;
            if (save)
                OrigText = (Color)buttonTextColor;
        }
    }

    public class QMToggleButton : QMButtonBase
    {
        public GameObject btnOn;
        public GameObject btnOff;
        private readonly List<QMButtonBase> showWhenOn = new List<QMButtonBase>();
        private readonly List<QMButtonBase> hideWhenOn = new List<QMButtonBase>();

        Action btnOnAction = null;
        Action btnOffAction = null;

        public QMToggleButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, string btnTextOn, Action btnActionOn, string btnTextOff, Action btnActionOff, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColorOn = null, Color? btnTextColorOff = null, bool defaultPosition = false)
        {
            btnQMLoc = btnMenu.getMenuName();
            initButton(btnXLocation, btnYLocation, btnTextOn, btnActionOn, btnTextOff, btnActionOff, btnToolTip, btnBackgroundColor, btnTextColorOn, btnTextColorOff, defaultPosition);
        }

        public QMToggleButton(string btnMenu, float btnXLocation, float btnYLocation, string btnTextOn, Action btnActionOn, string btnTextOff, Action btnActionOff, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColorOn = null, Color? btnTextColorOff = null, bool defaultPosition = false)
        {
            btnQMLoc = btnMenu;
            initButton(btnXLocation, btnYLocation, btnTextOn, btnActionOn, btnTextOff, btnActionOff, btnToolTip, btnBackgroundColor, btnTextColorOn, btnTextColorOff, defaultPosition);
        }

        private void initButton(float btnXLocation, float btnYLocation, string btnTextOn, Action btnActionOn, string btnTextOff, Action btnActionOff, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColorOn = null, Color? btnTextColorOff = null, bool defaultPosition = false)
        {
            btnType = "ToggleButton";
            button = UnityEngine.Object.Instantiate<GameObject>(QuickMenuStuff.ToggleButtonTemplate(), QuickMenuStuff.GetQuickMenuInstance().transform.Find(btnQMLoc), true);

            btnOn = button.transform.Find("Toggle_States_Visible/ON").gameObject;
            btnOff = button.transform.Find("Toggle_States_Visible/OFF").gameObject;
            
            initShift[0] = -4;
            initShift[1] = 0;

            setLocation(btnXLocation, btnYLocation);

            setOnText(btnTextOn);
            setOffText(btnTextOff);
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[0].name = "Text_ON";
            btnTextsOn[0].alignment = (TextAnchor)7; //Had to make some changes here to allow the text to align properly.
            btnTextsOn[0].alignByGeometry = true; //
            btnTextsOn[0].resizeTextForBestFit = true;
            btnTextsOn[1].name = "Text_OFF";
            btnTextsOn[1].alignment = (TextAnchor)1;
            btnTextsOn[1].alignByGeometry = true;
            btnTextsOn[1].resizeTextForBestFit = true;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[0].name = "Text_ON";
            btnTextsOff[0].alignment = (TextAnchor)7;
            btnTextsOff[0].alignByGeometry = true;
            btnTextsOff[0].resizeTextForBestFit = true;
            btnTextsOff[1].name = "Text_OFF";
            btnTextsOff[1].alignment = (TextAnchor)1;
            btnTextsOff[1].alignByGeometry = true;
            btnTextsOff[1].resizeTextForBestFit = true;

            setToolTip(btnToolTip);
            //button.transform.GetComponentInChildren<UiTooltip>().SetToolTipBasedOnToggle();

            setAction(btnActionOn, btnActionOff);
            btnOn.SetActive(false);
            btnOff.SetActive(true);

            if (btnBackgroundColor != null)
                setBackgroundColor((Color)btnBackgroundColor);
            else
                OrigBackground = btnOn.GetComponentsInChildren<Text>().First().color;

            if (btnTextColorOn != null)
                setTextColor((Color)btnTextColorOn);
            else
                OrigText = btnOn.GetComponentsInChildren<Image>().First().color;

            if (btnTextColorOff != null)
                setTextColor((Color)btnTextColorOff);
            else
                OrigText = btnOn.GetComponentsInChildren<Image>().First().color;

        }

        public override void setBackgroundColor(Color buttonBackgroundColor, bool save = true)
        {
            Image[] btnBgColorList = ((btnOn.GetComponentsInChildren<Image>()).Concat(btnOff.GetComponentsInChildren<Image>()).ToArray()).Concat(button.GetComponentsInChildren<Image>()).ToArray();
            foreach (Image btnBackground in btnBgColorList) btnBackground.color = buttonBackgroundColor;
            if (save)
                OrigBackground = (Color)buttonBackgroundColor;
        }

        public override void setTextColorOn(Color buttonTextColorOn, bool save = true)
        {
            Text[] btnTxtColorOnList = (btnOn.GetComponentsInChildren<Text>()).ToArray();
            foreach (Text btnText in btnTxtColorOnList) btnText.color = buttonTextColorOn;
            if (save)
                OrigText = (Color)buttonTextColorOn;
        }
        public override void setTextColorOff(Color buttonTextColorOff, bool save = true)
        {
            Text[] btnTxtColorOffList = (btnOff.GetComponentsInChildren<Text>()).ToArray();
            foreach (Text btnText in btnTxtColorOffList) btnText.color = buttonTextColorOff;
            if (save)
                OrigText = (Color)buttonTextColorOff;
        }

        public void setAction(Action buttonOnAction, Action buttonOffAction)
        {
            btnOnAction = buttonOnAction;
            btnOffAction = buttonOffAction;

            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            button.GetComponent<Button>().onClick.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityAction>((Action)(() =>
          {
              if (btnOn.activeSelf)
              {
                  setToggleState(false, true);
              }
              else
              {
                  setToggleState(true, true);
              }
          })));
        }


        public void setToggleState(bool toggleOn, bool shouldInvoke = false)
        {
            btnOn.SetActive(toggleOn);
            btnOff.SetActive(!toggleOn);
            try
            {
                if (toggleOn && shouldInvoke)
                {
                    btnOnAction.Invoke();
                    showWhenOn.ForEach(x => x.setActive(true));
                    hideWhenOn.ForEach(x => x.setActive(false));
                }
                else if (!toggleOn && shouldInvoke)
                {
                    btnOffAction.Invoke();
                    showWhenOn.ForEach(x => x.setActive(false));
                    hideWhenOn.ForEach(x => x.setActive(true));
                }
            }
            catch { }

        }

        public string getOnText()
        {
            return btnOn.GetComponentsInChildren<Text>()[0].text;
        }

        public void setOnText(string buttonOnText)
        {
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[0].text = buttonOnText;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[0].text = buttonOnText;
        }

        public void setOffText(string buttonOffText)
        {
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[1].text = buttonOffText;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[1].text = buttonOffText;
        }

    }

    public class QMNestedButton
    {
        protected QMSingleButton mainButton;
        protected QMSingleButton backButton;
        protected string menuName;
        protected readonly string btnQMLoc;
        protected string btnType;

        public QMNestedButton(QMNestedButton btnMenu, int btnXLocation, int btnYLocation, string btnText, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, Color? backbtnBackgroundColor = null, Color? backbtnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            initButton(btnXLocation, btnYLocation, btnText, btnToolTip, btnBackgroundColor, btnTextColor, backbtnBackgroundColor, backbtnTextColor);
        }

        public QMNestedButton(string btnMenu, int btnXLocation, int btnYLocation, string btnText, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, Color? backbtnBackgroundColor = null, Color? backbtnTextColor = null)
        {
            btnQMLoc = btnMenu;
            initButton(btnXLocation, btnYLocation, btnText, btnToolTip, btnBackgroundColor, btnTextColor, backbtnBackgroundColor, backbtnTextColor);
        }

        public void initButton(int btnXLocation, int btnYLocation, string btnText, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, Color? backbtnBackgroundColor = null, Color? backbtnTextColor = null)
        {
            btnType = "NestedButton";

            var menu = UnityEngine.Object.Instantiate<Transform>(QuickMenuStuff.NestedMenuTemplate(), QuickMenuStuff.GetQuickMenuInstance().transform);
            menuName = QMButtonAPI.identifier + btnQMLoc + "_" + btnXLocation + "_" + btnYLocation;
            menu.name = menuName;

            mainButton = new QMSingleButton(btnQMLoc, btnXLocation, btnYLocation, btnText, () => { QuickMenuStuff.ShowQuickmenuPage(menuName); }, btnToolTip, btnBackgroundColor, btnTextColor);

            var enumerator = menu.transform.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                var btnEnum = obj.Cast<Transform>();
                if (btnEnum != null)
                {
                    UnityEngine.Object.Destroy(btnEnum.gameObject);
                }
            }

            backbtnTextColor ??= Color.yellow;
            QMButtonAPI.allNestedButtons.Add(this);
            backButton = new QMSingleButton(this, 5, 2, "Back", () => { QuickMenuStuff.ShowQuickmenuPage(btnQMLoc); }, "Go Back", backbtnBackgroundColor, backbtnTextColor);
        }

        public string getMenuName()
        {
            return menuName;
        }

        public QMSingleButton getMainButton()
        {
            return mainButton;
        }

        public QMSingleButton getBackButton()
        {
            return backButton;
        }

        public void DestroyMe()
        {
            mainButton.DestroyMe();
            backButton.DestroyMe();
        }
    }
    public class QuickMenuStuff
    {
        // Internal cache of the BoxCollider Background for the Quick Menu
        private static BoxCollider QuickMenuBackgroundReference;

        // Internal cache of the Single Button Template for the Quick Menu
        private static GameObject SingleButtonReference;

        // Internal cache of the Toggle Button Template for the Quick Menu
        private static GameObject ToggleButtonReference;

        // Internal cache of the Nested Menu Template for the Quick Menu
        private static Transform NestedButtonReference;

        // Internal cache of the QuickMenu
        private static QuickMenu quickmenuInstance;




        // Fetch the background from the Quick Menu
        public static BoxCollider QuickMenuBackground()
        {
            if (QuickMenuBackgroundReference == null)
                QuickMenuBackgroundReference = GetQuickMenuInstance().GetComponent<BoxCollider>();
            return QuickMenuBackgroundReference;
        }

        // Fetch the Single Button Template from the Quick Menu
        public static GameObject SingleButtonTemplate()
        {
            if (SingleButtonReference == null)
                SingleButtonReference = GetQuickMenuInstance().transform.Find("ShortcutMenu/WorldsButton").gameObject;
            return SingleButtonReference;
        }

        // Fetch the Toggle Button Template from the Quick Menu
        public static GameObject ToggleButtonTemplate()
        {
            if (ToggleButtonReference == null)
            {
                ToggleButtonReference = GetQuickMenuInstance().transform.Find("UserInteractMenu/BlockButton").gameObject;
            }
            return ToggleButtonReference;
        }

        // Fetch the Nested Menu Template from the Quick Menu
        public static Transform NestedMenuTemplate()
        {
            if (NestedButtonReference == null)
            {
                NestedButtonReference = GetQuickMenuInstance().transform.Find("CameraMenu");
            }
            return NestedButtonReference;
        }

        // Fetch the Quick Menu instance
        public static QuickMenu GetQuickMenuInstance()
        {
            return quickmenuInstance ??= Dawn.PostProcessing.Core.instance;
        }

        // Cache the FieldInfo for getting the current page. Hope to god this works!
        private static FieldInfo currentPageGetter;

        // Show a Quick Menu page via the Page Name. Hope to god this works!
        public static void ShowQuickmenuPage(string pagename)
        {
            var quickmenu = GetQuickMenuInstance();
            var pageTransform = quickmenu.transform.Find(pagename);
            if (pageTransform == null)
            {
                Console.WriteLine("[QMStuff] pageTransform is null !");
            }

            if (currentPageGetter == null)
            {
                var shortcutMenu = quickmenu.transform.Find("ShortcutMenu").gameObject;
                if (shortcutMenu != null && !shortcutMenu.activeInHierarchy)
                    shortcutMenu = quickmenu.transform.Find("UserInteractMenu").gameObject;

                
                var fis = Il2CppType.Of<QuickMenu>().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where((fi) => fi.FieldType == Il2CppType.Of<GameObject>()).ToArray();
                //MelonLoader.MelonLogger.Log("[QMStuff] GameObject Fields in QuickMenu:");
                int count = 0;
                foreach (var fi in fis)
                {
                    var value = fi.GetValue(quickmenu)?.TryCast<GameObject>();
                    if (value != shortcutMenu || ++count != 2) continue;
                    //MelonLoader.MelonLogger.Log("[QMStuff] currentPage field: " + fi.Name);
                    currentPageGetter = fi;
                    break;
                }
                if (currentPageGetter == null)
                {
                    Console.WriteLine("[QMStuff] Unable to find field currentPage in QuickMenu");
                    return;
                }
            }

            currentPageGetter.GetValue(quickmenu)?.Cast<GameObject>().SetActive(false);

            var infoBar = GetQuickMenuInstance().transform.Find("QuickMenu_NewElements/_InfoBar").gameObject;
            infoBar.SetActive(pagename == "ShortcutMenu");

            QuickMenuContextualDisplay quickmenuContextualDisplay = GetQuickMenuInstance().field_Private_QuickMenuContextualDisplay_0;
            quickmenuContextualDisplay.Method_Public_Void_EnumNPublicSealedvaUnNoToUs7vUsNoUnique_0(QuickMenuContextualDisplay.EnumNPublicSealedvaUnNoToUs7vUsNoUnique.NoSelection);
            //quickmenuContextualDisplay.Method_Public_Nested0_0(QuickMenuContextualDisplay.Nested0.NoSelection);

            pageTransform.gameObject.SetActive(true);

            currentPageGetter.SetValue(quickmenu, pageTransform.gameObject);
            switch (pagename)
            {
                case "ShortcutMenu":
                    SetIndex(0);
                    break;
                case "UserInteractMenu":
                    SetIndex(3);
                    break;
                default:
                    SetIndex(-1);
                    break;
            }
        }

        // Set the current Quick Menu index
        public static void SetIndex(int index)
        {
            GetQuickMenuInstance().field_Private_Int32_0 = index;
        }
    }
}
