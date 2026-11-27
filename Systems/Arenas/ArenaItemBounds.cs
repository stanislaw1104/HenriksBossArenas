namespace HenriksBossArenas.Systems.Arenas;

public readonly record struct ArenaItemClampResult(
    float CenterX,
    float CenterY,
    bool WasClampedX,
    bool WasClampedY);

public static class ArenaItemBounds
{
    public static ArenaItemClampResult ClampCenter(float centerX, float centerY, int width, int height)
    {
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        float clampedCenterX = float.Clamp(
            centerX,
            (ArenaSubworldLayout.BoxLeftTileX + 1) * 16f + halfWidth,
            ArenaSubworldLayout.BoxRightTileX * 16f - halfWidth);
        float clampedCenterY = float.Clamp(
            centerY,
            (ArenaSubworldLayout.BoxCeilingTileY + 1) * 16f + halfHeight,
            ArenaSubworldLayout.FloorTileY * 16f - halfHeight);

        return new ArenaItemClampResult(
            clampedCenterX,
            clampedCenterY,
            clampedCenterX != centerX,
            clampedCenterY != centerY);
    }
}
