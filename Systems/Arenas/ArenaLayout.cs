namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaLayout
{
    public static bool IsBoundary(int relativeX, int relativeY, int width, int height, int floorOffset)
    {
        int floorY = height - floorOffset - 1;
        return relativeX == 0 || relativeX == width - 1 || relativeY == 0 || relativeY == floorY;
    }
}
