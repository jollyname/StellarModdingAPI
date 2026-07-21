using Items.Interface.Model;
using StellarModdingAPI.Core;
using System.Collections.Generic;
using UnityEngine;

namespace StellarModdingAPI.Items;

public static class ItemRegistry
{
    private static readonly Dictionary<uint, ItemSettings> _items = new();

    public static void Initialize()
    {
        _items.Clear();

        foreach (var item in Resources.FindObjectsOfTypeAll<ItemSettings>())
        {
            _items[item.id] = item;
        }

        StellarLogger.Msg($"Loaded {_items.Count} items");
    }

    public static ItemSettings Get(uint id)
    {
        if (!_items.TryGetValue(id, out var item))
        {
            throw new KeyNotFoundException($"No item with ID {id} was found.");
        }

        return item;
    }

    public static IReadOnlyDictionary<uint, ItemSettings> GetAll()
    {
        return _items;
    }
}