using System;
using Planet.Terrain.Evaluation.Step;
using Planet.Terrain.Material;
using UnityEngine;

namespace StellarModdingAPI.Planets;

public sealed record class PlanetSpawnRequest
(
    string SourcePlanetName,

    /// <summary>Must not collide with any existing planet's id.</summary>
    ulong Id,

    string Name,

    float Radius,

    /// <summary>Offset from the source's positionRelativeToParent. Zero spawns the clone on top of it.</summary>
    Vector3 PositionOffset,

    Action<TerrainConfig> TerrainOverride,

    TerrainMaterialConfig[]? Materials,

    bool RemoveRings = true,

    bool RegisterAsServer = true,

    bool RegisterAsClient = true
);
