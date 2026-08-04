using Core.Services;
using FishNet;
using FishNet.Object;
using MelonLoader;
using Planet.Interface.Services;
using System.Reflection;

namespace StellarModdingAPI.Planets
{
    /// <summary>
    /// Wires a cloned planet into FishNet networking and the game's client/server planet trackers,
    /// then triggers world-cell caching. Used by <see cref="PlanetFactory"/>.
    /// </summary>
    internal static class PlanetTrackerRegistry
    {
        public static void Register(global::Planet.Planet planet, bool isServer, MelonLogger.Instance logger)
        {
            // Object.Instantiate() does not register a NetworkObject with FishNet by itself
            var nob = planet.GetComponent<NetworkObject>();
            if (nob != null && isServer)
            {
                InstanceFinder.ServerManager.Spawn(nob);
            }
            else if (nob == null)
            {
                logger.Warning("[PlanetFactory] Clone has no NetworkObject component, skipping network spawn.");
            }

            // Find the live tracker instance, client or server, matching what LoadPlanets() populated at scene start
            object tracker = isServer
                ? (object)ServiceLocator.GetService<IPlanetsServerProvider>()
                : (object)ServiceLocator.GetService<IPlanetsClientProvider>();
            if (tracker == null)
            {
                logger.Error($"[PlanetFactory] Could not find planet tracker instance (isServer={isServer}).");
                return;
            }

            var trackerType = tracker.GetType(); // PlanetsClientTracker or PlanetsServerTracker
            var baseType = trackerType.BaseType;  // PlanetsTracker<T>

            // CreateTrackedPlanet is protected abstract on PlanetsTracker<T>, overridden
            var createMethod = trackerType.GetMethod("CreateTrackedPlanet",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var trackedPlanet = createMethod.Invoke(tracker, new object[] { planet });

            // _planets is protected on the base PlanetsTracker<T>; Add via reflection
            var planetsField = baseType.GetField("_planets", BindingFlags.NonPublic | BindingFlags.Instance);
            var dict = planetsField.GetValue(tracker);
            dict.GetType().GetMethod("Add").Invoke(dict, new object[] { planet.Id, trackedPlanet });

            // Match LoadPlanets(), also register a ring tracker entry, if the clone kept its ring
            var ring = planet.GetComponent<global::Planet.Rings.PlanetRing>();
            if (ring != null)
            {
                var ringsField = baseType.GetField("_rings", BindingFlags.NonPublic | BindingFlags.Instance);
                var ringsDict = ringsField.GetValue(tracker);
                ringsDict.GetType().GetMethod("Add").Invoke(ringsDict, new object[] { planet.Id, ring });
            }

            planet.BuildCache();
        }
    }
}