using System;
using UnityEngine;

namespace StellarModdingAPI.StellarDriveIntegration;

public static class MaterialUtilities
{
    private static readonly Shader _gameShader = 
        Shader.Find("Shader Graphs/PlanetObjectDefaultLighting") 
        ?? throw new InvalidOperationException("Couldn't find the game's PlanetObjectDefaultLighting shader.");

    private static Material ConvertMaterial(Material original)
    {
        Material material = new(_gameShader);

        if (original != null)
        {
            if (original.HasProperty("_MainTex"))
                material.SetTexture("_AlbedoTex", original.GetTexture("_MainTex"));

            if (original.HasProperty("_BumpMap"))
                material.SetTexture("_NormalMap", original.GetTexture("_BumpMap"));

            if (original.HasProperty("_EmissionMap"))
                material.SetTexture("_EmissionTex", original.GetTexture("_EmissionMap"));

            if (original.HasProperty("_EmissionColor"))
                material.SetColor("_EmissionColor", original.GetColor("_EmissionColor"));
        }

        return material;
    }

    public static void ApplyGameShader(GameObject instance)
    {
        foreach (var renderer in instance.GetComponentsInChildren<Renderer>())
        {
            Material[] originals = renderer.sharedMaterials;
            Material[] converted = new Material[originals.Length];

            for (int i = 0; i < originals.Length; i++)
                converted[i] = ConvertMaterial(originals[i]);

            renderer.materials = converted;
        }
    }
}