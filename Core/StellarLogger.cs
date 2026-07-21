using MelonLoader;

namespace StellarModdingAPI.Core;

public static class StellarLogger
{
    private static MelonLogger.Instance? _logger;

    public static void Initialize(MelonLogger.Instance logger)
    {
        _logger = logger;
    }

    public static void Msg(string message)
    {
        _logger?.Msg(message);
    }

    public static void Warning(string message)
    {
        _logger?.Warning(message);
    }

    public static void Error(string message)
    {
        _logger?.Error(message);
    }
}