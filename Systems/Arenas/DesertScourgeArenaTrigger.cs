using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class DesertScourgeArenaTrigger : GlobalNPC
{
    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        LogArenaBossHit(npc, $"item={item.type}, damage={damageDone}, player={player.whoAmI}");
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        LogArenaBossHit(npc, $"projectile={projectile.type}, damage={damageDone}, owner={projectile.owner}");
    }

    public override bool PreKill(NPC npc)
    {
        if (DesertScourgeArenaSubworld.IsActive() && IsDesertScourgeHead(npc))
        {
            ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
                $"Arena boss PreKill: npc={npc.whoAmI}, life={npc.life}, realLife={npc.realLife}, " +
                $"ai=({npc.ai[0]},{npc.ai[1]},{npc.ai[2]},{npc.ai[3]}).");
        }

        return true;
    }

    public override bool PreAI(NPC npc)
    {
        if (DesertScourgeArenaSubworld.IsActive() && IsDesertScourgeHead(npc))
        {
            Player target = Main.player[npc.target];
            int linkedNpcIndex = (int)npc.ai[0];
            NPC linkedNpc = linkedNpcIndex >= 0 && linkedNpcIndex < Main.maxNPCs ? Main.npc[linkedNpcIndex] : Main.npc[0];
            ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
                $"Arena boss PreAI: npc={npc.whoAmI}, active={npc.active}, life={npc.life}, timeLeft={npc.timeLeft}, " +
                $"target={npc.target}, targetActive={target.active}, targetDead={target.dead}, " +
                $"zoneDesert={target.ZoneDesert}, zoneBeach={target.ZoneBeach}, zenith={Main.zenithWorld}, " +
                $"link={linkedNpcIndex}, linkActive={linkedNpc.active}, linkType={linkedNpc.type}, linkLife={linkedNpc.life}.");
        }

        return true;
    }

    public override void PostAI(NPC npc)
    {
        if (DesertScourgeArenaSubworld.IsActive() && IsDesertScourgeHead(npc))
        {
            ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
                $"Arena boss PostAI: npc={npc.whoAmI}, active={npc.active}, life={npc.life}, timeLeft={npc.timeLeft}, target={npc.target}.");
        }
    }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !IsDesertScourgeHead(npc))
        {
            return;
        }

        bool isArenaSubworld = DesertScourgeArenaSubworld.IsActive();
        ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
            $"Desert Scourge OnSpawn: npc={npc.whoAmI}, active={npc.active}, source={source.GetType().Name}, " +
            $"zoneDesert={Main.LocalPlayer.ZoneDesert}, arenaSubworld={isArenaSubworld}.");

        ArenaSessionSystem session = ModContent.GetInstance<ArenaSessionSystem>();
        if (isArenaSubworld)
        {
            session.TryTrackArenaBoss(npc);
            return;
        }

        session.TryStartSession(npc);
    }

    private static bool IsDesertScourgeHead(NPC npc)
    {
        return ModLoader.TryGetMod("CalamityMod", out Mod calamity) &&
               calamity.TryFind<ModNPC>("DesertScourgeHead", out ModNPC desertScourgeHead) &&
                npc.type == desertScourgeHead.Type;
    }

    private static void LogArenaBossHit(NPC npc, string details)
    {
        if (DesertScourgeArenaSubworld.IsActive() && IsDesertScourgeHead(npc))
        {
            ModContent.GetInstance<HenriksBossArenas>().Logger.Info($"Arena boss hit: npc={npc.whoAmI}, {details}.");
        }
    }
}
