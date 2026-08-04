using System.Reflection;
using System.Linq;
using Ship.Interface.Settings;
using UI.BuildMenu;
using UnityEngine;

using Object = UnityEngine.Object;
using StellarModdingAPI.Parts;

namespace StellarModdingAPI.Integration;

public static class PartIntegration
{
    // TODO: move to new row when full (probably best to wait for ui changes in demo release) 
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
    public static bool IsPartIDTaken(PartID id)
    {
        return Resources.FindObjectsOfTypeAll<PartSettings>().Any(p => p.id == id);
    }
}