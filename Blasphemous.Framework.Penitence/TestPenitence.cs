using System.Collections;
using UnityEngine;

namespace Blasphemous.Framework.Penitence;

internal class TestPenitence : ModPenitence
{
    protected internal override string Id => "PE_Test";

    protected internal override string Name => "Penitence of Testing";

    protected internal override string Description => "Used to test features of the Penitence Framework";

    //protected internal override string ItemIdToGive => "RB01";

    //protected internal override InventoryManager.ItemType ItemTypeToGive => InventoryManager.ItemType.Bead;

    protected override void LoadImages(out Sprite inProgress, out Sprite completed, out Sprite abandoned, out Sprite gameplay, out Sprite chooseSelected, out Sprite chooseUnselected)
    {
        inProgress = null;
        completed = null;
        abandoned = null;
        gameplay = null;
        chooseSelected = null;
        chooseUnselected = null;
    }

    protected internal override void Activate()
    {
        Main.PenitenceFramework.LogError("Test penitence is activated");
    }

    protected internal override void Deactivate()
    {
        Main.PenitenceFramework.LogError("Test penitence is deactivated");
    }

    public override IEnumerator Complete()
    {
        base.Complete();

        yield return new WaitForSeconds(2);
        Main.PenitenceFramework.LogWarning("Waited 2s");
        yield return new WaitForSeconds(2);
        Main.PenitenceFramework.LogWarning("Waited 2 more seconds");
    }
}
