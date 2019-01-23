using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Breeze.AssetTypes;
using Breeze.FontSystem;
using Breeze.Helpers;
using Breeze.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze
{
    public class ScreenManager
    {
        private float fadeSpeed = 0.1f;
        private Rectangle previousScreenBounds = new Rectangle(0, 0, 0, 0);
        readonly List<BaseScreen> screens = new List<BaseScreen>();

        public ScreenFactory Factory;

        public ScreenManager()
        {
            Factory = new ScreenFactory(this);
        }

        public void Add(BaseScreen baseScreen)
        {
            if (screens != null && screens.Any())
            {
                baseScreen.ZIndex = screens.Max(t => t.ZIndex) + 1;
            }
            else
            {
                baseScreen.ZIndex = 1;
            }

            screens.Add(baseScreen);

            Debug.WriteLine("There are now "+screens.Count+" screen in the manager");
        }

        public void Remove(BaseScreen screen)
        {
            screen.CloseRequested = true;
            Debug.WriteLine("There are now " + screens.Count + " screen in the manager");
        }

        public void Remove<T>() where T : BaseScreen
        {
            T screen = SmartList().FirstOrDefault(t => t is T) as T;

            if (screen != null)
            {
                Remove((BaseScreen)screen);
            }

            Debug.WriteLine("There are now "+screens.Count+" screen in the manager");
        }

        public void RemoveAll<T>() where T : BaseScreen
        {
            List<BaseScreen> boof = SmartList().Where(t => t is T).ToList();

            boof.ForEach(x => x.CloseRequested = true);
            Debug.WriteLine("There are now " + screens.Count + " screen in the manager");
        }

        public void BringToFront(BaseScreen screen)
        {
            screen.ZIndex = SmartList().Max(t => t.ZIndex) + 1;
        }

        public void SendToBack(BaseScreen screen)
        {
            screen.ZIndex = SmartList().Min(t => t.ZIndex) - 1;
        }

        public BaseScreen ActiveScreen()
        {
            return SmartList().Where(t => t.CanInteract).OrderByDescending(t => t.ZIndex).FirstOrDefault();
        }

        public List<BaseScreen> ToList()
        {
            return screens;
        }

        public List<BaseScreen> SmartList()
        {
            List<BaseScreen> result = screens.ToList();

            return result.OrderBy(t => t.ZIndex).ToList();
        }

        public void Update(GameTime gameTime)
        {
            foreach (BaseScreen screen in SmartList())
            {
                {
                    screen.Update(gameTime);
                }
            }
        }

        private void HandleScreenUpdate()
        {
            foreach (BaseScreen baseScreen in SmartList().Where(t => t.CloseRequested).ToList())
            {
                baseScreen.OpenCloseOpacity = baseScreen.OpenCloseOpacity - fadeSpeed;

                if (baseScreen.OpenCloseOpacity < 0)
                {
                    screens.Remove(baseScreen);
                    baseScreen.FireCloseEvent();
                    Debug.WriteLine("There are now " + screens.Count + " screen in the manager");
                }
            }

            foreach (BaseScreen baseScreen in SmartList().Where(t => !t.CloseRequested).ToList())
            {
                if (baseScreen.OpenCloseOpacity < 1f)
                {
                    baseScreen.OpenCloseOpacity = baseScreen.OpenCloseOpacity + fadeSpeed;
                }
            }
        }

        public void Draw(SmartSpriteBatch spriteBatch, GameTime gameTime)
        {

            try
            {
                Solids.Instance.Bounds = spriteBatch.GraphicsDevice.Viewport.Bounds;

                Texture2D previousTexture = null;


                var smartList = SmartList().ToList();

                if (smartList.Any(t => t.IsBackgroundBlocking))
                {
                    int zindex = smartList.OrderBy(t => t.ZIndex).First(t => t.IsBackgroundBlocking).ZIndex;

                    smartList = smartList.Where(t => t.ZIndex >= zindex).ToList();
                }

                //smartList.RemoveAll(x => x is FSLoadingScreen);

                //if (Solids.Screens.SmartList().Any(x=>x is FSLoadingScreen))
                //{
                //    smartList.Add(Solids.Screens.SmartList().First(y => y is FSLoadingScreen));
                //}

                foreach (BaseScreen screen in smartList)
                {
                    
                    {
                        if (screen.RenderTarget == null || screen.RenderTarget.Bounds != Solids.Instance.Bounds)
                        {
                            screen.RenderTarget = new RenderTarget2D(Solids.Instance.SpriteBatch.GraphicsDevice, Solids.Instance.Bounds.Width, Solids.Instance.Bounds.Height);
                        }

                        Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(screen.RenderTarget);
                        Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);
                        screen.Draw(gameTime, screen.OpenCloseOpacity, previousTexture);

                        previousTexture = screen.RenderTarget;
                    }
                }

                Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(null);
                Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.TransparentBlack);

                if (smartList.Count > 0)
                {
                    using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, null))
                    {
                        Solids.Instance.SpriteBatch.Draw(previousTexture, Solids.Instance.Bounds, null, Color.White);
                    }
                }

                //foreach (BaseScreen screen in Solids.Screens.SmartList())
                //{
                //    if (screen.RenderTarget != null)
                //    {
                //        Solids.Instance.SpriteBatch.Draw(screen.RenderTarget, Solids.Bounds, null, Color.White);
                //    }
                //    else
                //    {
                //        BreezeDebug.WriteLine(screen.ToString()+" has a null render target?");
                //    }
                //}


                //Solids.Instance.SpriteBatch.GraphicsDevice.SetRenderTarget(null);
                //Solids.Instance.SpriteBatch.GraphicsDevice.Clear(Color.Pink);
                //Solids.Instance.SpriteBatch.ForceBegin(SpriteSortMode.Immediate,BlendState.Opaque,SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, null);
                //Solids.Instance.SpriteBatch.Draw(texture, Solids.Bounds, null, Color.White);
                //Solids.Instance.SpriteBatch.Draw(txt, Solids.Bounds, null, Color.White);

                //int width = (Solids.Bounds.Width / 20);

                //float ratio= ((float)txt.Width / (float)txt.Height);
                //int height = (int)((float)width/ ratio);

                //Solids.Instance.SpriteBatch.Draw(txt, new Rectangle(Solids.Bounds.X - width, Solids.Bounds.Y-height,width,height),null, Color.White);
                //Solids.Instance.SpriteBatch.ForceEnd();

           //     if (spriteBatch.GraphicsDevice.Viewport.Bounds != previousScreenBounds || Solids.Instance.ScreenManager.ToList().Any(t => t is BaseScreen && !((BaseScreen)t).BoundsSet))
                {
                    foreach (BaseScreen screen in SmartList().Where(t => t is BaseScreen && ((BaseScreen)t).IsFullScreen))
                    {
                        screen.SetBounds(spriteBatch.GraphicsDevice.Viewport.Bounds);
                    }

                  //  previousScreenBounds = spriteBatch.GraphicsDevice.Viewport.Bounds;
                }

                //if (Solids.Instance.InputService != null)
                //{
                //    spriteBatch.Begin();
                //    spriteBatch.Draw(Helpers.TextureHelpers.GetPoint(spriteBatch), new Rectangle((int)Solids.Instance.InputService.MousePosition.X, (int)Solids.Instance.InputService.MousePosition.Y, 1, 1), Color.Black);
                //    spriteBatch.End();
                //}
            }
            catch
            {
            }

            HandleScreenUpdate();
        }

        public class ScreenFactory
        {
            private ScreenManager screenManager;

            public ScreenFactory(ScreenManager manager)
            {
                screenManager = manager;
            }
            //public void AddMainMenu(bool fadeScreen = false)
            //{
            //    if (screenManager.screens.All(t => (t as MainMenuScreen) == null))
            //    {
            //        MainMenuScreen menuScreen = new MainMenuScreen();
            //        menuScreen.Fade = fadeScreen;
            //        menuScreen.Initialise();
            //        screenManager.Add(menuScreen);
            //        screenManager.BringToFront(menuScreen);
            //    }

            //}

            //public void AddSequencerView()
            //{
            //    if (screenManager.screens.All(t => (t as SequencerScreen) == null))
            //    {
            //        SequencerScreen sequencerScreen = new SequencerScreen();
            //        sequencerScreen.Initialise();
            //        screenManager.Add(sequencerScreen);
            //        screenManager.BringToFront(sequencerScreen);

            //        screenManager.RemoveAll<BackgroundScreen>();
            //        screenManager.RemoveAll<FilterScreen>();
            //    }
            //}

            //public void AddConsoleView()
            //{
            //    if (screenManager.screens.All(t => (t as ConsoleScreen) == null))
            //    {
            //        ConsoleScreen consoleScreen = new ConsoleScreen();
            //        consoleScreen.Initialise();
            //        screenManager.Add(consoleScreen);
            //        screenManager.BringToFront(consoleScreen);
            //    }
            //}

            public T AddViewWithoutDuplicationCheck<T>() where T : BaseScreen, new()
            {
                    T screen = new T();
                    ((T)screen).Initialise();
                    screenManager.Add(screen);
                    screenManager.BringToFront(screen);

                    return screen; 
            }

            public T AddView<T>() where T : BaseScreen, new()
            {
                if (screenManager.screens.All(t => (t as T) == null))
                {
                    T screen = new T();
                    ((T)screen).Initialise();
                    screenManager.Add(screen);
                    screenManager.BringToFront(screen);

                    return screen;
                }
                else
                {
                    return (T)screenManager.screens.First(t => (t as T) != null);
                }
            }

            public async Task<T> AddViewAsync<T>() where T : BaseScreen, new()
            {
                if (screenManager.screens.All(t => (t as T) == null))
                {
                    T screen = new T();
                    await ((T)screen).Initialise();
                    screenManager.Add(screen);
                    screenManager.BringToFront(screen);

                    return screen;
                }
                else
                {
                    return (T)screenManager.screens.First(t => (t as T) != null);
                }
            }

            public void AddView(BaseScreen screen)
            {
                screenManager.Add(screen);
                screenManager.BringToFront(screen);
            }

            //public void ShowToast(string title, string body, string icon)
            //{
            //    ToastScreen screen = new ToastScreen(title, body, icon);
            //    screen.Initialise();
            //    screenManager.Add(screen);
            //    screenManager.BringToFront(screen);
            //}

            //public void ShowToast(string title, string body, MDL2Symbols icon)
            //{
            //    ToastScreen screen = new ToastScreen(title, body, icon.AsChar());
            //    screen.Initialise();
            //    screenManager.Add(screen);
            //    screenManager.BringToFront(screen);
            //}


            //public void ShowToast(string title, string body, int icon)
            //{
            //    ToastScreen screen = new ToastScreen(title, body, ((char)icon).ToString());
            //    screen.Initialise();
            //    screenManager.Add(screen);
            //    screenManager.BringToFront(screen);
            //}

            //public void AddMachineEditorView(IMachineWithUI channelMachine)
            //{
            //    if (screenManager.screens.All(t => (t as MachineEditorScreen) == null))
            //    {
            //        MachineEditorScreen machineEditorScreen = new MachineEditorScreen(channelMachine);
            //        machineEditorScreen.Initialise();
            //        screenManager.Add(machineEditorScreen);
            //        screenManager.BringToFront(machineEditorScreen);
            //    }
            //}

            //public void AddMachineUIView(IMachineWithUI channelMachine)
            //{
            //    if (screenManager.screens.All(t => (t as MachineUIScreen) == null))
            //    {
            //        MachineUIScreen machineEditorScreen = new MachineUIScreen(channelMachine);
            //        machineEditorScreen.Initialise();
            //        screenManager.Add(machineEditorScreen);
            //        screenManager.BringToFront(machineEditorScreen);
            //    }
            //}

            //public void AddFileBrowser(string userFilesPath, string userFiles, bool b)
            //{
            //    if (screenManager.screens.All(t => (t as FileBrowserScreen) == null))
            //    {
            //        FileBrowserScreen fileBrowserScreen = new FileBrowserScreen();
            //        fileBrowserScreen.Initialise();
            //        screenManager.Add(fileBrowserScreen);
            //        screenManager.BringToFront(fileBrowserScreen);

            //        fileBrowserScreen.SetPath(userFilesPath, userFiles, b);
            //    }
            //}
        }

        public T Get<T>() where T : BaseScreen
        {
            return (T)SmartList().FirstOrDefault(t => t is T);
        }
    }
}
