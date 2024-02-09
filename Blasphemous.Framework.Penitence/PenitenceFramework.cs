﻿using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Input;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Framework.Penitence;

public class PenitenceFramework : BlasMod
{
    public PenitenceFramework() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    public Selection CurrentSelection { get; set; }
    public System.Action ChooseAction { get; set; }

    protected override void OnAllInitialized()
    {
        if (PenitenceRegister.Total > 0)
            Core.PenitenceManager.ResetPersistence();
    }

    protected override void OnUpdate()
    {
        if (CurrentSelection == Selection.Normal)
            return;

        if (InputHandler.GetButtonDown(ButtonCode.InventoryLeft))
        {
            CurrentSelectedCustomPenitence--;
            Object.FindObjectOfType<ChoosePenitenceWidget>().Option_SelectNoPenitence();
        }
        else if (InputHandler.GetButtonDown(ButtonCode.InventoryRight))
        {
            CurrentSelectedCustomPenitence++;
            Object.FindObjectOfType<ChoosePenitenceWidget>().Option_SelectNoPenitence();
        }
    }

    public void ActivatePenitence(string id)
    {
        PenitenceRegister.Penitences.FirstOrDefault(p => p.Id == id)?.Activate();
    }

    public void DeactivatePenitence(string id)
    {
        PenitenceRegister.Penitences.FirstOrDefault(p => p.Id == id)?.Deactivate();
    }

    public ModPenitence GetPenitence(string id)
    {
        return PenitenceRegister.Penitences.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<SelectSaveSlots.PenitenceData> GetPenitenceData(bool mainMenu)
    {
        return PenitenceRegister.Penitences.Select(penitence => new SelectSaveSlots.PenitenceData()
        {
            id = penitence.Id,
            InProgress = mainMenu ? penitence.InProgressImage : penitence.GameplayImage,
            Completed = penitence.CompletedImage,
            Missing = penitence.AbandonedImage
        });
    }

    public void ConfirmCustomPenitence()
    {
        ModPenitence newPenitence = PenitenceRegister.AtIndex(CurrentSelectedCustomPenitence - 1);
        Log("Activating custom penitence: " + newPenitence.Id);
        Core.PenitenceManager.ActivatePenitence(newPenitence.Id);
        ChooseAction();
    }

    private int m_CurrentSelectedCustomPenitence = 0;
    public int CurrentSelectedCustomPenitence
    {
        get => m_CurrentSelectedCustomPenitence;
        set
        {
            m_CurrentSelectedCustomPenitence = value > PenitenceRegister.Total ? 0
                : value < 0 ? PenitenceRegister.Total
                : value;
        }
    }

    private Image m_UnselectedButtonImage;
    public Image UnselectedButtonImage
    {
        get
        {
            if (m_UnselectedButtonImage == null)
            {
                m_UnselectedButtonImage = Object.FindObjectOfType<ChoosePenitenceWidget>().transform.Find("Options/NoPenitence/Image").GetComponent<Image>();
            }
            return m_UnselectedButtonImage;
        }
    }

    private Image m_SelectedButtonImage;
    public Image SelectedButtonImage
    {
        get
        {
            if (m_SelectedButtonImage == null)
            {
                m_SelectedButtonImage = UnselectedButtonImage.transform.Find("Selected/SelectedIconWithBorder").GetComponent<Image>();
            }
            return m_SelectedButtonImage;
        }
    }

    private Sprite m_NoPenitenceSelectedImage;
    public Sprite NoPenitenceSelectedImage
    {
        get => m_NoPenitenceSelectedImage;
        set => m_NoPenitenceSelectedImage ??= value;
    }

    private Sprite m_NoPenitenceUnselectedImage;
    public Sprite NoPenitenceUnselectedImage
    {
        get => m_NoPenitenceUnselectedImage;
        set => m_NoPenitenceUnselectedImage ??= value;
    }

    private Sprite m_Penitence2Image;
    public Sprite Penitence2Image
    {
        get => m_Penitence2Image;
        set => m_Penitence2Image ??= value;
    }

    public enum Selection
    {
        Normal,
        Bottom,
        Custom
    }
}
