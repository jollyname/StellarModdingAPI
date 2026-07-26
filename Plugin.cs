using Items.Interface.Model;
using MelonLoader;
using StellarModdingAPI.Items;
using StellarModdingAPI.Parts;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine;
using System;
using StellarModdingAPI.Integration;

namespace StellarModdingAPI;

public class Plugin : MelonPlugin
{
    public static class Events
    {
        public static event Action? LoadAssets;
        public static event Action? CreateItems;
        public static event Action? CreateParts;

        internal static void InvokeLoadAssets() => LoadAssets?.Invoke();
        internal static void InvokeCreateItems() => CreateItems?.Invoke();
        internal static void InvokeCreateParts() => CreateParts?.Invoke();
    }


    public override void OnInitializeMelon()
        => MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded);
     
    public override void OnDeinitializeMelon()
        => MelonEvents.OnSceneWasLoaded.Unsubscribe(OnSceneWasLoaded);


    public override void OnLateInitializeMelon()
    {
        Events.InvokeLoadAssets();

        Events.InvokeCreateItems();
        ItemRegistry.Refresh();

        Events.InvokeCreateParts();
    }

    public static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (buildIndex == SceneIntegration.GameInfo.BuildIndex)
        {
            ItemRegistry.Refresh();
            PartRegistry.Rebuild();
        }
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


    public static void RegisterPart(PartDefinition definition)
    {
        PartRegistry.Register(definition);
    }
}