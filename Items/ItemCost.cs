using Items.Interface.Model;

namespace StellarModdingAPI.Items;

public static class ItemCost
{
    public static ItemInstance Of(uint id, int amount)
    {
        return new ItemInstance(ItemRegistry.Get(id), amount);
    }
}