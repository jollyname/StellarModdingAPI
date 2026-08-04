namespace StellarModdingAPI.Items;

public readonly record struct ItemID(uint Value)
{
    public static implicit operator ItemID(uint id) => new(id);
    public static implicit operator uint(ItemID id) => id.Value;
}