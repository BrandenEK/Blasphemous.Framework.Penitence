﻿using Framework.Managers;
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

// Display buttons and store action when opening widget
[HarmonyPatch(typeof(AbandonPenitenceWidget), "Open", typeof(Action), typeof(Action))]
class AbandonPenitenceWidgetOpen_Patch
{
    public static void Postfix(AbandonPenitenceWidget __instance)
    {

        GameObject info = __instance.transform.Find("Info").gameObject;
        if(null == info)
        {
            ModLog.Error("AbandonPenitenceWidgetOpen_Patch: 'Info' object not found!");
            return;
        }

        if(null != info.transform.Find("Scroll View"))
        {
            ModLog.Info("AbandonPenitenceWidgetOpen_Patch: Scroll already added!");
            return;
        }

        // Add scrollview and scrollbar
        
        GameObject scrollView = null;
        GameObject scrollBar = null;
        {
            GameObject scrollViewSource = GameObject.Find("Game UI/Content/UI_CHOOSE_PENITENCE/Info/Scroll View");
            if(null == scrollViewSource)
            {
                ModLog.Error("AbandonPenitenceWidgetOpen_Patch: 'Scroll view' object not found!");
                return;
            }

            GameObject scrollBarSource = GameObject.Find("Game UI/Content/UI_CHOOSE_PENITENCE/Info/Scrollbar");
            if(null == scrollBarSource)
            {
                ModLog.Error("AbandonPenitenceWidgetOpen_Patch: 'Scrollbar' object not found!");
                return;
            }

            scrollView = GameObject.Instantiate(scrollViewSource, info.transform);
            {
                scrollView.name = "Scroll View";
                //scrollView.transform.GetComponent<CustomScrollView>().
            }

            scrollBar = GameObject.Instantiate(scrollBarSource, info.transform);
            {
                scrollBar.name = "Scrollbar";
                //scrollBar.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 200);
            }

        }
    }
}

// Show custom penitence when abandoning
[HarmonyPatch(typeof(AbandonPenitenceWidget), nameof(AbandonPenitenceWidget.UpdatePenitenceTextsAndDisplayedMedal))]
class AbandonPenitenceWidgetUpdateAndDisplay_Patch
{
    public static bool Prefix(AbandonPenitenceWidget __instance, Text ___penitenceTitle, Text ___penitenceInfoText, GameObject ___PE01Medal, GameObject ___PE02Medal, GameObject ___PE03Medal)
    {
        Transform scrollView = __instance.transform.Find("Info/Scroll View");
        if(null == scrollView)
        {
            ModLog.Error("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'scrollView' object not found!");
        }
        else
        {
            ModLog.Info("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'scrollView' loaded!");
        }

        Transform scrollBar = __instance.transform.Find("Info/Scrollbar");
        if(null == scrollBar)
        {
            ModLog.Error("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'scrollBar' object not found!");
        }
        else
        {
            ModLog.Info("AbandonPenitenceWidgetUpdateAndDisplay_Patch: 'scrollBar' loaded!");
            scrollBar.gameObject.SetActive(true);
        }

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
