using Ship.Interface.Model.Parts;
using Ship.Parts.Common;
using UnityEngine;
using StellarModdingAPI.StellarDriveIntegration;

namespace StellarModdingAPI.Parts;

public static class PrefabFactory
{
    public static GameObject Create(PartDefinition definition)
    {
        Vector3 offset = Vector3.up * (definition.Size.y / 2f);

        GameObject partObject = new(definition.Name);
        var bounds = partObject.AddComponent<ShipPartBounds>();
        bounds.bounds = definition.Size;
        bounds.center = offset;

        GameObject visualContainer = new("Visuals");
        visualContainer.AddComponent<DefaultShipPartVisuals>();
        visualContainer.transform.SetParent(partObject.transform, false);

        GameObject meshInstance = Object.Instantiate(definition.Prefab);
        MaterialUtilities.ApplyGameShader(meshInstance);

        meshInstance.transform.SetParent(visualContainer.transform, false);
        meshInstance.transform.localPosition = Vector3.zero;
        meshInstance.transform.localRotation = Quaternion.identity;

        partObject.SetActive(false);
        partObject.hideFlags = HideFlags.HideAndDontSave;

        return partObject;
    }
}