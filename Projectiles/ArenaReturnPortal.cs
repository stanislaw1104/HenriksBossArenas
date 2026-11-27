using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HenriksBossArenas.Systems.Arenas;

namespace HenriksBossArenas.Projectiles;

public sealed class ArenaReturnPortal : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PortalGunGate}";

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 48;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 36000;
    }

    public override void AI()
    {
        if (Main.netMode != NetmodeID.SinglePlayer || !Main.LocalPlayer.active || Main.LocalPlayer.dead ||
            !Projectile.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
        {
            return;
        }

        Projectile.active = false;
        DesertScourgeArenaSubworld.Exit();
    }
}
