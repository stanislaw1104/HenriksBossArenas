using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HenriksBossArenas.Systems.Arenas;

public static class DesertScourgeArenaBuilder
{
    public static void Build(Rectangle site)
    {
        const int floorOffset = 8;
        for (int relativeX = 0; relativeX < site.Width; relativeX++)
        {
            for (int relativeY = 0; relativeY < site.Height; relativeY++)
            {
                int x = site.Left + relativeX;
                int y = site.Top + relativeY;
                Main.tile[x, y].ClearEverything();

                if (ArenaLayout.IsBoundary(relativeX, relativeY, site.Width, site.Height, floorOffset))
                {
                    WorldGen.PlaceTile(x, y, TileID.SandstoneBrick, mute: true, forced: true);
                }
            }
        }

        NetMessage.SendTileSquare(-1, site.Center.X, site.Center.Y, site.Width, site.Height);
    }
}
