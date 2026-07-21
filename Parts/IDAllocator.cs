using StellarModdingAPI.StellarDriveIntegration;

namespace StellarModdingAPI.Parts
{
    public static class IDAllocator
    {
        private static ushort _nextID = 3500;

        public static ushort GetPartID()
        {
            while (IntegrationUtilities.IsPartIDTaken(_nextID))
            {
                _nextID++;
            }

            return _nextID++;
        }
    }
}
