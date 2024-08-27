using Blasphemous.ModdingAPI;
using UnityEngine;

namespace Blasphemous.Framework.Penitence;

internal class TestPenitence : ModPenitenceWithBead
{
    protected internal override string Id => "PE_TEST";

    protected internal override string Name => "Penitence of Testing";

    protected internal override string Description => "Used to test features of the Penitence Framework";

    protected override string BeadId => "RB01";

    protected override PenitenceImageCollection Images => new()
    {
        InProgress = CreateColor(Color.green),
        Completed = CreateColor(Color.yellow),
        Abandoned = CreateColor(Color.black),
        Gameplay = CreateColor(Color.cyan),
        ChooseSelected = null,
        ChooseUnselected = null
    };

    private Sprite CreateColor(Color color)
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero, 1, 0, SpriteMeshType.FullRect);
    }

    protected internal override void Activate()
    {
        ModLog.Error("Test penitence is activated");
    }

    protected internal override void Deactivate()
    {
        ModLog.Error("Test penitence is deactivated");
    }

    protected internal override void Update()
    {
        if (Time.frameCount % 60 == 0)
            ModLog.Warn("Updating test penitence");
    }
}
