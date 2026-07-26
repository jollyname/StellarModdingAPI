using MelonLoader;

namespace StellarModdingAPI.Abstract; 


/// <summary>
/// MelonMod base class with some API specific features
/// </summary>
/// <remarks> Make sure to call the base implementations of OnInitializeMelon and OnDeinitializeMelon, if you choose to override them </remarks>
public abstract class StellarMelonMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        Plugin.Events.LoadAssets += LoadAssets;
        Plugin.Events.CreateItems += CreateItems;
        Plugin.Events.CreateParts += CreateParts;
    }

    public override void OnDeinitializeMelon()
    {
        Plugin.Events.LoadAssets -= LoadAssets;
        Plugin.Events.CreateItems -= CreateItems;
        Plugin.Events.CreateParts -= CreateParts;
    }


    public virtual void LoadAssets() {}
    public virtual void CreateItems() {}
    public virtual void CreateParts() {}
}