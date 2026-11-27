using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaWorldProtection : GlobalTile
{
    public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged) =>
        !DesertScourgeArenaSubworld.IsActive();

    public override bool CanExplode(int i, int j, int type) =>
        !DesertScourgeArenaSubworld.IsActive();
}
