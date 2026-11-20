using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaSiteFinder
{
    public const int Width = 180;
    public const int Height = 90;
    private const int WorldMargin = 50;
    private static readonly int[] HorizontalOffsets = [300, -300, 600, -600, 900, -900];

    public static bool TryFindDesertSite(Player initiator, out Rectangle site)
    {
        site = Rectangle.Empty;
        if (!initiator.ZoneDesert)
        {
            return false;
        }

        int playerTileX = (int)(initiator.Center.X / 16f);
        int playerTileY = (int)(initiator.Center.Y / 16f);
        foreach (int offset in HorizontalOffsets)
        {
            var candidate = new Rectangle(playerTileX + offset - Width / 2, playerTileY - Height / 2, Width, Height);
            if (ArenaBounds.IsInsideWorld(candidate.Left, candidate.Top, candidate.Width, candidate.Height, Main.maxTilesX, Main.maxTilesY, WorldMargin) &&
                HasDesertTerrain(candidate))
            {
                site = candidate;
                return true;
            }
        }

        return false;
    }

    private static bool HasDesertTerrain(Rectangle site)
    {
        int desertTiles = 0;
        for (int x = site.Left; x < site.Right; x += 6)
        {
            for (int y = site.Top; y < site.Bottom; y += 6)
            {
                ushort tileType = Main.tile[x, y].TileType;
                if (tileType is TileID.Sand or TileID.HardenedSand or TileID.Sandstone)
                {
                    desertTiles++;
                }
            }
        }

        return desertTiles >= 100;
    }
}
