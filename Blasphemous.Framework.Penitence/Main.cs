using BepInEx;

namespace Blasphemous.Framework.Penitence;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.4.1")]
internal class Main : BaseUnityPlugin
{
    public static Main Instance { get; private set; }
    public static PenitenceFramework PenitenceFramework { get; private set; }

    private void Start()
    {
        Instance = this;
        PenitenceFramework = new PenitenceFramework();
    }
}
