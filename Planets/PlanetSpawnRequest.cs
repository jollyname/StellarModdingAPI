using System;
using UnityEngine;

namespace StellarModdingAPI.Planets
{
    public sealed class PlanetSpawnRequest
    {
        public string SourcePlanetName { get; set; }

        /// <summary>Must not collide with any existing planet's id.</summary>
        public ulong Id { get; set; }

        public string Name { get; set; }

        public float Radius { get; set; }

        /// <summary>Offset from the source's positionRelativeToParent. Zero spawns the clone on top of it.</summary>
        public Vector3 PositionOffset { get; set; }

        public Action<Planet.Terrain.Evaluation.Step.TerrainConfig> TerrainOverride { get; set; }

        public Planet.Terrain.Material.TerrainMaterialConfig[] Materials { get; set; }

        public bool RemoveRings { get; set; } = true;

        public bool RegisterAsServer { get; set; } = true;

        public bool RegisterAsClient { get; set; } = true;
    }
}