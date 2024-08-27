using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Blasphemous.ModdingAPI.Input;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Framework.Penitence;

/// <summary>
/// Handles using custom penitences
/// </summary>
public class PenitenceFramework : BlasMod
{
    internal PenitenceFramework() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    internal Selection CurrentSelection { get; set; }
    internal System.Action ChooseAction { get; set; }

    /// <summary>
    /// Recalculate list of penitences after all registered
    /// </summary>
    protected override void OnAllInitialized()
    {
        if (PenitenceRegister.Total > 0)
            Core.PenitenceManager.ResetPersistence();
    }

#if DEBUG
    /// <summary>
    /// Register the test penitence
    /// </summary>
    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterPenitence(new TestPenitence());
    }
#endif

    /// <summary>
    /// When selecting at altar, process input
    /// </summary>
    protected override void OnUpdate()
    {
        // Update all penitences while the game is laoded
        if (SceneHelper.GameSceneLoaded)
        {
            foreach (ModPenitence p in PenitenceRegister.Penitences)
                p.Update();
        }

        // Handle input on selection altar
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

    /// <summary>
    /// Acivates a custom penitence
    /// </summary>
    public void ActivatePenitence(string id)
    {
        PenitenceRegister.Penitences.FirstOrDefault(p => p.Id == id)?.Activate();
    }

    /// <summary>
    /// Deactivates a custom penitence
    /// </summary>
    public void DeactivatePenitence(string id)
    {
        PenitenceRegister.Penitences.FirstOrDefault(p => p.Id == id)?.Deactivate();
    }

    /// <summary>
    /// Gets a penitence by id
    /// </summary>
    public ModPenitence GetPenitence(string id)
    {
        return PenitenceRegister.Penitences.FirstOrDefault(p => p.Id == id);
    }

    internal IEnumerable<SelectSaveSlots.PenitenceData> GetPenitenceData(bool mainMenu)
    {
        return PenitenceRegister.Penitences.Select(penitence => new SelectSaveSlots.PenitenceData()
        {
            id = penitence.Id,
            InProgress = mainMenu ? penitence.CachedImages.InProgress : penitence.CachedImages.Gameplay,
            Completed = penitence.CachedImages.Completed,
            Missing = penitence.CachedImages.Abandoned
        });
    }

    /// <summary>
    /// Activate penitence when choosing custom one at altar
    /// </summary>
    internal void ConfirmCustomPenitence()
    {
        ModPenitence newPenitence = PenitenceRegister.AtIndex(CurrentSelectedCustomPenitence - 1);
        ModLog.Info("Activating custom penitence: " + newPenitence.Id);
        Core.PenitenceManager.ActivatePenitence(newPenitence.Id);
        ChooseAction();
    }

    private int m_CurrentSelectedCustomPenitence = 0;
    internal int CurrentSelectedCustomPenitence
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
    internal Image UnselectedButtonImage
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
    internal Image SelectedButtonImage
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
    internal Sprite NoPenitenceSelectedImage
    {
        get => m_NoPenitenceSelectedImage;
        set => m_NoPenitenceSelectedImage ??= value;
    }

    private Sprite m_NoPenitenceUnselectedImage;
    internal Sprite NoPenitenceUnselectedImage
    {
        get => m_NoPenitenceUnselectedImage;
        set => m_NoPenitenceUnselectedImage ??= value;
    }

    private Sprite m_Penitence2Image;
    internal Sprite Penitence2Image
    {
        get => m_Penitence2Image;
        set => m_Penitence2Image ??= value;
    }

    internal enum Selection
    {
        Normal,
        Bottom,
        Custom
    }
}
