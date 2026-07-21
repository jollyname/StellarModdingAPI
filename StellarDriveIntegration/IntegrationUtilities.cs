using System.Reflection;
using UnityEngine;

using Object = UnityEngine.Object;
using Ship.Interface.Settings;
using System.Linq;
using UI.BuildMenu;

namespace StellarModdingAPI.StellarDriveIntegration;

/// <summary>
/// Provides various utilities to avoid direct interaction with StellarDrive's code in the rest of the project
/// </summary>
public static class IntegrationUtilities
{
    /// <summary>
    /// Adds a custom part to the game's menus
    /// Method should be called after loading the main scene
    /// </summary>
    public static void AddPart(PartSettings part)
    {
        BuildMenu buildMenu = Object.FindFirstObjectByType<BuildMenu>();
        BuildPanel partsPanel = buildMenu.GetComponentsInChildren<BuildPanel>().FirstOrDefault(p => p.gameObject.name == "PartsPanel");

        FieldInfo rowsField = partsPanel.GetType().GetField("_rows", BindingFlags.NonPublic | BindingFlags.Instance);
        BuildRow[] originalRows = (BuildRow[])rowsField.GetValue(partsPanel);

        BuildRow row = originalRows.FirstOrDefault();
        row.parts.Add(part);
    }

    // <summary>
    /// Checks if a Part with the ID already exists
    /// </summary>
    public static bool IsPartIDTaken(ushort id)
    {
        return Resources.FindObjectsOfTypeAll<PartSettings>().Any(p => p.id == id);
    }
}