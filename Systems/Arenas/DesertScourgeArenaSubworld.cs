using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using HenriksBossArenas.Projectiles;

namespace HenriksBossArenas.Systems.Arenas;

public static class DesertScourgeArenaSubworld
{
    private const string SubworldLibraryModName = "SubworldLibrary";
    private static string? id;
    private static readonly ArenaBossSpawnState bossSpawn = new();

    public static string Id => id ?? throw new InvalidOperationException("The Desert Scourge arena subworld is not registered.");

    public static void Register(Mod owner)
    {
        if (id is not null)
        {
            return;
        }

        if (!ModLoader.TryGetMod(SubworldLibraryModName, out Mod subworldLibrary))
        {
            throw new InvalidOperationException("Subworld Library is required to register the Desert Scourge arena.");
        }

        object result = subworldLibrary.Call(
            "Register",
            owner,
            "DesertScourgeArena",
            ArenaSubworldLayout.Width,
            ArenaSubworldLayout.Height,
            CreateTasks(),
            null,
            -1,
            false,
            false,
            false,
            false,
            null,
            null,
            (Action)UpdateArena,
            null,
            null,
            null,
            null,
            (Action)RequestDesertScourgeSpawn,
            null);
        string registeredName = result as string ?? throw new InvalidOperationException("Subworld Library did not register the Desert Scourge arena.");
        id = ArenaSubworldId.Create(owner.Name, registeredName);
    }

    public static bool MovePlayerToArena(int playerWhoAmI)
    {
        if (!ModLoader.TryGetMod(SubworldLibraryModName, out Mod subworldLibrary))
        {
            return false;
        }

        if (Main.netMode != Terraria.ID.NetmodeID.SinglePlayer)
        {
            return false;
        }

        return subworldLibrary.Call("Enter", Id) is true;
    }

    public static bool IsActive()
    {
        return ModLoader.TryGetMod(SubworldLibraryModName, out Mod subworldLibrary) &&
               subworldLibrary.Call("IsActive", Id) is true;
    }

    public static bool Exit()
    {
        return ModLoader.TryGetMod(SubworldLibraryModName, out Mod subworldLibrary) &&
               subworldLibrary.Call("Exit") is true;
    }

    public static void SpawnReturnPortal()
    {
        if (Main.netMode != Terraria.ID.NetmodeID.SinglePlayer)
        {
            return;
        }

        Projectile.NewProjectile(
            new Terraria.DataStructures.EntitySource_Misc("HenriksBossArenas:DesertScourgeArenaReturnPortal"),
            new Microsoft.Xna.Framework.Vector2(
                ArenaSubworldLayout.ReturnPortalTileX * 16,
                ArenaSubworldLayout.ReturnPortalTileY * 16),
            Microsoft.Xna.Framework.Vector2.Zero,
            ModContent.ProjectileType<ArenaReturnPortal>(),
            0,
            0f,
            Main.myPlayer);
    }

    private static List<GenPass> CreateTasks() => [new PassLegacy("Desert Scourge Arena", GenerateArena)];

    private static void GenerateArena(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Building Desert Scourge arena";

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Main.tile[x, y].ClearEverything();
            }
        }

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = ArenaSubworldLayout.FloorTileY + 1; y < Main.maxTilesY; y++)
            {
                Main.tile[x, y].ResetToType(TileID.Sand);
            }
        }

        for (int x = ArenaSubworldLayout.BoxLeftTileX; x <= ArenaSubworldLayout.BoxRightTileX; x++)
        {
            Main.tile[x, ArenaSubworldLayout.BoxCeilingTileY].ResetToType(TileID.SandstoneBrick);
            Main.tile[x, ArenaSubworldLayout.FloorTileY].ResetToType(TileID.SandstoneBrick);
        }

        for (int y = ArenaSubworldLayout.BoxCeilingTileY; y <= ArenaSubworldLayout.FloorTileY; y++)
        {
            Main.tile[ArenaSubworldLayout.BoxLeftTileX, y].ResetToType(TileID.SandstoneBrick);
            Main.tile[ArenaSubworldLayout.BoxRightTileX, y].ResetToType(TileID.SandstoneBrick);
        }

        Main.worldSurface = Main.maxTilesY - 42;
        Main.rockLayer = Main.maxTilesY;
        Main.spawnTileX = ArenaSubworldLayout.SpawnTileX;
        Main.spawnTileY = ArenaSubworldLayout.SpawnTileY;

        Tile floorTile = Main.tile[ArenaSubworldLayout.SpawnTileX, ArenaSubworldLayout.FloorTileY];
        ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
            $"Generated arena: spawn=({Main.spawnTileX},{Main.spawnTileY}), floor={ArenaSubworldLayout.FloorTileY}, " +
            $"floorHasTile={floorTile.HasTile}, floorType={floorTile.TileType}, worldSurface={Main.worldSurface}.");
    }

    private static void RequestDesertScourgeSpawn() => bossSpawn.Request();

    private static void UpdateArena()
    {
        if (bossSpawn.TryConsume(Main.LocalPlayer.active && !Main.LocalPlayer.dead))
        {
            SpawnDesertScourge();
        }
    }

    private static void SpawnDesertScourge()
    {
        if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient || !ModLoader.TryGetMod("CalamityMod", out Mod calamity) ||
            !calamity.TryFind<ModNPC>("DesertScourgeHead", out ModNPC desertScourgeHead))
        {
            return;
        }

        NPC.SpawnOnPlayer(Main.myPlayer, desertScourgeHead.Type);
        int bossWhoAmI = NPC.FindFirstNPC(desertScourgeHead.Type);
        ModContent.GetInstance<HenriksBossArenas>().Logger.Info(
            $"Spawned arena Desert Scourge: npc={bossWhoAmI}, type={desertScourgeHead.Type}, " +
            $"playerActive={Main.LocalPlayer.active}, playerZoneDesert={Main.LocalPlayer.ZoneDesert}.");
    }
}
