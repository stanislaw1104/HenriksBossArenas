using Microsoft.Xna.Framework;
using Terraria;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaTileSnapshot
{
    private readonly Rectangle site;
    private readonly Tile[,] tiles;

    private ArenaTileSnapshot(Rectangle site)
    {
        this.site = site;
        tiles = new Tile[site.Width, site.Height];
    }

    public static ArenaTileSnapshot Capture(Rectangle site)
    {
        var snapshot = new ArenaTileSnapshot(site);
        for (int x = 0; x < site.Width; x++)
        {
            for (int y = 0; y < site.Height; y++)
            {
                var tile = new Tile();
                tile.CopyFrom(Main.tile[site.X + x, site.Y + y]);
                snapshot.tiles[x, y] = tile;
            }
        }

        return snapshot;
    }

    public void Restore()
    {
        for (int x = 0; x < site.Width; x++)
        {
            for (int y = 0; y < site.Height; y++)
            {
                Main.tile[site.X + x, site.Y + y].CopyFrom(tiles[x, y]);
            }
        }
    }
}
