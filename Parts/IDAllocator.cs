using StellarModdingAPI.Integration;

namespace StellarModdingAPI.Parts;

// TODO: ensure ids are always mapped to the same parts
public static class IDAllocator
{
    private static readonly PartID InitialId = 3500;
    private static PartID _nextID = InitialId;

    public static PartID GetPartID()
    {
        while (PartIntegration.IsPartIDTaken(_nextID))
        {
            _nextID++;
        }

        return _nextID++;
    }

    // feels hacky but works for now
    public static void Reset()
    {
        _nextID = InitialId;
    }
}