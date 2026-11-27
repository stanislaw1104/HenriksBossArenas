using Terraria;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaLootContainment : GlobalItem
{
    public override void PostUpdate(Item item)
    {
        if (!item.active || !DesertScourgeArenaSubworld.IsActive())
        {
            return;
        }

        ArenaItemClampResult clampResult = ArenaItemBounds.ClampCenter(
            item.Center.X,
            item.Center.Y,
            item.width,
            item.height);

        if (!clampResult.WasClampedX && !clampResult.WasClampedY)
        {
            return;
        }

        item.Center = new Microsoft.Xna.Framework.Vector2(clampResult.CenterX, clampResult.CenterY);

        if (clampResult.WasClampedX)
        {
            item.velocity.X = 0f;
        }

        if (clampResult.WasClampedY)
        {
            item.velocity.Y = 0f;
        }
    }
}
