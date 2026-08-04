// Originally written by Max-H for the StellarModdingToolkit.
// Source: https://github.com/Max-H-0/StellarModdingToolkit

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;

using Object = UnityEngine.Object;

namespace StellarModdingAPI.Assets;

/// <summary>
/// Loads Assets in an Assembly for later use
/// (Assumes every EmbeddedResource is an AssetBundle)
/// </summary>
public class AssetLoader
{
    private readonly Assembly _assembly;
    private readonly string[] _providedKeys;
    private readonly MelonLogger.Instance? _logger;

    private readonly Dictionary<string, Object?> _loadedAssets = [];
    
    
    /// <summary> Creates new AssetLoader </summary>
    /// <param name="assembly"> The Assembly the Assets' AssetBundles(as "EmbeddedResource"s) are located in </param>
    /// <param name="assetKeys"> The Names of the Assets that should be loaded </param>
    /// <param name="logger"> The Logger that should be used (if any) </param>;
    /// <remarks> Usage before OnLateInitializeMelon may cause Native Error! </remarks>
    public AssetLoader(Assembly assembly, string[] assetKeys, MelonLogger.Instance? logger = null)
    {
        _assembly = assembly;
        _providedKeys = assetKeys;
        _logger = logger;

        LoadAssets();
    }


    /// <summary> Gets a previously loaded Asset by Name (throws on failure) </summary>
    /// <param name="key"> The Name of the Asset </param>
    public Object GetAsset(string key)
    {
        if (TryGetAsset(key, out var asset)) return asset; 
        
        if (_providedKeys.Contains(key))
        {
            throw new NullReferenceException("Attempted to lookup valid asset key that failed to load on initialization");
        }
        else
        {
            throw new KeyNotFoundException("Attempted to lookup invalid asset key that wasn't provided on initialization");
        }
    }

    /// <summary> Gets a previously loaded Asset by Name and casts it (throws on failure) </summary>
    /// <param name="key"> The Name of the Asset </param>
    public T GetAsset<T>(string key)
        where T : Object
    {
        var @object = GetAsset(key);

        if (@object is T asset)
        {
            return asset;
        }
        else
        {
            throw new InvalidCastException($"Couldn't cast loaded asset '{key}' of type '{@object.GetType()}' to type '{typeof(T)}'");
        }
    }

    /// <summary> Safely gets a previously loaded Asset by Name</summary>
    /// <param name="key"> The Name of the Asset </param>
    public bool TryGetAsset(string key, [NotNullWhen(true)] out Object? asset)
    {
        return _loadedAssets.TryGetValue(key, out asset);
    }    
    
    /// <summary> Safely gets a previously loaded Asset by Name and casts it </summary>
    /// <param name="key"> The Name of the Asset </param>
    public bool TryGetAsset<T>(string key, [NotNullWhen(true)] out T? asset)
        where T : Object
    {
        TryGetAsset(key, out var @object);

        asset = @object as T;
        return asset != null;
    }
    

    private void LoadAssets()
    {
        _logger?.Msg($"Loading assets...");
        var resourceNames = _assembly.GetManifestResourceNames();

        foreach (var name in resourceNames)
        {
            _logger?.Msg($"Processing resource: {name}");

            try
            {
                using Stream stream = _assembly.GetManifestResourceStream(name);
                var assetBundle = AssetBundle.LoadFromStream(stream);

                if (assetBundle != null)
                {
                    Object[] assets = assetBundle.LoadAllAssets();
                    SetMatchingAssets(assets);
                }
                else
                {
                    _logger?.Msg($"Resource '{name}' is not an AssetBundle, proceeding...");
                }
            }
            catch (Exception e)
            {
                _logger?.Warning($"Failed to load resource '{name}' as AssetBundle: {e}");
            }
        }

        var unloadedKeys = _providedKeys.Where(k => !_loadedAssets.ContainsKey(k));
        var totalCount = _providedKeys.Length;
        var loadedCount = totalCount - unloadedKeys.Count();
        _logger?.Msg($"Asset loading complete: {loadedCount}/{totalCount} asset(s) loaded");

        if (unloadedKeys.Any()) _logger?.Warning($"Couldn't find/load: {string.Join(", ", unloadedKeys)}");
    }

    private void SetMatchingAssets(Object[] loadedAssets)
    {
        foreach (var asset in loadedAssets)
        {
            var name = asset.name;
            
            if(!_providedKeys.Contains(name))
            {
                _logger?.Warning($"The following asset was found but matches no key: {name}");
                continue;
            }

            if(_loadedAssets.TryGetValue(name, out var value) && value != null)
            {
                _logger?.Error($"The following asset was found but the key already has an entry: {name}");
                continue;
            }
            
            _loadedAssets[name] = asset;
            _logger?.Msg($"Successfully loaded: {name} ({asset.GetType()})");
        }
    }
}