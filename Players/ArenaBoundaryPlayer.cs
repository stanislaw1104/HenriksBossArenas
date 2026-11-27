using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HenriksBossArenas.Systems.Arenas;

namespace HenriksBossArenas.Players;

public sealed class ArenaBoundaryPlayer : ModPlayer
{
    public override void PostUpdate()
    {
        if (!DesertScourgeArenaSubworld.IsActive())
        {
            return;
        }

        Vector2 clampedCenter = new(
            ArenaPlayerBounds.ClampCenterX(Player.Center.X, Player.width),
            ArenaPlayerBounds.ClampCenterY(Player.Center.Y, Player.height));

        if (clampedCenter != Player.Center)
        {
            Player.Center = clampedCenter;
            Player.velocity = Vector2.Zero;
        }
    }
}
