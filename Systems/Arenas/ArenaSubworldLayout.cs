namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaSubworldLayout
{
    public const int Width = 600;
    public const int Height = 300;
    public const int SpawnTileX = Width / 2;
    public const int SpawnTileY = Height - 45;

    public static bool IsSpawnInsideWorld() =>
        SpawnTileX > 0 && SpawnTileX < Width - 1 && SpawnTileY > 0 && SpawnTileY < Height - 1;
}
