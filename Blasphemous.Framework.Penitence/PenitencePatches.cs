using Framework.Managers;
using Framework.Penitences;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Widgets;
using Gameplay.UI;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Playmaker2.Action;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Framework.Penitence;

// Add custom penitences to list
[HarmonyPatch(typeof(PenitenceManager), "ResetPenitencesList")]
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
[HarmonyPatch(typeof(PenitenceSlot), "SetPenitenceConfig")]
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
[HarmonyPatch(typeof(GameplayWidget), "Awake")]
class GameplayWidget_Patch
{
    public static void Postfix(List<SelectSaveSlots.PenitenceData> ___PenitencesConfig)
    {
        ___PenitencesConfig.AddRange(Main.PenitenceFramework.GetPenitenceData(false));
    }
}

// Set selecting status when changing options
[HarmonyPatch(typeof(ChoosePenitenceWidget), "Option_SelectPE01")]
class ChoosePenitenceWidgetSelect1_Patch
{
    public static void Postfix() => Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
}
[HarmonyPatch(typeof(ChoosePenitenceWidget), "Option_SelectPE02")]
class ChoosePenitenceWidgetSelect2_Patch
{
    public static void Postfix() => Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
}
[HarmonyPatch(typeof(ChoosePenitenceWidget), "Option_SelectPE03")]
class ChoosePenitenceWidgetSelect3_Patch
{
    public static void Postfix() => Main.PenitenceFramework.CurrentSelection = PenitenceFramework.Selection.Normal;
}
[HarmonyPatch(typeof(ChoosePenitenceWidget), "OnClose")]
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
[HarmonyPatch(typeof(ChoosePenitenceWidget), "Open", typeof(Action), typeof(Action))]
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
[HarmonyPatch(typeof(ChoosePenitenceWidget), "Option_SelectNoPenitence")]
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
            Main.PenitenceFramework.SelectedButtonImage.sprite = currentPenitence.ChooseSelectedImage;
            Main.PenitenceFramework.UnselectedButtonImage.sprite = currentPenitence.ChooseUnselectedImage;
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
[HarmonyPatch(typeof(UIController), "ShowConfirmationWidget", typeof(string), typeof(Action), typeof(Action))]
class UIController_Patch
{
    public static void Prefix(ref string infoMessage)
    {
        if (Main.PenitenceFramework.CurrentSelection == PenitenceFramework.Selection.Custom)
            infoMessage = ScriptLocalization.UI_Penitences.CHOOSE_PENITENCE_CONFIRMATION;
    }
}

// When confirming a custom penitence, activate it
[HarmonyPatch(typeof(ChoosePenitenceWidget), "ContinueWithNoPenitenceAndClose")]
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

// Show custom penitence when abandoning
[HarmonyPatch(typeof(AbandonPenitenceWidget), "UpdatePenitenceTextsAndDisplayedMedal")]
class AbandonPenitenceWidgetOpen_Patch
{
    public static bool Prefix(Text ___penitenceTitle, Text ___penitenceInfoText, GameObject ___PE01Medal, GameObject ___PE02Medal, GameObject ___PE03Medal)
    {
        if (Core.PenitenceManager.GetCurrentPenitence() is not ModPenitenceSystem modPenitence)
            return true;

        ModPenitence currPenitence = Main.PenitenceFramework.GetPenitence(modPenitence.Id);
        if (currPenitence == null)
            return false;

        Image medalImage = ___PE02Medal.GetComponentInChildren<Image>();
        Main.PenitenceFramework.Penitence2Image = medalImage.sprite;
        medalImage.sprite = currPenitence.ChooseSelectedImage;
        ___penitenceTitle.text = currPenitence.Name;
        ___penitenceInfoText.text = currPenitence.Description;

        ___PE01Medal.SetActive(false);
        ___PE02Medal.SetActive(true);
        ___PE03Medal.SetActive(false);
        return false;
    }
}
[HarmonyPatch(typeof(AbandonPenitenceWidget), "OnClose")]
class AbandonPenitenceWidgetClose_Patch
{
    public static void Postfix(GameObject ___PE02Medal)
    {
        ___PE02Medal.GetComponentInChildren<Image>().sprite = Main.PenitenceFramework.Penitence2Image;
    }
}

// Complete penitence and give item on completion
[HarmonyPatch(typeof(PenitenceCheckCurrent), "OnEnter")]
class CurrentPenitence_Patch
{
    public static bool Prefix(PenitenceCheckCurrent __instance)
    {
        if (Core.PenitenceManager.GetCurrentPenitence() is not ModPenitenceSystem modPenitence)
            return true;

        // I am assuming that this method is only used when the game is over to complete the penitence
        Main.PenitenceFramework.Log("Completing custom penitence: " + modPenitence.Id);
        
        ModPenitence currPenitence = Main.PenitenceFramework.GetPenitence(modPenitence.Id);

        if( !currPenitence.Complete(__instance) )
        {
            // Penitence completion failed, save and exit
            Core.Persistence.SaveGame(true);
            __instance.Fsm.Event(__instance.noPenitenceActive);
            __instance.Finish();
        }

        // We're done, skip base method
        return false;
    }
}
