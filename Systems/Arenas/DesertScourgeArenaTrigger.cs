using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class DesertScourgeArenaTrigger : GlobalNPC
{
    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !IsDesertScourgeHead(npc))
        {
            return;
        }

        ModContent.GetInstance<ArenaSessionSystem>().TryStartSession(npc);
    }

    private static bool IsDesertScourgeHead(NPC npc)
    {
        return ModLoader.TryGetMod("CalamityMod", out Mod calamity) &&
               calamity.TryFind<ModNPC>("DesertScourgeHead", out ModNPC desertScourgeHead) &&
               npc.type == desertScourgeHead.Type;
    }
}
