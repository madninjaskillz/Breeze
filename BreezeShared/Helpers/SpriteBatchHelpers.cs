using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze;
using Breeze.Screens;
using Breeze.AssetTypes;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.Helpers
{
    public static class SpriteBatchHelpers
    {
        public static void DrawBorder(this SmartSpriteBatch spriteBatch, FloatRectangle rect, Color color, int brushSize, FloatRectangle? clip)
        {
            for (int x = 0; x < brushSize; x++)
            {
                for (int y = 0; y < brushSize; y++)
                {
                    FloatRectangle r = new FloatRectangle(rect.X + x, rect.Y + y, rect.Width - x - x, rect.Height - y - y);
                    spriteBatch.DrawRectangle(r, color, clip: clip);
                }
            }
        }

        public static void DrawTexturedRectangle(this SmartSpriteBatch spriteBatch, FloatRectangle rect, Color color, string fillTexture, FloatRectangle? clip = null)
        {
            DrawTexturedRectangle(spriteBatch, rect, color, Solids.Instance.AssetLibrary.GetTexture(fillTexture), clip: clip);
        }

        public static void DrawTexturedRectangle(this SmartSpriteBatch spriteBatch, FloatRectangle trect, Color color, Texture2D fillTexture2D, TileMode tileMode = TileMode.JustStretch, FloatRectangle? clip = null)
        {
           // using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
            {
                FloatRectangle rect = trect.Clip(clip);

                switch (tileMode)
                {
                    case (TileMode.Tile):
                    {

                        for (int y = 0; y < rect.Height; y = y + fillTexture2D.Height)
                        {
                            for (int x = 0; x < rect.Width; x = x + fillTexture2D.Width)
                            {
                                float twidth = fillTexture2D.Width;
                                float theight = fillTexture2D.Height;

                                float rX = x + rect.X;
                                float rY = y + rect.Y;

                                if (x + twidth > rect.Width)
                                {
                                    twidth = fillTexture2D.Width - ((x + twidth) - rect.Width);
                                }

                                if (y + theight > rect.Height)
                                {
                                    theight = fillTexture2D.Height - ((y + theight) - rect.Height);
                                }

                                Rectangle sourceRectangle = new Rectangle(0, 0, (int) twidth, (int) theight);
                                Rectangle destRectangle = new Rectangle((int) rX, (int) rY, (int) twidth, (int) theight);

                                spriteBatch.Draw(fillTexture2D, destRectangle, sourceRectangle, color);
                            }
                        }
                        break;
                    }

                    case (TileMode.JustStretch):
                    {
                        spriteBatch.Draw(fillTexture2D, rect.ToRectangle, color);
                        break;
                    }

                    case (TileMode.StretchToFill):
                    {

                        float rectAspectRatio = (float) rect.Width / (float) rect.Height;
                        float aspectRatio = (float) fillTexture2D.Width / (float) fillTexture2D.Height;
                        float sw = 0;
                        float sh = 0;
                        int xo = 0;
                        int yo = 0;
                        sw = fillTexture2D.Width;
                        sh = sw / rectAspectRatio;
                        yo = (int) ((fillTexture2D.Height - sh) / 2f);

                        if (sh > fillTexture2D.Height)
                        {
                            sh = fillTexture2D.Height;
                            sw = sh * rectAspectRatio;

                            yo = 0;
                            xo = (int) ((fillTexture2D.Width - sw) / 2f);

                        }

                        Rectangle sourceRect = new Rectangle((int) xo, (int) yo, (int) sw, (int) sh);

                        spriteBatch.Draw(fillTexture2D, rect.ToRectangle, sourceRect, color);
                        break;
                    }
                }
            }

        }
    }
}
