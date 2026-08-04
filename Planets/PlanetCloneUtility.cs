using System;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace StellarModdingAPI.Planets
{
    internal static class PlanetCloneUtility
    {
        // 'Planet.Planet' properties are get-only, we have to set them directly via reflection
        private static readonly FieldInfo IdField =
            typeof(global::Planet.Planet).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo PositionRelativeToParentField =
            typeof(global::Planet.Planet).GetField("positionRelativeToParent", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo TerrainConfigField =
            typeof(Planet.Terrain.PlanetTerrain).GetField("terrainConfig", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo TerrainStackField =
            typeof(Planet.Terrain.PlanetTerrain).GetField("_terrainOperationStack", BindingFlags.NonPublic | BindingFlags.Instance);

        // Shared with PlanetMaterialUtility, which needs to poll this field while waiting for
        // PlanetObjects to allocate its material array
        internal static readonly FieldInfo MaterialsConfigField =
            typeof(Planet.PlanetObjects).GetField("_materials", BindingFlags.NonPublic | BindingFlags.Instance);

        public static GameObject Clone(PlanetSpawnRequest request, MelonLogger.Instance logger)
        {
            var source = UnityEngine.Object.FindObjectsOfType<global::Planet.Planet>().FirstOrDefault(p => p.name == request.SourcePlanetName);
            if (source == null)
            {
                logger.Error($"[PlanetFactory] Source planet '{request.SourcePlanetName}' not found");
                return null;
            }

            // Cloning the whole GameObject brings every component on it (PlanetPhysicalProperties,
            // PlanetTerrain, PlanetObjects, PlanetBakedLighting, PlanetNetworkedState,
            // PlanetHotspotsEnvironment, NetworkObject, NetworkObserver)
            GameObject clone = UnityEngine.Object.Instantiate(source.gameObject);
            clone.name = request.Name;

            var clonedPlanet = clone.GetComponent<global::Planet.Planet>();
            IdField.SetValue(clonedPlanet, request.Id);

            var terrain = clone.GetComponent<Planet.Terrain.PlanetTerrain>();

            if (request.TerrainOverride != null)
            {
                var config = (Planet.Terrain.Evaluation.Step.TerrainConfig)TerrainConfigField.GetValue(terrain);
                request.TerrainOverride(config);
            }

            if (request.Materials != null && request.Materials.Length > 0)
            {
                terrain.materials = request.Materials;

                var planetObjects = clone.GetComponent<Planet.PlanetObjects>();
                if (planetObjects != null)
                    MelonCoroutines.Start(PlanetMaterialUtility.RefreshMaterials(planetObjects, logger));
            }

            RebuildTerrainStack(terrain, request.Radius, logger);

            // positionRelativeToParent
            if (PositionRelativeToParentField != null)
            {
                object currentPos = PositionRelativeToParentField.GetValue(clonedPlanet);
                object newPos = AddVector3Double(currentPos, request.PositionOffset, logger);
                PositionRelativeToParentField.SetValue(clonedPlanet, newPos);
            }
            else
            {
                logger.Warning("[PlanetFactory] Could not find 'positionRelativeToParent', position offset was NOT applied");
            }

            return clone;
        }

        private static void RebuildTerrainStack(Planet.Terrain.PlanetTerrain terrain, float newRadius, MelonLogger.Instance logger)
        {
            if (TerrainConfigField == null || TerrainStackField == null)
            {
                logger.Error("[PlanetFactory] Could not resolve terrain reflection fields, a game update may have renamed them");
                return;
            }

            object terrainConfig = TerrainConfigField.GetValue(terrain);
            int highestLod = terrain.HighestLOD;

            // Constructor signature per PlanetTerrain.Awake(): (terrainConfig, radius, highestLOD)
            var stackType = TerrainStackField.FieldType;
            var ctor = stackType.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { terrainConfig?.GetType() ?? stackType, typeof(float), typeof(int) },
                null);

            // Fall back to Activator
            object newStack = ctor != null
                ? ctor.Invoke(new[] { terrainConfig, (object)newRadius, (object)highestLod })
                : Activator.CreateInstance(stackType, terrainConfig, newRadius, highestLod);

            TerrainStackField.SetValue(terrain, newStack);
        }

        // Vector3Double is a Core.Values type so we add to it via reflection too.
        private static object AddVector3Double(object vector3Double, Vector3 offset, MelonLogger.Instance logger)
        {
            var type = vector3Double.GetType();
            var ctor = type.GetConstructor(new[] { typeof(double), typeof(double), typeof(double) });
            var xField = type.GetField("x") ?? type.GetField("X");
            var yField = type.GetField("y") ?? type.GetField("Y");
            var zField = type.GetField("z") ?? type.GetField("Z");

            if (ctor == null || xField == null || yField == null || zField == null)
            {
                logger.Warning("[PlanetFactory] Could not reflect into Vector3Double x/y/z fields, position offset NOT applied");
                return vector3Double;
            }

            double x = Convert.ToDouble(xField.GetValue(vector3Double)) + offset.x;
            double y = Convert.ToDouble(yField.GetValue(vector3Double)) + offset.y;
            double z = Convert.ToDouble(zField.GetValue(vector3Double)) + offset.z;
            return ctor.Invoke(new object[] { x, y, z });
        }
    }
}