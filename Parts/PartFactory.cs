using Ship.Interface.Settings;
using StellarModdingAPI.Core;
using UnityEngine;

namespace StellarModdingAPI.Parts;

public static class PartFactory
{
    public static PartSettings? Create(PartDefinition definition)
    {
        StellarLogger.Msg($"Creating PartSettings for {definition.Name}");

        if (definition.Prefab == null)
        {
            StellarLogger.Error($"Part '{definition.Name}' has no prefab!");
            return null;
        }
        if (definition.Thumbnail == null)
        {
            StellarLogger.Warning($"Part '{definition.Name}' has no thumbnail! It will be replaced by a black texture");
        }

        var part = ScriptableObject.CreateInstance<PartSettings>();

        part.fullLabel = definition.Name;
        part.name = definition.Name;
        part.description = definition.Description;
        part.size = definition.Size;
        part.id = IDAllocator.GetPartID(); // Get ID automatically

        StellarLogger.Msg($"Assigned ID {part.id} to {definition.Name}");

        part.mass = definition.Mass;

        part.thumbnailTex = definition.Thumbnail != null ? definition.Thumbnail : Texture2D.blackTexture;

        part.snappingStyle = definition.Snapping;

        // TODO: Stop hardcoding this
        part.internalStateType = Ship.Interface.Model.Parts.StateTypes.PartInternalStateType.None;

        part.buildingCost = definition.BuildingCost;

        part.part = PrefabFactory.Create(definition);

        StellarLogger.Msg($"Finished creating part: {definition.Name}");

        return part;
    }
}