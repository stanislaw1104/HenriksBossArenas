using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaSessionSystem : ModSystem
{
    private readonly ArenaSessionState state = new();
    private readonly ArenaParticipantRoster participants = new();

    public bool HasActiveSession => state.IsActive;

    public bool TryStartSession(NPC boss)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || state.IsActive)
        {
            return false;
        }

        if (!state.TryBegin(boss.whoAmI, boss.type))
        {
            return false;
        }

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
            EndSession();
            return false;
        }

        boss.active = false;
        boss.netUpdate = true;
        NetMessage.SendData(MessageID.SyncNPC, number: boss.whoAmI);
        return true;
    }

    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !state.IsActive)
        {
            return;
        }

        NPC boss = Main.npc[state.BossWhoAmI];
        if (!state.IsTracking(boss.whoAmI, boss.type) || !boss.active)
        {
            EndSession();
        }
    }

    public override void PreSaveAndQuit()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient && state.IsActive)
        {
            EndSession();
        }
    }

    public override void OnWorldUnload()
    {
        EndSession();
    }

    private void EndSession()
    {
        participants.Clear();
        state.Clear();
    }
}
