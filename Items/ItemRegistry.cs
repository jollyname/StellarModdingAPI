using Items.Interface.Model;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;

namespace StellarModdingAPI.Items;

public static class ItemRegistry
{
    private static readonly Dictionary<ItemID, ItemSettings> _items = [];

    public static void Refresh()
    {
        _items.Clear();

        foreach (var item in Resources.FindObjectsOfTypeAll<ItemSettings>())
        {
            _items[item.id] = item;
        }

        MelonLogger.Msg($"Loaded {_items.Count} items");
    }

    public static ItemSettings Get(ItemID id)
    {
        if (!_items.TryGetValue(id, out var item))
        {
            throw new KeyNotFoundException($"No item with ID {id} was found.");
        }

        return item;
    }

    public static IReadOnlyDictionary<ItemID, ItemSettings> GetAll()
    {
        return _items;
    }
}