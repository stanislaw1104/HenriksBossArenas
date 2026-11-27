namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaSubworldLayout
{
    public const int Width = 1200;
    public const int Height = 300;
    public const int BoxLeftTileX = Width / 2 - 70;
    public const int BoxRightTileX = Width / 2 + 69;
    public const int BoxCeilingTileY = 80;
    public const int FloorTileY = Height - 120;
    public const int SpawnTileX = Width / 2;
    public const int SpawnTileY = Height - 124;
    public const int ReturnPortalTileX = BoxRightTileX - 8;
    public const int ReturnPortalTileY = FloorTileY - 2;

    public static bool IsSpawnInsideWorld() =>
        SpawnTileX > 0 && SpawnTileX < Width - 1 && SpawnTileY > 0 && SpawnTileY < Height - 1;
}
