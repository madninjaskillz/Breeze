using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Breeze;
using Breeze.FontSystem;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze
{
    public static class BreezeDebug
    {
        public static List<string> ConsoleHistory = new List<string>();

        public static void WriteLine(object obj)
        {
            if (obj != null)
            {
                ConsoleHistory.AddRange(obj.ToString().Replace("\r", "").Split('\n'));
            }
        }
    }
    public class DebugSystem
    {
        private int pointer = 0;
        public List<DebugObject> DebugObjects = new List<DebugObject>();

        public void SetValue(string key, float value)
        {
            if (DebugObjects.Any(t => t.Key == key))
            {
                DebugObjects.First(t => t.Key == key).CurrentValue = value;
            }
            else
            {
                DebugObjects.Add(new DebugObject()
                {
                    Key = key,
                    CurrentValue = value
                });
            }
        }

        public void Draw(SmartSpriteBatch spriteBatch, bool showDebug)
        {

            BMFont font = Solids.Instance.AssetLibrary.GetFont("consolas");

            if (font != null)
            {
                if (showDebug)
                {
                    var bounds = spriteBatch.GraphicsDevice.Viewport.Bounds;
                    spriteBatch.DrawSolidRectangle(new FloatRectangle(0, 0, bounds.Width, (int)(bounds.Height * 0.8f)), Color.CornflowerBlue * 0.75f);

                }
                var scale = new Vector2(0.75f, 0.75f);
                int mxh = 0;
                int mxw = 0;
                foreach (var i in DebugObjects)
                {
                    var r = font.MeasureString(i.CurrentText) * scale;
                    if (r.X > mxw) mxw = (int)r.X;
                    if (r.Y > mxh) mxh = (int)r.Y;
                }
                int width = 100 + mxw + 20;

                if (showDebug)
                {
                    spriteBatch.DrawSolidRectangle(new FloatRectangle(spriteBatch.GraphicsDevice.Viewport.Bounds.Width - (int)width - 20, 2, (int)width, (int)mxh * DebugObjects.Count), Color.Black);
                }

                int ct = 0;
                float mx = 1;

                if (showDebug)
                {
                    Vector2 fontscale = new Vector2(0.4f, 0.4f);
                    int ps = 50;
                    font.DrawText(spriteBatch, new Vector2(10, 20), "Key", Color.White, fontscale);
                    font.DrawText(spriteBatch, new Vector2(300, 20), "Runs", Color.White, fontscale);
                    font.DrawText(spriteBatch, new Vector2(450, 20), "Avg", Color.White, fontscale);
                    font.DrawText(spriteBatch, new Vector2(600, 20), "Min", Color.White, fontscale);
                    font.DrawText(spriteBatch, new Vector2(750, 20), "Max", Color.White, fontscale);
                    font.DrawText(spriteBatch, new Vector2(900, 20), "Total", Color.White, fontscale);
                    foreach (KeyValuePair<string, BenchMarkDetails> benchMarkDetailse in BenchMarkProvider.BenchMarks)
                    {
                        font.DrawText(spriteBatch, new Vector2(10, ps), benchMarkDetailse.Key, Color.White, new Vector2(0.5f, 0.5f));
                        font.DrawText(spriteBatch, new Vector2(300, ps), benchMarkDetailse.Value.NumberOfTimesRun.ToString(), Color.White, fontscale);
                        font.DrawText(spriteBatch, new Vector2(450, ps), benchMarkDetailse.Value.AverageTime.ToString("c"), Color.White, fontscale);
                        font.DrawText(spriteBatch, new Vector2(600, ps), benchMarkDetailse.Value.ShortestTime.ToString("c"), Color.White, fontscale);
                        font.DrawText(spriteBatch, new Vector2(750, ps), benchMarkDetailse.Value.LongestTime.ToString("c"), Color.White, fontscale);
                        font.DrawText(spriteBatch, new Vector2(900, ps), benchMarkDetailse.Value.TotalTime.ToString("c"), Color.White, fontscale);

                        ps = ps + 30;
                    }
                }

                foreach (var i in DebugObjects)
                {
                    if (showDebug)
                    {


                        font.DrawText(spriteBatch, new Vector2(spriteBatch.GraphicsDevice.Viewport.Bounds.Width - width - 10, mxh * ct), i.CurrentText, Color.White, scale);
                        if (Solids.Instance.FrameCounter.CurrentFramesPerSecond > 59)
                        {
                            if (pointer % 500 == 0)
                            {
                                mx = Math.Max(i.HistoricValues.Max(), 1);

                                if (mx > i.MXValue)
                                {
                                    i.MXValue = mx;
                                }
                                else
                                {
                                    i.MXValue = i.MXValue * 0.995f;
                                }

                            }

                            mx = i.MXValue;

                            int xct = 0;
                            for (int x = pointer; x < pointer + 49; x++)
                            {
                                float h1 = (i.HistoricValues[x % 50] / mx) * mxh;
                                float h2 = (i.HistoricValues[(x + 1) % 50] / mx) * mxh;

                                int ps = spriteBatch.GraphicsDevice.Viewport.Bounds.Width - 90 + (xct * 2);

                                spriteBatch.DrawLine(new Vector2(ps, h1 + (ct * mxh)), new Vector2(ps + 2, h2 + (ct * mxh)), Color.White);

                                xct++;
                            }
                        }

                        ct++;
                    }
                    i.HistoricValues[pointer % 50] = i.CurrentValue;
                }


                pointer++;
            }
        }

        public class DebugObject
        {
            private float mx = 0;

            public string Key;
            public float CurrentValue;
            public float[] HistoricValues = new float[50];

            public float MXValue = 0;

            public string CurrentText => Key + ": " + CurrentValue;
        }
    }
}
