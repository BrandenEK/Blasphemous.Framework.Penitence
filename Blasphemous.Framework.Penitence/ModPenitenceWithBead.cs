using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
using System.Collections;

namespace Blasphemous.Framework.Penitence;

/// <summary>
/// An abstract representation of a penitence that rewards a bead upon completion
/// </summary>
public abstract class ModPenitenceWithBead : ModPenitence
{
    /// <summary>
    /// The id of the bead to give upon completion
    /// </summary>
    protected abstract string BeadId { get; }

    /// <summary>
    /// Completes the penitence and gives the reward bead
    /// </summary>
    public override IEnumerator Complete()
    {
        yield return base.Complete();

        if (Core.InventoryManager.IsRosaryBeadOwned(BeadId))
            yield break;

        RosaryBead bead = Core.InventoryManager.GetBaseObject(BeadId, InventoryManager.ItemType.Bead) as RosaryBead;
        Core.InventoryManager.AddRosaryBead(bead);
        UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, bead.caption, bead.picture, InventoryManager.ItemType.Bead);

        while (UIController.instance.IsShowingPopUp())
            yield return null;
    }
}
