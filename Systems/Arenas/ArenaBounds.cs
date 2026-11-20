namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaBounds
{
    public static bool IsInsideWorld(int left, int top, int width, int height, int maxTilesX, int maxTilesY, int margin)
    {
        if (width <= 0 || height <= 0 || margin < 0)
        {
            return false;
        }

        return left > margin &&
               top > margin &&
               left + width < maxTilesX - margin &&
               top + height < maxTilesY - margin;
    }
}
