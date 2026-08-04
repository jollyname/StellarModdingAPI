using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace StellarModdingAPI.Planets
{
    public static class PlanetMaterialUtility
    {
        /// <summary>
        /// Builds a material palette by copying individual TerrainMaterialConfig entries from
        /// one or more existing planets, optionally re-tinting each via albedoColor 
        /// (which multiplies against the albedo texture, not ideal but it works)
        /// </summary>
        public static Planet.Terrain.Material.TerrainMaterialConfig[] BuildFromDonors(params (string donorPlanetName, int materialIndex, Color? tint)[] picks)
        {
            var result = new Planet.Terrain.Material.TerrainMaterialConfig[picks.Length];

            for (int i = 0; i < picks.Length; i++)
            {
                var (donorName, materialIndex, tint) = picks[i];

                var donor = UnityEngine.Object.FindObjectsOfType<global::Planet.Planet>()
                    .FirstOrDefault(p => p.name == donorName);
                if (donor == null)
                    throw new InvalidOperationException($"Material donor planet '{donorName}' not found.");

                var donorTerrain = donor.GetComponent<Planet.Terrain.PlanetTerrain>();
                if (donorTerrain?.materials == null || materialIndex >= donorTerrain.materials.Length)
                    throw new InvalidOperationException($"'{donorName}' has no material at index {materialIndex}.");

                var src = donorTerrain.materials[materialIndex];
                result[i] = new Planet.Terrain.Material.TerrainMaterialConfig
                {
                    albedoTexture = src.albedoTexture,
                    albedoColor = tint ?? src.albedoColor,
                    roughnessTexture = src.roughnessTexture,
                    roughnessFloor = src.roughnessFloor,
                    normalMap = src.normalMap,
                    metalness = src.metalness,
                    tile = src.tile,
                    footstepSound = src.footstepSound
                };
            }

            return result;
        }

        internal static IEnumerator RefreshMaterials(Planet.PlanetObjects planetObjects, MelonLogger.Instance logger)
        {
            var ensure = typeof(Planet.PlanetObjects).GetMethod("EnsureMaterialsAreAllocated", BindingFlags.NonPublic | BindingFlags.Instance);

            for (int attempt = 0; attempt < 10; attempt++)
            {
                yield return null;

                try
                {
                    ensure?.Invoke(planetObjects, null);
                    if (PlanetCloneUtility.MaterialsConfigField?.GetValue(planetObjects) != null)
                        yield break;
                }
                catch (Exception ex)
                {
                    logger.Warning($"[PlanetFactory] Material allocation attempt {attempt} failed: {ex.Message}");
                }
            }

            logger.Warning("[PlanetFactory] Materials never allocated after 10 attempts :(");
        }
    }
}