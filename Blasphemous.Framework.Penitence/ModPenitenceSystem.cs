using Framework.Penitences;

namespace Blasphemous.Framework.Penitence;

internal class ModPenitenceSystem : IPenitence
{
    public string id;
    public string Id => id;

    public bool Completed { get; set; }
    public bool Abandoned { get; set; }

    public void Activate()
    {
        Main.PenitenceFramework.ActivatePenitence(Id);
    }

    public void Deactivate()
    {
        Main.PenitenceFramework.DeactivatePenitence(Id);
    }

    public ModPenitenceSystem(string id)
    {
        this.id = id;
    }
}
