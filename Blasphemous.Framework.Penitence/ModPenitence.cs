using UnityEngine;
using Framework.Managers;
using Framework.Inventory;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI;
using Tools.Playmaker2.Action;

namespace Blasphemous.Framework.Penitence;

/// <summary>
/// An abstract representation of a penitence
/// </summary>
public abstract class ModPenitence
{
    /// <summary>
    /// The unique id of this penitence (PEXX...)
    /// </summary>
    protected internal abstract string Id { get; }

    /// <summary>
    /// The descriptive name of this penitence
    /// </summary>
    protected internal abstract string Name { get; }

    /// <summary>
    /// The full description of this penitence
    /// </summary>
    protected internal abstract string Description { get; }

    /// <summary>
    /// The ID of the item to give for completing the penitence
    /// </summary>
    protected internal abstract string ItemIdToGive { get; }

    /// <summary>
    /// The type of the item to give for completing the penitence
    /// </summary>
    protected internal abstract InventoryManager.ItemType ItemTypeToGive { get; }

    /// <summary>
    /// Should perform any necessary actions to activate this penitence's functionality
    /// </summary>
    protected internal abstract void Activate();

    /// <summary>
    /// Should perform any necessary actions to deactivate this penitence's functionality
    /// </summary>
    protected internal abstract void Deactivate();

    /// <summary>
    /// Should perform any necessary actions to complete the penitence
    /// By default it marks the current penitence as complete and awards
    /// the reward item defined above.
    /// </summary>
    public bool Complete(PenitenceCheckCurrent fsmStateAction)
    {
        // Mark penitence as complete
        Core.PenitenceManager.MarkCurrentPenitenceAsCompleted();

        // If there is not item to give, we're done
        if( null == ItemIdToGive || string.Empty == ItemIdToGive )
        {
            return false;
        }

        BaseInventoryObject item = Core.InventoryManager.GetBaseObject(ItemIdToGive, ItemTypeToGive);
        
        // If the item is not valid, we're done
        if ( null == item )
        {
            return false;
        }

        // Give the item, and, if successful, cue UI pop-up, then, on dialog close, save and finish 
        if( !Core.InventoryManager.AddBaseObject(item) )
        {
            // Item is already owned, or adding the item failed, we're done
            return false;
        }

        PopUpWidget.OnDialogClose += FinishAction;
        UIController.instance.ShowObjectPopUp( UIController.PopupItemAction.GetObejct,
                                                item.caption,
                                                item.picture,
                                                item.GetItemType(),
                                                3f,
                                                true );

        return true;

        void FinishAction()
        {
            PopUpWidget.OnDialogClose -= FinishAction;
            Core.Persistence.SaveGame(true);
            fsmStateAction.Fsm.Event(fsmStateAction.noPenitenceActive);
            fsmStateAction.Finish();
        }        
    }

    internal Sprite InProgressImage { get; private set; }
    internal Sprite CompletedImage { get; private set; }
    internal Sprite AbandonedImage { get; private set; }
    internal Sprite GameplayImage { get; private set; }
    internal Sprite ChooseSelectedImage { get; private set; }
    internal Sprite ChooseUnselectedImage { get; private set; }

    /// <summary>
    /// Stores the associated images for the penitence - only executed on startup
    /// </summary>
    protected abstract void LoadImages(out Sprite inProgress, out Sprite completed, out Sprite abandoned, out Sprite gameplay, out Sprite chooseSelected, out Sprite chooseUnselected);

    /// <summary>
    /// Creates a new custom penitence
    /// </summary>
    public ModPenitence()
    {
        LoadImages(out Sprite inProgress, out Sprite completed, out Sprite abandoned, out Sprite gameplay, out Sprite chooseSelected, out Sprite chooseUnselected);
        InProgressImage = inProgress;
        CompletedImage = completed;
        AbandonedImage = abandoned;
        GameplayImage = gameplay;
        ChooseSelectedImage = chooseSelected;
        ChooseUnselectedImage = chooseUnselected;
    }
}
