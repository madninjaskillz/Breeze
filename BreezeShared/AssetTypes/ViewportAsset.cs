using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ezmuzeMono.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ezmuzeMono.AssetTypes
{
    public class ViewportAsset  : KeyedUpdatedAsset
    {
        public BaseScreen BaseScreen { get; set; }
        public BaseScreen ParentScreen { get; set; }

        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null)
        {

            int y = (int)BaseScreen.AllAssets.Max(t => t.Position.Bottom);
            var x = (int)BaseScreen.AllAssets.Max(t => t.Position.Right);

            if (BaseScreen.RenderTarget == null || BaseScreen.RenderTarget.Width != (int)x || BaseScreen.RenderTarget.Height != (int)y)
            {
                BaseScreen.RenderTarget = new RenderTarget2D(Solids.SpriteBatch.GraphicsDevice, x,y);
            }

            Solids.SpriteBatch.GraphicsDevice.SetRenderTarget(BaseScreen.RenderTarget);
            Solids.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);
            //  Solids.SpriteBatch.ForceBegin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, null);
            using (new SmartSpriteBatchManager(Solids.SpriteBatch))
            {
                BaseScreen.Draw(new GameTime(), BaseScreen.OpenCloseOpacity, null);
            }

            Solids.SpriteBatch.GraphicsDevice.SetRenderTarget(ParentScreen.RenderTarget);

            //  Solids.SpriteBatch.ForceEnd();

            //previousTexture = screen.RenderTarget;

            base.Draw(spriteBatch, screen, opacity, clip, bgTexture);
        }


        public void Update(ViewportAsset asset)
        {
            //foreach (var VARIABLE in asset.BaseScreen.)
            //{
            //    this = asset;
            //}
        }
    }
}
