using Terraria;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class DesertScourgeArenaSceneEffect : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle =>
        ModContent.GetInstance<DesertScourgeArenaBackgroundStyle>();

    public override bool IsSceneEffectActive(Player player) => DesertScourgeArenaSubworld.IsActive();
}
