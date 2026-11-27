namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaPlayerBounds
{
    public static float ClampCenterX(float centerX, int width)
    {
        float halfWidth = width / 2f;
        float minCenterX = (ArenaSubworldLayout.BoxLeftTileX + 1) * 16f + halfWidth;
        float maxCenterX = ArenaSubworldLayout.BoxRightTileX * 16f - halfWidth;
        return float.Clamp(centerX, minCenterX, maxCenterX);
    }

    public static float ClampCenterY(float centerY, int height)
    {
        float halfHeight = height / 2f;
        float minCenterY = (ArenaSubworldLayout.BoxCeilingTileY + 1) * 16f + halfHeight;
        float maxCenterY = ArenaSubworldLayout.FloorTileY * 16f - halfHeight;
        return float.Clamp(centerY, minCenterY, maxCenterY);
    }
}
