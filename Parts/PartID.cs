namespace StellarModdingAPI.Parts;

public readonly record struct PartID(ushort Value)
{
    public static implicit operator ushort(PartID id) => id.Value;
    public static implicit operator PartID(ushort id) => new(id);
}