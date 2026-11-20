using Terraria.ModLoader;
using HenriksBossArenas.Systems.Arenas;

namespace HenriksBossArenas;

public sealed class HenriksBossArenas : Mod
{
    public override void Load()
    {
        DesertScourgeArenaSubworld.Register(this);
    }
}
