using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace HenriksBossArenas.Systems.Arenas;

public static class DesertScourgeArenaSubworld
{
    private const string SubworldLibraryModName = "SubworldLibrary";
    private static string? id;

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
            (Action)SpawnDesertScourge);
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

    private static List<GenPass> CreateTasks() => [new PassLegacy("Desert Scourge Arena", GenerateArena)];

    private static void GenerateArena(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Building Desert Scourge arena";
        int floorY = ArenaSubworldLayout.SpawnTileY + 8;

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = floorY; y < Main.maxTilesY; y++)
            {
                Main.tile[x, y].ResetToType(TileID.Sand);
            }

            Main.tile[x, floorY].ResetToType(TileID.SandstoneBrick);
        }

        for (int y = 0; y <= floorY; y++)
        {
            Main.tile[0, y].ResetToType(TileID.SandstoneBrick);
            Main.tile[Main.maxTilesX - 1, y].ResetToType(TileID.SandstoneBrick);
        }

        Main.spawnTileX = ArenaSubworldLayout.SpawnTileX;
        Main.spawnTileY = ArenaSubworldLayout.SpawnTileY;
    }

    private static void SpawnDesertScourge()
    {
        if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient || !ModLoader.TryGetMod("CalamityMod", out Mod calamity) ||
            !calamity.TryFind<ModNPC>("DesertScourgeHead", out ModNPC desertScourgeHead))
        {
            return;
        }

        NPC.NewNPC(
            new Terraria.DataStructures.EntitySource_Misc("HenriksBossArenas:DesertScourgeArena"),
            ArenaSubworldLayout.SpawnTileX * 16,
            (ArenaSubworldLayout.SpawnTileY - 20) * 16,
            desertScourgeHead.Type);
    }
}
