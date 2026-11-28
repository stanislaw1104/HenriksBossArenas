namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaBackgroundTiling
{
    public static float GetSingleBackgroundX(float relativeCameraX, int screenWidth, int backgroundWidth, float parallax)
    {
        float centeredX = (screenWidth - backgroundWidth) / 2f - relativeCameraX * parallax;
        return float.Clamp(centeredX, screenWidth - backgroundWidth, 0f);
    }
}
