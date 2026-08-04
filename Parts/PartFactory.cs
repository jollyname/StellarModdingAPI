using System;
using MelonLoader;
using Ship.Interface.Model.Parts.StateTypes;
using Ship.Interface.Settings;
using UnityEngine;

namespace StellarModdingAPI.Parts;

public static class PartFactory
{
    /// <exception cref="ArgumentException">If PartDefinition.Prefab is null</exception>
    public static PartSettings Create(PartDefinition definition)
    {
        MelonLogger.Msg($"Creating PartSettings for {definition.Name}");

        if (definition.Prefab == null) throw new ArgumentException($"Part '{definition.Name}' has no prefab!");


        var part = ScriptableObject.CreateInstance<PartSettings>();

        part.fullLabel = definition.Name;
        part.name = definition.Name;
        part.description = definition.Description;
        part.size = definition.LogicalSize;
        part.id = IDAllocator.GetPartID(); // Get ID automatically
        part.mass = definition.Mass;
        part.snappingStyle = definition.Snapping;
        part.internalStateType = PartInternalStateType.None;  // TODO: Stop hardcoding this
        part.buildingCost = definition.BuildingCost;

        if (definition.Thumbnail != null)
        {
            part.thumbnailTex = definition.Thumbnail;
        }
        else
        {
            MelonLogger.Warning($"Part '{definition.Name}' has no thumbnail! It will be replaced by a black texture");
            part.thumbnailTex = Texture2D.blackTexture;
        }

        part.part = PrefabFactory.Create(definition);
        MelonLogger.Msg($"Finished creating part: {definition.Name}");

        return part;
    }
}