using Ship.Interface.Settings;
using StellarModdingAPI.Core;
using StellarModdingAPI.StellarDriveIntegration;
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

    public static void Build()
    {
        StellarLogger.Msg($"Building {_definitions.Count} part(s)...");

        foreach (var definition in _definitions)
        {
            StellarLogger.Msg($"Creating part: {definition.Name}");

            PartSettings part = PartFactory.Create(definition);

            _parts.Add(part);

            IntegrationUtilities.AddPart(part);

            StellarLogger.Msg($"Added part: {definition.Name} (ID: {part.id})");
        }
    }

    public static void Clean()
    {
        var ids = _parts.Select(p => p.id).ToHashSet();

        foreach (var item in Object.FindObjectsOfType<PartSettings>()
            .Where(p => ids.Contains(p.id))
            .ToArray())
        {
            Object.Destroy(item);
        }
    }
}