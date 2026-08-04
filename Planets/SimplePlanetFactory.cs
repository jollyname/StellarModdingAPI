using System;
using System.Threading;
using MelonLoader;
using UnityEngine;

namespace StellarModdingAPI.Planets
{
    public enum TerrainStyle
    {
        /// <summary>Boring terrain with moderate mountains, perfect climate for beans.</summary>
        EarthLike,

        /// <summary>Tall mountains.</summary>
        Mountainous,

        /// <summary>Low, wide dunes.</summary>
        Desert,

        /// <summary>Mostly flat terrain.</summary>
        IcyFlat,

        /// <summary>Exaggerated, chaotic terrain, doesn't look like Earth at all.</summary>
        Alien
    }

    public static class SimplePlanetFactory
    {
        private const ulong AutoIdBase = 600_000_000;
        private static long _autoIdCounter = (long)AutoIdBase;

        /// <summary>
        /// Spawns a new planet cloned from <paramref name="sourcePlanetName"/> with a chosen
        /// terrain look, at a given distance away from the source along a single axis.
        /// </summary>
        /// <param name="sourcePlanetName">Name of the existing in-scene planet to clone.</param>
        /// <param name="name">Name for the new planet.</param>
        /// <param name="radius">Radius of the new planet.</param>
        /// <param name="terrainStyle">Overall terrain look. Defaults to <see cref="TerrainStyle.EarthLike"/>.</param>
        /// <param name="distanceFromSource">
        /// How far to offset the new planet from the source, in meters along the Z axis. Defaults to 5,000,000
        /// </param>
        /// <param name="id">
        /// Optional explicit planet id. If omitted, one is auto-assigned (see <see cref="AutoIdBase"/>).
        /// </param>
        /// <param name="materials">
        /// Optional replacement material palette. If omitted, the clone keeps the
        /// source planet's own textures, the simplest option if you don't need custom textures is to
        /// build an array with <see cref="PlanetMaterialUtility.BuildFromDonors"/>)
        /// </param>
        public static GameObject Spawn(
            string sourcePlanetName,
            string name,
            float radius,
            TerrainStyle terrainStyle = TerrainStyle.EarthLike,
            float distanceFromSource = 5_000_000f,
            ulong? id = null,
            Planet.Terrain.Material.TerrainMaterialConfig[] materials = null,
            MelonLogger.Instance logger = null)
        {
            var request = new PlanetSpawnRequest
            {
                SourcePlanetName = sourcePlanetName,
                Name = name,
                Radius = radius,
                Id = id ?? NextAutoId(),
                PositionOffset = new Vector3(0f, 0f, distanceFromSource), // I should really find a better way to do this, but for now this is fine
                TerrainOverride = GetTerrainRecipe(terrainStyle),
                Materials = materials,
            };

            return PlanetFactory.Spawn(request, logger);
        }

        private static ulong NextAutoId() => (ulong)Interlocked.Increment(ref _autoIdCounter);

        private static Action<Planet.Terrain.Evaluation.Step.TerrainConfig> GetTerrainRecipe(TerrainStyle style)
        {
            return style switch
            {
                TerrainStyle.Mountainous => config => Apply(config, MountainousRecipe, clampMin: 0f, clampMax: 12000f, clampSmoothness: 0.2f),
                TerrainStyle.Desert => config => Apply(config, DesertRecipe, clampMin: 0f, clampMax: 3000f, clampSmoothness: 0.4f),
                TerrainStyle.IcyFlat => config => Apply(config, IcyFlatRecipe, clampMin: 0f, clampMax: 1200f, clampSmoothness: 0.6f),
                TerrainStyle.Alien => config => Apply(config, AlienRecipe, clampMin: 0f, clampMax: 8000f, clampSmoothness: 0.2f),
                TerrainStyle.EarthLike or _ => config => Apply(config, EarthLikeRecipe, clampMin: 0f, clampMax: 8000f, clampSmoothness: 0.2f),
            };
        }

        // I'm not exactly the best with this stuff, so these presets are pretty rough but I think it's fine

        // strength, frequency, octaves, gain, lacunarity
        // (index 0 = continents, 1 = mountains, 2 = medium detail, 3 = surface detail)
        private static readonly NoiseRecipe[] EarthLikeRecipe =
        {
            new(400f, 5f, 2, 0.55f, 2f),
            new(500f, 75f, 6, 0.45f, 2.2f),
            new(240f, 60f, 3, 0.5f, 2f),
            new(80f, 200f, 5, 0.5f, 2f),
        };

        private static readonly NoiseRecipe[] MountainousRecipe =
        {
            new(900f, 5f, 2, 0.55f, 2f),
            new(1200f, 75f, 6, 0.45f, 2.2f),
            new(240f, 60f, 3, 0.5f, 2f),
            new(80f, 200f, 5, 0.5f, 2f),
        };

        private static readonly NoiseRecipe[] DesertRecipe =
        {
            new(150f, 3f, 2, 0.5f, 2f),
            new(60f, 40f, 4, 0.4f, 2f),
            new(90f, 90f, 3, 0.5f, 2f),
            new(40f, 250f, 4, 0.5f, 2f),
        };

        private static readonly NoiseRecipe[] IcyFlatRecipe =
        {
            new(60f, 3f, 2, 0.5f, 2f),
            new(20f, 50f, 3, 0.4f, 2f),
            new(15f, 100f, 2, 0.5f, 2f),
            new(10f, 220f, 3, 0.5f, 2f),
        };

        private static readonly NoiseRecipe[] AlienRecipe =
        {
            new(900f, 5f, 2, 0.55f, 2f),
            new(1200f, 75f, 6, 0.45f, 2.2f),
            new(240f, 60f, 3, 0.5f, 2f),
            new(80f, 200f, 5, 0.5f, 2f),
        };

        private static void Apply(
            Planet.Terrain.Evaluation.Step.TerrainConfig config,
            NoiseRecipe[] recipe,
            float clampMin,
            float clampMax,
            float clampSmoothness)
        {
            int count = Math.Min(recipe.Length, config.noiseSteps?.Length ?? 0);
            for (int i = 0; i < count; i++)
            {
                var n = config.noiseSteps[i].noiseConfig;
                n.strength = recipe[i].Strength;
                n.frequency = recipe[i].Frequency;
                n.octaves = recipe[i].Octaves;
                n.gain = recipe[i].Gain;
                n.lacunarity = recipe[i].Lacunarity;
                config.noiseSteps[i].noiseConfig = n;
            }

            if (config.clampSteps != null && config.clampSteps.Length > 0)
            {
                config.clampSteps[0].min = clampMin;
                config.clampSteps[0].max = clampMax;
                config.clampSteps[0].smoothness = clampSmoothness;
            }
        }

        private readonly struct NoiseRecipe
        {
            public readonly float Strength;
            public readonly float Frequency;
            public readonly int Octaves;
            public readonly float Gain;
            public readonly float Lacunarity;

            public NoiseRecipe(float strength, float frequency, int octaves, float gain, float lacunarity)
            {
                Strength = strength;
                Frequency = frequency;
                Octaves = octaves;
                Gain = gain;
                Lacunarity = lacunarity;
            }
        }
    }
}