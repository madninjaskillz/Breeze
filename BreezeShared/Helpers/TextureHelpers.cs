using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UAP
using Windows.Storage;
#endif
using Breeze.Helpers;
using Breeze.Storage.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.Helpers
{
    public static class TextureHelpers
    {
        public static Rectangle ToRectangle(this FloatRectangle? woot)
        {
            if (!woot.HasValue)
            {
                return Solids.Instance.Bounds;
            }

            return woot.Value.ToRectangle;
        }

        public static Texture2D LoadTexture(this GraphicsDevice gd, string filename)
        {
#if WINDOWS_UAP
            if (Solids.Instance.Storage.DatfileStorage != null)
            {
                return Texture2D.FromStream(gd, Solids.Instance.Storage.DatfileStorage.GetStream(filename));
            }

            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string xx = Solids.Instance.ContentPathRelative.GetFilePath(filename);
            using (Stream fs = folder.OpenStreamForReadAsync(xx).Result)
            {
                return Texture2D.FromStream(gd, fs);
            }
#else
            if (Solids.DatfileStorage != null)
            {
                return Texture2D.FromStream(gd, Solids.DatfileStorage.GetStream(filename));
            }

            string folder = Solids.ContentPath.GetFile(filename);
            using (Stream fs = filename.ToStream().Result)
            {
                return Texture2D.FromStream(gd, fs);
            }
#endif
        }

        public static RenderTarget2D GetRenderTarget()
        {
            return new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, Solids.Instance.Bounds.Width, Solids.Instance.Bounds.Height);
        }
       
        public static Texture2D ConvertToTexture(this RenderTarget2D renderTarget2D, RenderTarget2D target2D)
        {
            int width = Solids.Instance.SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = Solids.Instance.SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;

            RenderTarget2D newrenderTarget2D = new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget2D);
            Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);

            using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, Solids.SamplerState))
            {
                Solids.Instance.SpriteBatch.Draw(renderTarget2D, Solids.Instance.Bounds, null, Color.White);
            }
            Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(target2D);
            //Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Black);
            return newrenderTarget2D;
        }


        public static Texture2D BlurTexture(this Texture2D tex, int range, RenderTarget2D target2D)
        {
            int width = Solids.Instance.SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = Solids.Instance.SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;

            RenderTarget2D renderTarget2D = new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget2D);
            Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Black);

            var effect = Solids.GaussianBlur.Effect;

            effect.Parameters["range"].SetValue(range);
            effect.Parameters["wifth"].SetValue((float)width);
            effect.Parameters["hight"].SetValue((float)height);

            // TODO: Add your drawing code here
            using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, effect))
            {
                Solids.Instance.SpriteBatch.Draw(tex, Solids.Instance.Bounds, null, Color.White);
            }

            
            Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(target2D);
            //Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Black);
            return renderTarget2D;
        }
        
        //public static Texture2D RenderToTarget(int width, int height, Action action)
        //{
        //    RenderTarget2D renderTarget2D = new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
        //    Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget2D);
        //    Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);

        //    Solids.Instance.SpriteBatch.ForceBegin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Solids.SamplerState);
        //    action();

        //    Solids.Instance.SpriteBatch.ForceEnd();
        //    Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(Solids.DefaultScreenTarget);
        //    //Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Black);
        //    return renderTarget2D;

        //}
        public static (Rectangle position, Rectangle? source) GetAdjustedDestAndSourceAfterClip(FloatRectangle? detination, Rectangle? sourceRectangle, FloatRectangle? clip)
        {
            
            {
                if (clip == null)
                {
                    return (detination.ToRectangle(), sourceRectangle);
                }

                Rectangle pos = (detination).ToRectangle();

                Rectangle clippedPos = detination.Value.Clamp(clip.Value).ToRectangle;
                Rectangle? fixedSource = sourceRectangle;

                if (sourceRectangle != null)
                {
                    int w = sourceRectangle.Value.Width;
                    int h = sourceRectangle.Value.Height;

                    int sw = pos.Width;
                    int sh = pos.Height;

                    int leftDif = clippedPos.X - pos.X;
                    int topDif = clippedPos.Y - pos.Y;

                    int rightDif = pos.Right - clippedPos.Right;
                    int bottomDif = pos.Bottom - clippedPos.Bottom;

                    float leftDifPerc = (float) leftDif / sw;
                    float rightDifPerc = (float) rightDif / sw;

                    float topDifPerc = (float) topDif / sh;
                    float bottomDifPerc = (float) bottomDif / sh;

                    int leftAdjust = (int) (w * leftDifPerc);
                    int topAdjust = (int) (h * topDifPerc);

                    int rightAdjust = (int) (w * rightDifPerc);
                    int bottomAdjust = (int) (h * bottomDifPerc);


                    int newX = (int) (sourceRectangle.Value.X + leftAdjust);
                    int newY = (int) (sourceRectangle.Value.Y + topAdjust);
                    int newWidth = (int) (w - rightAdjust - leftAdjust);
                    int newHeight = (int) (h - bottomAdjust - topAdjust);

                    //  int newWidth = newR - newX;
                    //  int newHeight = newB - newY;

                    fixedSource = new Rectangle(newX, newY, newWidth, newHeight);


                }

                return (clippedPos, fixedSource);
            }
        }
    }
}
