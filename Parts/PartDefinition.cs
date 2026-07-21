using Items.Interface.Model;
using Ship.Interface.Model;
using System.Collections.Generic;
using UnityEngine;

namespace StellarModdingAPI.Parts;

public class PartDefinition
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public GameObject Prefab { get; set; } = null!;
    public Texture2D Thumbnail { get; set; } = null!;

    public Vector3 Size { get; set; }
    public float Mass { get; set; }

    public SnappingStyle Snapping { get; set; } = SnappingStyle.PreciseOnAny;

    public List<ItemInstance> BuildingCost { get; set; } = [];
}