﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Storage;
using Breeze.FontSystem;
using Breeze.Helpers;
using Breeze.Services.InputService;
using Breeze.Storage;
using Breeze.Storage.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Breeze
{
    public class BreezeInstance
    {
        private List<char> bufferedInput = new List<char>();

        public Font Fonts { get; set; }
        public string ContentPath { get; set; }
        public string ContentPathRelative { get; set; }
        public AssetLibrary AssetLibrary { get; set; }
        public ScreenManager ScreenManager { get; set; }
        public SmartSpriteBatch SpriteBatch { get; set; }
        public InputService InputService { get; set; }
        internal Rectangle Bounds { get; set; }
        public bool ShowDebug { get; set; }
        public DebugSystem DebugSystem = new DebugSystem();
        public FrameCounter FrameCounter = new FrameCounter();
        public Storage.Storage Storage { get; set; } = new Storage.Storage();

        public BreezeInstance(ContentManager contentManager, SpriteBatch spriteBatch, Game game)
        {
            ScreenManager = new ScreenManager();
            SpriteBatch = new SmartSpriteBatch(spriteBatch.GraphicsDevice);
            AssetLibrary = new AssetLibrary(SpriteBatch);
            Solids.GaussianBlur = new Breeze.Helpers.GaussianBlur(game);
            InputService = new InputService();




#if WINDOWS_UAP
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            ContentPath = folder.Path + "\\Ezmuze.Content\\";
            ContentPathRelative = "Ezmuze.Content\\";
#elif ANDROID

            Solids.ContentPath = "Content\\";
            Solids.ContentPathRelative = "Content\\";
#else
            Solids.ContentPath =  "\\Ezmuze.Content\\";
    Solids.ContentPathRelative = "\\Ezmuze.Content\\";
#endif

#if WINDOWS_UAP
            string pakPath = ContentPathRelative.GetFile("Content.pak");
            Debug.WriteLine(FileLocation.InstalledLocation);

            StorageFile packBuffer = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(pakPath).GetAwaiter().GetResult();

            Stream stream = packBuffer.OpenStreamForReadAsync().Result;
            int streamLength = (int)stream.Length;
            byte[] tmp = new byte[streamLength];
            stream.Read(tmp, 0, tmp.Length);

            string tocPath = ContentPathRelative.GetFile("Content.toc");

            Debug.WriteLine(FileLocation.InstalledLocation);
            Debug.WriteLine(tocPath);
            Debug.WriteLine(pakPath);

            var tocBuffer = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(tocPath).GetAwaiter().GetResult();

            string json = FileIO.ReadTextAsync(tocBuffer).GetAwaiter().GetResult();



            Storage.DatfileStorage.TabletOfContents = JsonConvert.DeserializeObject<List<FileEntry>>(json);
            Storage.DatfileStorage.DataStorage = tmp;

            Debug.WriteLine("test!");
            //Solids.Breeze.Storage.Init(Solids.ContentPathRelative,tmp).ConfigureAwait(false).GetAwaiter().GetResult();

#elif ANDROID
            string pakPath = ("Content.pak");
            string tocPath = ("Content.toc");

            Stream tocBuffer = tocPath.FromAsset();
            var sr = new StreamReader(tocBuffer);
            string json = sr.ReadToEnd();
            
            Solids.DatfileStorage.TabletOfContents = JsonConvert.DeserializeObject<List<Storage.FileEntry>>(json);

            var pakBuffer = pakPath.ToStream().Result;

            int length = Solids.DatfileStorage.TabletOfContents.Last().Start + Solids.DatfileStorage.TabletOfContents.Last().Length;

            byte[] data = new byte[length];
            pakBuffer.Read(data, 0, (int)length);
            Solids.DatfileStorage.DataStorage = data;


#endif
            Solids.Instance = this;



            //todo - this needs to be hella more dynamic
            Fonts = new Font
            {
                EuroStile = new FontFamily("EuroStile", "eurostile"),
                Consolas = new FontFamily("Consolas", "consolas"),
                MDL2 = new FontFamily("MDL2", "mdl"),
                Segoe = new FontFamily("SegoeUI", "SegoeUI"),
                SegoeLight = new FontFamily("SegoeUILight", "SegoeUILight")
            };

            Fonts.Fonts.Add("EuroStile", Fonts.EuroStile);
            Fonts.Fonts.Add("Consolas", Fonts.Consolas);
            Fonts.Fonts.Add("MDL2", Fonts.MDL2);
            Fonts.Fonts.Add("Segoe", Fonts.Segoe);
            Fonts.Fonts.Add("SegoeLight", Fonts.SegoeLight);

            
        }

      
        public void Draw(GameTime gameTime, bool showDebug)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FrameCounter.Update(deltaTime);

            if (SpriteBatch.GraphicsDevice.Viewport.Bounds != Solids.Bounds)
            {
                Solids.Bounds = SpriteBatch.GraphicsDevice.Viewport.Bounds;
                Bounds = SpriteBatch.GraphicsDevice.Viewport.Bounds;
                //Solids.DefaultScreenTarget = new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, Solids.Bounds.Width, Solids.Bounds.Height);
            }

            ScreenManager.Draw(SpriteBatch, gameTime);


            if (Debugger.IsAttached)
            {
                DebugSystem.SetValue("FPS", FrameCounter.CurrentFramesPerSecond);
                DebugSystem.SetValue("AVG FPS", FrameCounter.AverageFramesPerSecond);
            }


            SpriteBatch.GraphicsDevice.SetRenderTarget(null);

            if (InputService != null)
            {
                using (new SmartSpriteBatchManager(SpriteBatch))
                {
                    InputService.Draw(gameTime);
                }
            }


            if (ShowDebug)
            {
                float step = Solids.Bounds.Width / (float)BenchMarkProvider.Steps;
                float htStep = 1000 / (float)Solids.Bounds.Height;

                int ps = 25;

                foreach (KeyValuePair<string, BenchMarkDetails> bench in BenchMarkProvider.BenchMarks)
                {
                    TimeSpan tm = bench.Value.FrameHistorySpan[BenchMarkProvider.FramePointer % BenchMarkProvider.Steps];
                    TimeSpan xx = (bench.Value.TotalTime - tm);
                    bench.Value.FrameHistory[(BenchMarkProvider.FramePointer + 1) % BenchMarkProvider.Steps] = xx.TotalMilliseconds;
                    bench.Value.FrameHistorySpan[BenchMarkProvider.FramePointer % BenchMarkProvider.Steps] = bench.Value.TotalTime;
                }

                SpriteBatch.DrawSolidRectangle(new FloatRectangle(0, 25, Solids.Bounds.Width / 2f, (BenchMarkProvider.BenchMarks.Count + 1) * 25), Color.Black * 0.85f);

                int cttt = 0;
                Vector2[] fpspoints = new Vector2[BenchMarkProvider.Steps];
                foreach (KeyValuePair<string, BenchMarkDetails> b in BenchMarkProvider.BenchMarks.ToList())
                {
                    cttt = cttt + 5;
                    float th = 0;
                    Vector2[] points = new Vector2[BenchMarkProvider.Steps];
                    for (int i = 0; i < BenchMarkProvider.Steps; i++)
                    {
                        int ptr = ((BenchMarkProvider.FramePointer + 2) + i) % BenchMarkProvider.Steps;
                        double h = b.Value.FrameHistory[ptr];

                        int hhh = 0;
                        if (h > 0)
                        {
                            hhh = (int)(h / htStep);
                            if (double.IsNaN(hhh))
                            {
                                hhh = 0;
                            }
                        }

                        if (i == cttt)
                        {
                            th = Solids.Bounds.Height - hhh;
                        }

                        points[i] = new Vector2(i * step, Solids.Bounds.Height - hhh);
                    }

                    SpriteBatch.DrawLine(points, b.Value.Color * 0.5f, brushSize: 2);

                    SpriteBatch.Begin(blendState: BlendState.NonPremultiplied);
                    Fonts.Consolas.GetFont(20).DrawText(SpriteBatch, new Vector2(10, ps), b.Key + " - " + b.Value.NumberOfTimesRun + " - " + b.Value.FrameHistory[(BenchMarkProvider.FramePointer + 1) % BenchMarkProvider.Steps] + "ms", b.Value.Color);
                    Fonts.Consolas.GetFont(20).DrawText(SpriteBatch, new Vector2((Solids.Bounds.Width / BenchMarkProvider.Steps) * cttt, th), b.Key + " - " + b.Value.FrameHistory[(BenchMarkProvider.FramePointer + 1) % BenchMarkProvider.Steps] + "ms", b.Value.Color);
                    SpriteBatch.End();
                    ps = ps + 25;

                }

                float thh = 0;
                for (int i = 0; i < BenchMarkProvider.Steps; i++)
                {
                    int ptr = ((BenchMarkProvider.FramePointer + 2) + i) % BenchMarkProvider.Steps;
                    float hhhh = BenchMarkProvider.FPSLog[ptr] / (60f / Solids.Bounds.Height);
                    fpspoints[i] = new Vector2(i * step, hhhh);
                    if (i == BenchMarkProvider.Steps / 2)
                    {
                        thh = hhhh;
                    }
                }

                SpriteBatch.DrawLine(fpspoints, Color.White * 0.25f, brushSize: 2);
                SpriteBatch.Begin(blendState: BlendState.NonPremultiplied);
                Fonts.Consolas.GetFont(20).DrawText(SpriteBatch, new Vector2(10, ps), "FPS:" + FrameCounter.AverageFramesPerSecond, Color.White * .5f);
                Fonts.Consolas.GetFont(20).DrawText(SpriteBatch, new Vector2((Solids.Bounds.Width / BenchMarkProvider.Steps) * cttt, thh), "FPS:" + FrameCounter.CurrentFramesPerSecond, Color.White * .5f);
                SpriteBatch.End();
                BenchMarkProvider.NextFrame();
            }
        }

      
        public void TextInputHandler(object sender, TextInputEventArgs e)
        {
            if (!char.IsControl(e.Character))
            {
                bufferedInput.Add(e.Character);
            }
        }

        public void Update(GameTime gameTime)
        {
            InputService.PressedChars = bufferedInput;
            bufferedInput = new List<char>();
            
            // TODO: Add your update logic here

            InputService?.Update(gameTime);

            ScreenManager.Update(gameTime);
        }
    }



    public class Font
    {
        public FontFamily MDL2;
        public FontFamily EuroStile { get; set; }
        public FontFamily Consolas { get; set; }
        public FontFamily Segoe { get; set; }
        public FontFamily SegoeLight { get; set; }

        public Dictionary<string, FontFamily> Fonts = new Dictionary<string, FontFamily>();
    }
}