namespace StellarModdingAPI.Integration;

public readonly record struct SceneInfo(int BuildIndex);

public static class SceneIntegration
{
    public static SceneInfo MenuInfo = new(BuildIndex: 0);
    public static SceneInfo GameInfo = new(BuildIndex: 1);
}