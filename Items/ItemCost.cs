using Items.Interface.Model;

namespace StellarModdingAPI.Items;

public static class ItemCost
{
    public static ItemInstance Of(ItemID item, int amount)
    {
        return new ItemInstance(ItemRegistry.Get(item), amount);
    }
}