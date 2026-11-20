using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaSessionSystem : ModSystem
{
    private readonly ArenaSessionState state = new();
    private readonly List<ArenaParticipant> participants = new();
    private ArenaTileSnapshot? snapshot;
    private Rectangle site;

    public bool HasActiveSession => state.IsActive;

    public bool TryStartSession(NPC boss)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || state.IsActive)
        {
            return false;
        }

        boss.TargetClosest();
        Player initiator = Main.player[boss.target];
        if (!initiator.active || !ArenaSiteFinder.TryFindDesertSite(initiator, out site) || !state.TryBegin(boss.whoAmI, boss.type))
        {
            return false;
        }

        snapshot = ArenaTileSnapshot.Capture(site);
        DesertScourgeArenaBuilder.Build(site);

        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
            {
                continue;
            }

            participants.Add(new ArenaParticipant(player.name, player.position));
            TeleportPlayerToArena(player);
        }

        boss.Center = GetArenaSpawnPosition();
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
        participants.Clear();
        snapshot = null;
        state.Clear();
    }

    private void EndSession()
    {
        foreach (ArenaParticipant participant in participants)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.name == participant.Name)
                {
                    player.Teleport(participant.OriginalPosition);
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                    break;
                }
            }
        }

        snapshot?.Restore();
        NetMessage.SendTileSquare(-1, site.Center.X, site.Center.Y, site.Width, site.Height);
        participants.Clear();
        snapshot = null;
        state.Clear();
    }

    private void TeleportPlayerToArena(Player player)
    {
        Vector2 spawnPosition = GetArenaSpawnPosition() + new Vector2((player.whoAmI % 8) * 48f, 0f);
        player.Teleport(spawnPosition);
        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
    }

    private Vector2 GetArenaSpawnPosition() => new((site.Left + site.Width / 2) * 16f, (site.Bottom - 12) * 16f);
}
