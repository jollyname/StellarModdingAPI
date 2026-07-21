using Items.Interface.Model;
using MelonLoader;
using StellarModdingAPI.Items;
using StellarModdingAPI.Parts;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine;

namespace StellarModdingAPI;

public class Plugin : MelonPlugin
{
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        MelonLogger.Msg("StellarModdingAPI initialized!");
    }

    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();
    }

    // TODO: Move this somewhere more appropriate
    public override void OnUpdate()
    {
        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {
            // Find every ItemSettings object in the game and return their ID
            var items = Resources.FindObjectsOfTypeAll<ItemSettings>().OrderBy(i => i.id);

            foreach (var item in items)
            {
                MelonLogger.Msg($"{item.id}: {item.itemName}");
            }
        }
    }

    // Helpers
    public static void Initialize()
    {
        ItemRegistry.Initialize();
    }

    public static void RegisterPart(PartDefinition definition)
    {
        MelonLogger.Msg($"Registering part definition: {definition.Name}");

        PartRegistry.Register(definition);
    }

    public static void Build()
    {
        PartRegistry.Build();
    }

    public static void Clean()
    {
        PartRegistry.Clean();
    }
}