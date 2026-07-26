using Items.Interface.Model;
using Ship.Interface.Model;
using System.Collections.Generic;
using UnityEngine;

namespace StellarModdingAPI.Parts;

/// <param name="PhysicalSize"> Used for positioning and collision </param>
/// <param name="LogicalSize"> Used for things like snapping </param>
public record class PartDefinition
(
    string Name,
    string Description,

    GameObject Prefab,
    Texture2D? Thumbnail,

    Vector3 PhysicalSize,
    float Mass,

    Vector3 LogicalSize,
    SnappingStyle Snapping = SnappingStyle.PreciseOnAny,

    params List<ItemInstance> BuildingCost
);