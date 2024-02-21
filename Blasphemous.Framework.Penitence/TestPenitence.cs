using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
using System.Collections;
using UnityEngine;

namespace Blasphemous.Framework.Penitence;

internal class TestPenitence : ModPenitence
{
    protected internal override string Id => "PE_Test";

    protected internal override string Name => "Penitence of Testing";

    protected internal override string Description => "Used to test features of the Penitence Framework";

    protected override void LoadImages(out Sprite inProgress, out Sprite completed, out Sprite abandoned, out Sprite gameplay, out Sprite chooseSelected, out Sprite chooseUnselected)
    {
        inProgress = CreateColor(Color.green);
        completed = CreateColor(Color.yellow);
        abandoned = CreateColor(Color.black);
        gameplay = CreateColor(Color.cyan);
        chooseSelected = null;
        chooseUnselected = null;
    }

    private Sprite CreateColor(Color color)
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero, 1, 0, SpriteMeshType.FullRect);
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

        if (Core.InventoryManager.IsRosaryBeadOwned(BEAD_ID))
            yield break;

        BaseInventoryObject obj = Core.InventoryManager.GetBaseObject(BEAD_ID, InventoryManager.ItemType.Bead);
        Main.PenitenceFramework.LogWarning("Found object: " + obj.lore);

        Core.InventoryManager.AddRosaryBead(obj as RosaryBead);
        UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, obj.caption, obj.picture, InventoryManager.ItemType.Bead);

        while (UIController.instance.IsShowingPopUp())
        {
            Main.PenitenceFramework.LogError("Waiting while show");
            yield return null;
        }

        Main.PenitenceFramework.LogWarning("Finished waiting item added");
    }

    private const string BEAD_ID = "RB01";
}
