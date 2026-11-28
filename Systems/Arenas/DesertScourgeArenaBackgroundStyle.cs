using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class DesertScourgeArenaBackgroundStyle : ModSurfaceBackgroundStyle
{
    private const string TexturePath = "HenriksBossArenas/Assets/Backgrounds/DesertScourgeArena";
    private const float Parallax = 0.1f;

    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        fades[Slot] = 1f;
    }

    public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
    {
        if (!DesertScourgeArenaSubworld.IsActive())
        {
            return true;
        }

        Texture2D texture = ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;
        float scale = (float)System.Math.Max(Main.screenWidth / (double)texture.Width, Main.screenHeight / (double)texture.Height);
        int drawWidth = (int)(texture.Width * scale);
        int drawHeight = (int)(texture.Height * scale);
        float relativeCameraX = Main.screenPosition.X - ArenaSubworldLayout.BoxLeftTileX * 16f;
        int drawX = (int)ArenaBackgroundTiling.GetSingleBackgroundX(relativeCameraX, Main.screenWidth, drawWidth, Parallax);
        int drawY = (Main.screenHeight - drawHeight) / 2;

        spriteBatch.Draw(texture, new Rectangle(drawX, drawY, drawWidth, drawHeight), Color.White);

        return false;
    }
}
