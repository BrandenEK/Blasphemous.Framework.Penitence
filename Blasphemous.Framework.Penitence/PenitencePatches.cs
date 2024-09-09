using Framework.Managers;
using Framework.Penitences;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Widgets;
using Gameplay.UI;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.Playmaker2.Action;
using UnityEngine;
using UnityEngine.UI;
using Blasphemous.ModdingAPI;

namespace Blasphemous.Framework.Penitence;

// Add custom penitences to list
[HarmonyPatch(typeof(PenitenceManager), nameof(PenitenceManager.ResetPenitencesList))]
class PenitenceManager_Patch
{
    public static void Postfix(List<IPenitence> ___allPenitences)
    {
        ___allPenitences.AddRange(PenitenceRegister.Penitences.Select(p => new ModPenitenceSystem(p.Id) as IPenitence));
    }
}

// Add any missing custom penitences whe loading save data
[HarmonyPatch(typeof(PenitenceManager), nameof(PenitenceManager.SetCurrentPersistentState))]
class Penitence_Load_Patch
{
    public static void Postfix(List<IPenitence> ___allPenitences)
    {
        ___allPenitences.AddRange(PenitenceRegister.Penitences
            .Where(x => !___allPenitences.Any(y => x.Id == y.Id))
            .Select(p => new ModPenitenceSystem(p.Id) as IPenitence));
    }
}

// Add config settings for custom penitences
[HarmonyPatch(typeof(PenitenceSlot), nameof(PenitenceSlot.SetPenitenceConfig))]
class PenitenceSlot_Patch
{
    public static void Postfix(Dictionary<string, SelectSaveSlots.PenitenceData> ___allPenitences)
    {
        foreach (SelectSaveSlots.PenitenceData data in Main.PenitenceFramework.GetPenitenceData(true))
        {
            ___allPenitences.Add(data.id, data);
        }
    }
}
[HarmonyPatch(typeof(GameplayWidget), nameof(GameplayWidget.Awake))]
class GameplayWidget_Patch
{
    public static void Postfix(List<SelectSaveSlots.PenitenceData> ___PenitencesConfig)
    {
        ___PenitencesConfig.AddRange(Main.PenitenceFramework.GetPenitenceData(false));
    }
}

// Set selecting status when changing options
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.Option_SelectPE01))]
class ChoosePenitenceWidgetSelect1_Patch
{
    public static void Postfix() => Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
}
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.Option_SelectPE02))]
class ChoosePenitenceWidgetSelect2_Patch
{
    public static void Postfix() => Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
}
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.Option_SelectPE03))]
class ChoosePenitenceWidgetSelect3_Patch
{
    public static void Postfix() => Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
}
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.OnClose))]
class ChoosePenitenceWidgetClose_Patch
{
    public static void Postfix()
    {
        Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
        Main.PenitenceFramework.SelectedButtonImage.sprite = Main.PenitenceFramework.NoPenitenceSelectedImage;
        Main.PenitenceFramework.UnselectedButtonImage.sprite = Main.PenitenceFramework.NoPenitenceUnselectedImage;
        Main.PenitenceFramework.CurrentSelectedCustomPenitence = 0;
    }
}

// Display buttons and store action when opening widget
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.Open), typeof(Action), typeof(Action))]
class ChoosePenitenceWidgetOpen_Patch
{
    public static void Postfix(Action onChoosingPenitence)
    {
        Main.PenitenceFramework.ChooseAction = onChoosingPenitence;
        Main.PenitenceFramework.NoPenitenceUnselectedImage = Main.PenitenceFramework.UnselectedButtonImage.sprite;
        Main.PenitenceFramework.NoPenitenceSelectedImage = Main.PenitenceFramework.SelectedButtonImage.sprite;
        Main.PenitenceFramework.CurrentSelectedCustomPenitence = 0;

        // Add lb/rb buttons
        Transform buttonHolder = UnityEngine.Object.FindObjectOfType<NewInventoryWidget>().transform.Find("External/Background/Headers/Inventory/Caption/Selector_Help/");

        Transform parent = Main.PenitenceFramework.UnselectedButtonImage.transform;
        if (parent.childCount == 1)
        {
            GameObject left = UnityEngine.Object.Instantiate(buttonHolder.GetChild(0).gameObject, parent);
            GameObject right = UnityEngine.Object.Instantiate(buttonHolder.GetChild(1).gameObject, parent);
            left.GetComponent<RectTransform>().anchoredPosition = new Vector2(-5, 10);
            right.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 10);
        }
    }
}

// Update selection status when selecting normally or changing custom penitences
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.Option_SelectNoPenitence))]
class ChoosePenitenceWidgetSelectNone_Patch
{
    public static bool Prefix(Text ___penitenceTitle, Text ___penitenceInfoText, CustomScrollView ___penitenceScroll)
    {

        int currPenitenceIdx = Main.PenitenceFramework.CurrentSelectedCustomPenitence;
        if (currPenitenceIdx > 0)
        {
            Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Custom;
            ModPenitence currentPenitence = PenitenceRegister.AtIndex(currPenitenceIdx - 1);
            ___penitenceTitle.text = currentPenitence.Name;
            ___penitenceInfoText.text = currentPenitence.Description;
            Main.PenitenceFramework.SelectedButtonImage.sprite = currentPenitence.CachedImages.ChooseSelected;
            Main.PenitenceFramework.UnselectedButtonImage.sprite = currentPenitence.CachedImages.ChooseUnselected;
        }
        else
        {
            ___penitenceTitle.text = ScriptLocalization.UI_Penitences.NO_PENITENCE;
            Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Bottom;
            ___penitenceInfoText.text = ScriptLocalization.UI_Penitences.NO_PENITENCE_INFO;
            Main.PenitenceFramework.SelectedButtonImage.sprite = Main.PenitenceFramework.NoPenitenceSelectedImage;
            Main.PenitenceFramework.UnselectedButtonImage.sprite = Main.PenitenceFramework.NoPenitenceUnselectedImage;
        }
        ___penitenceScroll.NewContentSetted();
        return false;
    }
}

// Display choose penitence confirmation when choosing custom one
[HarmonyPatch(typeof(UIController), nameof(UIController.ShowConfirmationWidget), typeof(string), typeof(Action), typeof(Action))]
class UIController_Patch
{
    public static void Prefix(ref string infoMessage)
    {
        if (Main.PenitenceFramework.CurrentSelection == PenitenceFramework.Selection.Custom)
            infoMessage = ScriptLocalization.UI_Penitences.CHOOSE_PENITENCE_CONFIRMATION;
    }
}

// When confirming a custom penitence, activate it
[HarmonyPatch(typeof(ChoosePenitenceWidget), nameof(ChoosePenitenceWidget.ContinueWithNoPenitenceAndClose))]
class ChoosePenitenceWidgetActivate_Patch
{
    public static bool Prefix(ChoosePenitenceWidget __instance)
    {
        if (Main.PenitenceFramework.CurrentSelection == PenitenceFramework.Selection.Custom)
        {
            Main.PenitenceFramework.ConfirmCustomPenitence();
            __instance.Close();
            return false;
        }
        return true;
    }
}

// Add the scroll view UI elements when when opening widget
[HarmonyPatch(typeof(AbandonPenitenceWidget), "Open", typeof(Action), typeof(Action))]
class AbandonPenitenceWidgetOpen_Patch
{
    public static bool Prefix(AbandonPenitenceWidget __instance)
    {
        ModLog.Info("AbandonPenitenceWidgetOpen_Patch: PREFIX START");

        // Integrity check
        
        GameObject info = __instance.transform.Find("Info").gameObject;
        if(null == info)
        {
            ModLog.Error("AbandonPenitenceWidgetOpen_Patch: 'Info' object not found!");
            return true;
        }
        else if(null != info.transform.Find("Scroll View"))
        {
            // The UI elements have already been added, do nothing
            return true;
        }

        // Add scrollview and scrollbar UI elements
        {   
            GameObject scrollView = null;
            GameObject scrollBar  = null;

            // Retrieve the UI elements from the Choose Penitence widget
            
            GameObject scrollViewSource = GameObject.Find("Game UI/Content/UI_CHOOSE_PENITENCE/Info/Scroll View");
            if(null == scrollViewSource)
            {
                ModLog.Error("AbandonPenitenceWidgetOpen_Patch: 'Scroll View' object not found!");
                return true;
            }

            GameObject scrollBarSource = GameObject.Find("Game UI/Content/UI_CHOOSE_PENITENCE/Info/Scrollbar");
            if(null == scrollBarSource)
            {
                ModLog.Error("AbandonPenitenceWidgetOpen_Patch: 'Scrollbar' object not found!");
                return true;
            }

            // Create a copy of the UI elements

            scrollView = GameObject.Instantiate(scrollViewSource, info.transform);
            if(null == scrollView)
            {
                ModLog.Error("AbandonPenitenceWidgetOpen_Patch: Failed to instantiate new 'Scroll View' object!");
                return true;
            }

            scrollBar = GameObject.Instantiate(scrollBarSource, info.transform);
            if(null == scrollBar)
            {
                ModLog.Error("AbandonPenitenceWidgetOpen_Patch: Failed to instantiate new 'Scrollbar' object!");
                return true;
            }

            // Configure the new UI elements

            {
                // Update names, since instantiation appends "copy" to them
                scrollView.name = scrollViewSource.name;
                scrollBar.name  = scrollBarSource.name;


                // --- Resize elements to match widget ---

                // Get those transforms!
                GameObject abandonText = info.transform.Find("Text").gameObject;
                
                RectTransform abandonTextRect = abandonText.transform.GetComponent<RectTransform>();
                RectTransform scrollViewRect  = scrollView.transform.GetComponent<RectTransform>();
                RectTransform scrollBarRect   = scrollBar.transform.GetComponent<RectTransform>();

                
                // For whatever reason, the regular text area is somewhat small. Let's make it bigger so
                // we minimize the need for the scroll view              
                abandonTextRect.anchoredPosition = new Vector2(18, 0);
                abandonTextRect.sizeDelta = new Vector2(210, 220);

                // The scroll view has a good position, let's just make it taller to match the regular text area
                scrollViewRect.sizeDelta = new Vector2( scrollViewRect.sizeDelta.x, abandonTextRect.sizeDelta.y);

                // Same with the scrollbar, just stretch it vertically
                scrollBarRect.sizeDelta = new Vector2( scrollBarRect.sizeDelta.x, abandonTextRect.sizeDelta.y );
                
                

                // TODO: Controls
               
            }

        }

        return true;
    }
}

// Show custom penitence when abandoning
[HarmonyPatch(typeof(AbandonPenitenceWidget), nameof(AbandonPenitenceWidget.UpdatePenitenceTextsAndDisplayedMedal))]
class AbandonPenitenceWidgetUpdateAndDisplay_Patch
{
    public static bool Prefix(AbandonPenitenceWidget __instance, Text ___penitenceTitle, Text ___penitenceInfoText, GameObject ___PE01Medal, GameObject ___PE02Medal, GameObject ___PE03Medal)
    {
        ModLog.Info("AbandonPenitenceWidgetUpdateAndDisplay_Patch: PREFIX START");

        // Reactivate in case it was deactivated previously
        ___penitenceInfoText.gameObject.SetActive(true);

        if (Core.PenitenceManager.GetCurrentPenitence() is not ModPenitenceSystem modPenitence)
            return true;

        ModPenitence currPenitence = Main.PenitenceFramework.GetPenitence(modPenitence.Id);
        if (currPenitence == null)
            return false;

        Image medalImage = ___PE02Medal.GetComponentInChildren<Image>();
        Main.PenitenceFramework.Penitence2Image = medalImage.sprite;
        medalImage.sprite = currPenitence.CachedImages.ChooseSelected;
        ___penitenceTitle.text = currPenitence.Name;
        ___penitenceInfoText.text = currPenitence.Description;

        ___PE01Medal.SetActive(false);
        ___PE02Medal.SetActive(true);
        ___PE03Medal.SetActive(false);

        
        GameObject scrollView = null;
        GameObject scrollBar  = null;

        // Check if we have our toys
        {
            Transform info = __instance.transform.Find("Info");
            if(null == info)
            {
                ModLog.Error("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'Info' object not found!");
                return false;
            }

            scrollView = info.transform.Find("Scroll View").gameObject;
            if(null == scrollView)
            {
                ModLog.Error("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'scrollView' object not found!");
                return false;
            }            

            scrollBar = info.transform.Find("Scrollbar").gameObject;
            if(null == scrollBar)
            {
                ModLog.Error("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'scrollBar' object not found!");
                return false;
            }
        }
        
        // Check if text overflows and we need a scrollbar
        if(LayoutUtility.GetPreferredHeight(___penitenceInfoText.rectTransform) > ___penitenceInfoText.gameObject.GetComponent<RectTransform>().rect.height)
        {
            Text scrollText = scrollView.transform.Find("Viewport").Find("Content").Find("Penitence_Text").GetComponent<Text>();
            if(null == scrollText)
            {
                ModLog.Error("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'Penitence_Text' object not found!");
                return false;
            }

            scrollText.text = ___penitenceInfoText.text;

            ___penitenceInfoText.gameObject.SetActive(false);
            scrollBar.gameObject.SetActive(true);
            scrollView.gameObject.SetActive(true);
        }
        else
        {
            scrollBar.gameObject.SetActive(false);
            scrollView.gameObject.SetActive(false);
        }       

        return false;
    }
}

[HarmonyPatch(typeof(AbandonPenitenceWidget), nameof(AbandonPenitenceWidget.OnClose))]
class AbandonPenitenceWidgetClose_Patch
{
    public static void Postfix(GameObject ___PE02Medal)
    {
        ___PE02Medal.GetComponentInChildren<Image>().sprite = Main.PenitenceFramework.Penitence2Image;
    }
}

// Complete penitence
[HarmonyPatch(typeof(PenitenceCheckCurrent), nameof(PenitenceCheckCurrent.OnEnter))]
class CurrentPenitence_Patch
{
    public static bool Prefix(PenitenceCheckCurrent __instance)
    {
        if (Core.PenitenceManager.GetCurrentPenitence() is not ModPenitenceSystem modPenitence)
            return true;

        // I am assuming that this method is only used when the game is over to complete the penitence
        ModLog.Info("Completing custom penitence: " + modPenitence.Id);

        ModPenitence currPenitence = Main.PenitenceFramework.GetPenitence(modPenitence.Id);
        Main.Instance.StartCoroutine(AwaitPenitenceCompletion(__instance, currPenitence));
        return false;
    }

    private static IEnumerator AwaitPenitenceCompletion(PenitenceCheckCurrent action, ModPenitence penitence)
    {
        yield return penitence.Complete();

        Core.Persistence.SaveGame(true);
        action.Fsm.Event(action.noPenitenceActive);
        action.Finish();
    }
}
