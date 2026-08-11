using System;
using MelonLoader;
using Planet.Rings;
using UnityEngine;

namespace StellarModdingAPI.Planets;

public static class PlanetFactory
{
    private static readonly MelonLogger.Instance DefaultLog = new("StellarModdingAPI.PlanetFactory");

    /// <summary>
    /// Clones <see cref="PlanetSpawnRequest.SourcePlanetName"/>, applies the requested
    /// terrain/material overrides, and registers it with the appropriate planet trackers.
    /// Returns the new planet's GameObject, or null if the source planet could not be found.
    /// </summary>
    public static GameObject? Spawn(PlanetSpawnRequest request, MelonLogger.Instance? logger = null)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrEmpty(request.SourcePlanetName))
            throw new ArgumentException("SourcePlanetName is required.", nameof(request));

        logger ??= DefaultLog;

        var clone = PlanetCloneUtility.Clone(request, logger);
        if (clone == null) return null;

        // This doesn't remove the ring's asteroids for some reasons
        if (request.RemoveRings)
        {
            var ring = clone.GetComponent<PlanetRing>();
            if (ring != null)
                UnityEngine.Object.Destroy(ring);
        }

        var planet = clone.GetComponent<Planet.Planet>();

        if (request.RegisterAsServer)
            PlanetTrackerRegistry.Register(planet, isServer: true, logger);
        if (request.RegisterAsClient)
            PlanetTrackerRegistry.Register(planet, isServer: false, logger);

        return clone;
    }
}
