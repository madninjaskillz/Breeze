using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.Helpers
{
    /// <summary>
    /// A Gaussian blur filter kernel class. A Gaussian blur filter kernel is
    /// perfectly symmetrical and linearly separable. This means we can split
    /// the full 2D filter kernel matrix into two smaller horizontal and
    /// vertical 1D filter kernel matrices and then perform the Gaussian blur
    /// in two passes. Contrary to what you might think performing the Gaussian
    /// blur in this way is actually faster than performing the Gaussian blur
    /// in a single pass using the full 2D filter kernel matrix.
    /// <para>
    /// The GaussianBlur class is intended to be used in conjunction with an
    /// HLSL Gaussian blur shader. The following code snippet shows a typical
    /// Effect file implementation of a Gaussian blur.
    /// <code>
    /// #define RADIUS  7
    /// #define KERNEL_SIZE (RADIUS * 2 + 1)
    ///
    /// float weights[KERNEL_SIZE];
    /// float2 offsets[KERNEL_SIZE];
    ///
    /// texture colorMapTexture;
    ///
    /// sampler2D colorMap = sampler_state
    /// {
    ///     Texture = <![CDATA[<colorMapTexture>;]]>
    ///     MipFilter = Linear;
    ///     MinFilter = Linear;
    ///     MagFilter = Linear;
    /// };
    ///
    /// float4 PS_GaussianBlur(float2 texCoord : TEXCOORD) : COLOR0
    /// {
    ///     float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);
    ///
    ///     <![CDATA[for (int i = 0; i < KERNEL_SIZE; ++i)]]>
    ///         color += tex2D(colorMap, texCoord + offsets[i]) * weights[i];
    /// 
    ///     return color;
    /// }
    /// 
    /// technique GaussianBlur
    /// {
    ///     pass
    ///     {
    ///         PixelShader = compile ps_2_0 PS_GaussianBlur();
    ///     }
    /// }
    /// </code>
    /// The RADIUS constant in the effect file must match the radius value in
    /// the GaussianBlur class. The effect file's weights global variable
    /// corresponds to the GaussianBlur class' kernel field. The effect file's
    /// offsets global variable corresponds to the GaussianBlur class'
    /// offsetsHoriz and offsetsVert fields.
    /// </para>
    /// </summary>
    public class GaussianBlur
    {
        private Game game;
        public Effect Effect;
        private int radius;
        private float amount;
        private float sigma;
        private float[] kernel;
        private Vector2[] offsetsHoriz;
        private Vector2[] offsetsVert;


        public GaussianBlur(Game game)
        {
            this.game = game;

            Effect = game.Content.Load<Effect>("BasicBlur");
        }
        public void DoBlur(Texture2D sprite, int blurAmount, float bGdim)
        {
            if (Effect == null) return;
            if (Solids.Settings.EnableBlur)
            {

                Effect.CurrentTechnique = Effect.Techniques["AcrylicBlur"];
                Effect.Parameters["gfxWidth"].SetValue((float)sprite.Width);
                Effect.Parameters["gfxHeight"].SetValue((float)sprite.Height);
                Effect.Parameters["blurSize"].SetValue(blurAmount);
                Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.TransparentBlack);
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, null, null, null))
                {
                    //Solids.Instance.SpriteBatch.ForceBegin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, Effect, null);

                    Solids.Instance.SpriteBatch.Draw(sprite, Solids.Bounds, null, Color.White * bGdim);
                }
            }
        }


        public void DoBlur(Texture2D sprite, int blurAmount, Color color, Rectangle rect, Rectangle? scissorRect, FloatRectangle? clip, int noisePerc = 0)
        {
            if (Effect == null) return;
            if (sprite == null)
            {
                return;
            }

            rect = new FloatRectangle(rect).Clamp(clip).ToRectangle;

            if (Solids.Settings.EnableBlur)
            {
                ////      Solids.Instance.SpriteBatch.DoEnd();
                //Solids.Instance.SpriteBatch.Scissor = scissorRect;
                ////      Solids.Instance.SpriteBatch.DoEnd();

                Effect.CurrentTechnique = Effect.Techniques["AcrylicBlur"];
                Effect.Parameters["gfxWidth"].SetValue((float)sprite.Width);
                Effect.Parameters["gfxHeight"].SetValue((float)sprite.Height);
                Effect.Parameters["blurSize"].SetValue((int)(blurAmount));
                if (noisePerc > 0)
                {
                    Effect.Parameters["noisePerc"].SetValue(noisePerc);
                }

                //Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.TransparentBlack);
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Solids.Instance.SpriteBatch.RasterizerState, Effect, null))
                {
                    Solids.Instance.SpriteBatch.Draw(sprite, rect, rect, color);
                }
            }
        }

        public void DoBlur(int blurAmount, Rectangle? scissorRect, FloatRectangle? clip, int noisePerc, Rectangle sprite, Action renderCode)
        {
            if (Effect == null) return;

            if (Solids.Settings.EnableBlur)
            {
                ////      Solids.Instance.SpriteBatch.DoEnd();
                //Solids.Instance.SpriteBatch.Scissor = scissorRect;
                ////      Solids.Instance.SpriteBatch.DoEnd();

                Effect.CurrentTechnique = Effect.Techniques["AcrylicBlur"];
                Effect.Parameters["gfxWidth"].SetValue((float)sprite.Width);
                Effect.Parameters["gfxHeight"].SetValue((float)sprite.Height);
                Effect.Parameters["blurSize"].SetValue((int)(blurAmount));
                if (noisePerc > 0)
                {
                    Effect.Parameters["noisePerc"].SetValue(noisePerc);
                }

                //Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.TransparentBlack);
                Solids.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Solids.Instance.SpriteBatch.RasterizerState, Effect, null);
                renderCode();
                Solids.Instance.SpriteBatch.End();
            }
        }

        public Effect StartBlur(int blurAmount, Rectangle? scissorRect, FloatRectangle? clip, int noisePerc, Rectangle sprite)
        {
            

            if (Solids.Settings.EnableBlur)
            {
                Effect.CurrentTechnique = Effect.Techniques["AcrylicBlur"];
                Effect.Parameters["gfxWidth"].SetValue((float)sprite.Width);
                Effect.Parameters["gfxHeight"].SetValue((float)sprite.Height);
                Effect.Parameters["blurSize"].SetValue((int)(blurAmount));
                if (noisePerc > 0)
                {
                    Effect.Parameters["noisePerc"].SetValue(noisePerc);
                }
            }

            return Effect;
        }





        public Texture2D PerformGaussianBlur(Texture2D srcTexture, SmartSpriteBatch spriteBatch)
        {

            RenderTarget2D renderTarget1 = new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, srcTexture.Width, srcTexture.Height, false, Solids.Instance.SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);

            game.GraphicsDevice.SetRenderTarget(renderTarget1);

            Effect.CurrentTechnique = Effect.Techniques["Technique1"];
            Effect.Parameters["wifth"].SetValue(srcTexture.Width);
            Effect.Parameters["hight"].SetValue(srcTexture.Height);

            using (new SmartSpriteBatchManager(spriteBatch, 0, BlendState.Opaque, null, null, null, Effect))
            {
                spriteBatch.Draw(srcTexture, new Rectangle(0, 0, srcTexture.Width, srcTexture.Height), Color.White);
            }

            game.GraphicsDevice.SetRenderTarget(null);
            return renderTarget1;
        }

    }
}
