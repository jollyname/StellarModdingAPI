using MelonLoader;
using Ship.Interface.Settings;
using StellarModdingAPI.Integration;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarModdingAPI.Parts;

public static class PartRegistry
{
    private static readonly List<PartDefinition> _definitions = [];
    private static readonly List<PartSettings> _parts = [];

    public static void Register(PartDefinition definition)
    {
        _definitions.Add(definition);
    }

    public static void Rebuild()
    {
        MelonLogger.Msg($"Building {_definitions.Count} part(s)...");

        Clean();
        IDAllocator.Reset();

        foreach (var definition in _definitions)
        {
            PartSettings part = PartFactory.Create(definition);

            _parts.Add(part);

            PartIntegration.AddPart(part);

            MelonLogger.Msg($"Added part: {definition.Name} (ID: {part.id})");
        }
    }

    public static void Clean()
    {
        var ids = _parts.Select(p => p.id).ToHashSet();

        foreach (var item in Object.FindObjectsOfType<PartSettings>()
            .Where(p => ids.Contains(p.id))
            .ToArray())
        {
            Object.DestroyImmediate(item);
        }
    }
}