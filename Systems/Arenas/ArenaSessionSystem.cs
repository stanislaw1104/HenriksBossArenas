using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaSessionSystem : ModSystem
{
    private readonly ArenaSessionState state = new();
    private readonly ArenaParticipantRoster participants = new();
    private bool pendingDefeatReturn;

    public bool HasActiveSession => state.IsActive;

    public bool TryStartSession(NPC boss)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || state.IsActive)
        {
            ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
                $"Arena session ignored boss spawn: npc={boss.whoAmI}, type={boss.type}, activeSession={state.IsActive}.");
            return false;
        }

        if (!state.TryBegin(boss.whoAmI, boss.type))
        {
            return false;
        }

        ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
            $"Arena session started: sourceBoss={boss.whoAmI}, type={boss.type}.");

        bool transitionStarted = false;
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
            {
                continue;
            }

            participants.Add(player.name);
            transitionStarted |= DesertScourgeArenaSubworld.MovePlayerToArena(player.whoAmI);
        }

        if (!transitionStarted)
        {
            EndSession("no player transition started");
            return false;
        }

        state.WaitForArenaBoss();
        boss.active = false;
        boss.netUpdate = true;
        NetMessage.SendData(MessageID.SyncNPC, number: boss.whoAmI);
        return true;
    }

    public bool TryTrackArenaBoss(NPC boss)
    {
        bool tracked = state.TryTrackArenaBoss(boss.whoAmI, boss.type);
        ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
            $"Arena boss tracking {(tracked ? "accepted" : "rejected")}: npc={boss.whoAmI}, type={boss.type}, activeSession={state.IsActive}.");
        return tracked;
    }

    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.SinglePlayer && pendingDefeatReturn && !DesertScourgeArenaSubworld.IsActive())
        {
            Player player = Main.LocalPlayer;
            if (player.active && !player.dead)
            {
                player.Teleport(new Vector2(Main.spawnTileX * 16f, Main.spawnTileY * 16f));
                pendingDefeatReturn = false;
            }
        }

        if (Main.netMode == NetmodeID.MultiplayerClient || !state.IsActive)
        {
            return;
        }

        NPC boss = Main.npc[state.BossWhoAmI];
        if (state.IsAwaitingArenaBoss)
        {
            return;
        }

        if (!state.IsTracking(boss.whoAmI, boss.type) || !boss.active)
        {
            bool hasLivingParticipants = HasLivingParticipants();
            if (DesertScourgeArenaSubworld.IsActive() && ArenaDefeatEvaluator.IsDefeat(boss.active, hasLivingParticipants))
            {
                ReturnAfterDefeat();
                return;
            }

            if (DesertScourgeArenaSubworld.IsActive() && ArenaDefeatEvaluator.IsVictory(boss.active, hasLivingParticipants))
            {
                DesertScourgeArenaSubworld.SpawnReturnPortal();
                EndSession("arena boss defeated; return portal opened");
                return;
            }

            EndSession($"tracked boss inactive or replaced: npc={boss.whoAmI}, type={boss.type}, active={boss.active}");
        }
    }

    public override void PreSaveAndQuit()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient && state.IsActive)
        {
            EndSession();
        }
    }

    private void EndSession(string reason = "session cleanup")
    {
        if (state.IsActive)
        {
            ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
                $"Arena session ended: {reason}.");
        }

        participants.Clear();
        state.Clear();
    }

    private void ReturnAfterDefeat()
    {
        pendingDefeatReturn = true;
        EndSession("all participants died and the arena boss left");
        DesertScourgeArenaSubworld.Exit();
    }

    private static bool HasLivingParticipants()
    {
        foreach (Player player in Main.ActivePlayers)
        {
            if (!player.dead)
            {
                return true;
            }
        }

        return false;
    }
}
