namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaSubworldId
{
    public static string Create(string modName, string subworldName) => $"{modName}/{subworldName}";
}
