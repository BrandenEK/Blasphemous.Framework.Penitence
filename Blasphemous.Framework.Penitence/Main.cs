using BepInEx;

namespace Blasphemous.Framework.Penitence;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.0.2")]
public class Main : BaseUnityPlugin
{
    public static PenitenceFramework PenitenceFramework { get; private set; }

    private void Start()
    {
        PenitenceFramework = new PenitenceFramework();
    }
}
