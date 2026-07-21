using Items.Interface.Model;
using MelonLoader;
using StellarModdingAPI.Items;
using StellarModdingAPI.Parts;
using StellarModdingAPI.Core;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine;

namespace StellarModdingAPI;

public class Plugin : MelonPlugin
{
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        StellarLogger.Initialize(LoggerInstance);
        StellarLogger.Msg("StellarModdingAPI initialized!");
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
                StellarLogger.Msg($"{item.id}: {item.itemName}");
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
        StellarLogger.Msg($"Registering part definition: {definition.Name}");

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