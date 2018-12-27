using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Breeze.AssetTypes;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.Helpers;
using Breeze.Services.InputService;
using Breeze.Shared.AssetTypes;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.Screens
{
    public class BaseScreen// : IScreen
    {
        public bool HidesBehind { get; set; } = false;
        public bool ClearBG { get; set; } = false;
        public bool CanInteract = true;
        public bool DimBackground = false;
        public bool BlurBackground = false;
        public bool BoundsSet = false;
        public float FixedAspectRatio = 0;

        public float BGdim { get; set; } = 0f;
        public int ZIndex { get; set; } = 0;
        public delegate void OnCloseEventHandler();
        public event OnCloseEventHandler OnCloseScreen;
        public bool CloseRequested { get; set; }
        public float OpenCloseOpacity { get; set; }
        public InteractiveAsset ActiveButton = null;
        public bool IsFullScreen { get; set; } = false;

        public Dictionary<string,DataboundAsset> Templates = new Dictionary<string,DataboundAsset>();

        public List<DataboundAsset> RealTime = new List<DataboundAsset>();
        public List<DataboundAsset> AllAssets = new List<DataboundAsset>();
        public List<DataboundAsset> FixedAssets = new List<DataboundAsset>();
        public List<DataboundAsset> DynamicProcessedAssets = new List<DataboundAsset>();
        private List<DataboundAsset> dynamicAssets = new List<DataboundAsset>();
        public RenderTarget2D RenderTarget;
        public FloatRectangle? WindowClip;
        public int BGblur { get; set; } = 0;
        public InputBindingEvents BindingEvents { get; set; }
        public bool HandleButtons = true;
        public bool HandleButtonNav = true;

        public bool firstUpdateComplete = false;
        public ScreenAbstractor Screen;

        private bool mouseActive = false;
        private readonly float dimSpeed = 0.06f;

        public void LoadScreen(XmlNode node)
        {
            if (node.FirstChild.Attributes.GetNamedItem("FixedAspectRatio")?.Value != null) FixedAspectRatio = float.Parse(node.FirstChild.Attributes.GetNamedItem("FixedAspectRatio")?.Value);
            FixedAssets.AddRange(HandleChildren(node.FirstChild));

        }

        public List<DataboundAsset> HandleChildren(XmlNode node)
        {
            List<DataboundAsset> results = new List<DataboundAsset>();
            List<Type> types = ReflectionHelpers.TypesImplementingInterface(typeof(DataboundAsset)).ToList();

            Debug.WriteLine(node);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                Debug.WriteLine(childNode);
                Debug.WriteLine(childNode.Attributes);

                Type type = types.FirstOrDefault(t => t.Name == childNode.Name);
                if (type != null)
                {
                    DataboundAsset asset = (DataboundAsset)Activator.CreateInstance(type);
                    asset.LoadFromXml(childNode.Attributes);

                    if (asset is DataboundAssetWhereChildIsContentAsset masterAsset && !string.IsNullOrEmpty(childNode.InnerText))
                    {
                        masterAsset.LoadContent(childNode.InnerText);
                    }

                    if (asset is DataboundContainterAsset containterAsset)
                    {
                        containterAsset.Children.Value.AddRange(HandleChildren(childNode));
                    }

                    results.Add(asset);
                }
                else
                {
                    if (childNode.Name.ToLower() == "resources")
                    {
                        foreach (XmlNode xmlNode in childNode.ChildNodes)
                        {
                            if (xmlNode.Name.ToLower() == "template")
                            {
                                string nm = (string)xmlNode.Attributes.GetNamedItem("Name").Value;

                                Templates.Add(nm,HandleChildren(xmlNode).First());

                                Debug.WriteLine(nm);
                            }
                        }
                    }
                }
            }

            return results;
        }

        public class InputBindingEvents
        {
            internal Dictionary<InputService.InputBinding, List<Action>> inputBindingEvents = new Dictionary<InputService.InputBinding, List<Action>>();

            public void Add(InputService.InputBinding binding, Action action)
            {
                if (inputBindingEvents.ContainsKey(binding))
                {
                    if (!inputBindingEvents[binding].Contains(action))
                    {
                        inputBindingEvents[binding].Add(action);
                    }
                }
                else
                {
                    inputBindingEvents.Add(binding, new List<Action> { action });
                }
            }

            public InputBindingEvents(InputService.InputBinding binding, Action action)
            {
                inputBindingEvents.Add(binding, new List<Action> { action });
            }
        }

        internal void StartWindowDrag(ButtonClickEventArgs args)
        {
            Debug.WriteLine("Oh Hai!");
            dragOffset = args.ClickPosition;
            dragging = true;
        }


        internal void StopWindowDrag(ButtonClickEventArgs args)
        {
            Debug.WriteLine("Oh Hai!");
            dragOffset = null;
            dragging = false;
        }


        private bool dragging = false;
        private Vector2? dragOffset = null;

        //internal InputBindings.ContextMenu ShowContextMenu(List<SequencerScreen.ContextControl> contextControls)
        //{
        //    Vector2 ps = Solids.Instance.InputService.MouseScreenPosition;

        //    if (Solids.Instance.InputService.MouseActive && MouseScreenPos.HasValue)
        //    {
        //        ps = Solids.Instance.InputService.MouseScreenPosition;
        //    }

        //    return InputBindings.ContextMenu.OpenContextMenu(new FloatRectangle(ps.X, ps.Y, 1, 1), contextControls.Select(y => y.ButtonBasics).ToList());
        //}
        public bool IsPressed(InputService.InputBinding binding)
        {
            return (Solids.Instance.ScreenManager.ActiveScreen() == this && binding.IsPressed(Solids.Instance.InputService.CurrentStack));
        }

        public List<DataboundAsset> DynamicAssets
        {
            get => dynamicAssets;
            set
            {
                dynamicAssets = value;
                UpdateAllAssets();
            }
        }

        public bool IsBackgroundBlocking { get; set; }

        public Vector2? MouseScreenPos
        {
            get
            {
                if (Solids.Instance.ScreenManager.ActiveScreen() == this)
                {
                    return Solids.Instance.InputService.MouseScreenPosition;
                }
                return null;

            }
        }

        public void AddOrUpdateDynamicAsset(DataboundAsset asset)
        {
            //DataboundAsset da = DynamicProcessedAssets.FirstOrDefault(t => t.Key == asset.Key);

            //if (da != null)
            //{
            //    da.Update(da, asset);
            //}
            //else
            //{
            //    DynamicProcessedAssets.Add(asset);
            //}

            //asset.IsDirty = true;
        }

        private int allAssetHash=0;
        public void UpdateAllAssets()
        {

            var tmp = FlattenTree(FixedAssets);
            var hash = tmp.Count;
            if (hash != allAssetHash)
            {
                AllAssets = tmp;
                allAssetHash = hash;
            }

            //AllAssets = new List<DataboundAsset>();
            //AllAssets.AddRange(GetAssetsAndTheirChildren(FixedAssets));
            AllAssets.AddRange(GetAssetsAndTheirChildren(DynamicProcessedAssets.Select(x => (DataboundAsset)x).ToList()));

        }

        public T FindChildOfType<T>(DataboundContainterAsset asset) where T : DataboundAsset
        {
            if (asset.Children.Value != null)
            {
                if (asset.Children.Value.FirstOrDefault(x => x is T) is T found)
                {
                    return found;
                }

                foreach (DataboundContainterAsset databoundAsset in asset.Children.Value.Where(x => x is DataboundContainterAsset))
                {
                    T foundInChildren = FindChildOfType<T>(databoundAsset);

                    if (foundInChildren != null)
                    {
                        return foundInChildren;
                    }
                }
            }

            return null;
        }

        public List<T> FindChildrenOfType<T>(DataboundContainterAsset asset) where T : DataboundAsset
        {
            List<T> results = new List<T>();
            if (asset.Children.Value != null)
            {
                foreach (DataboundAsset databoundAsset in asset.Children.Value)
                {
                    if ((databoundAsset as T)!=null)
                    {
                        results.Add((T)databoundAsset);
                    }

                    if (databoundAsset is DataboundContainterAsset databoundContainterAsset)
                    {
                        List<T> foundInChildren = FindChildrenOfType<T>(databoundContainterAsset);

                        if (foundInChildren != null)
                        {
                            results.AddRange(foundInChildren);
                        }
                    }
                }
            }

            return results;
        }

        public List<DataboundAsset> FlattenTree(DataboundAsset asset)
        {
            List<DataboundAsset> result = new List<DataboundAsset> {asset};

            if (asset is DataboundContainterAsset dbc)
            {
                foreach (DataboundAsset databoundAsset in GetVirtualChildren(dbc))
                {
                    result.AddRange(FlattenTree(databoundAsset));
                }
            }
            
            return result.Where(t=>!(t is ContentAsset)).ToList();
        }

        public List<DataboundAsset> FlattenTree(List<DataboundAsset> assets)
        {
            return assets.SelectMany(FlattenTree).ToList();
        }




        public List<DataboundAsset> GetVirtualChildren(DataboundAsset screenAsset)
        {
            List<DataboundAsset> result = new List<DataboundAsset>();

       if (screenAsset is TemplateAsset templateAsset)
            {
                if (!string.IsNullOrWhiteSpace(templateAsset.Template.Value))
                {
                    string templateName = templateAsset.Template.Value;

                    DataboundAsset templateContent = Templates[templateName].DeepClone();
                    templateContent.ParentPosition = templateAsset.ActualPosition;

                    //if (templateContent is DataboundContainterAsset templateContainterAsset)
                    //{
                    //    List<DataboundAsset> childs = templateAsset.Children.Value.DeepClone();
                    //    foreach (DataboundAsset databoundAsset in childs)
                    //    {
                    //        databoundAsset.ParentPosition = templateAsset.ActualPosition;
                    //    }
                    //    List<ContentAsset> content = FindChildrenOfType<ContentAsset>(templateContainterAsset);

                        

                    //    foreach (var cont in content)
                    //    {
                    //        cont.Children.Value = childs;
                    //        cont.ParentPosition = templateAsset.ActualPosition;
                    //    }

                    //  //  templateContainterAsset.Children = new DataboundAsset.DataboundValue<List<DataboundAsset>>();
                    //}

                    result.Add(templateContent);
                    //result.AddRange(GetVirtualChildren(templateContent));


                }
            }
            else if (screenAsset is DataboundContainterAsset asset)
            {
                if (asset.Children.Value != null)
                {
                    result.AddRange(asset.Children.Value);
                }
            }

            return result;
        }

        public List<DataboundAsset> GetAssetsAndTheirChildren(List<DataboundAsset> assets)
        {
            List<DataboundAsset> result = new List<DataboundAsset>();
            foreach (var screenAsset in assets)
            {
                result.Add(screenAsset);
                if (screenAsset is DataboundContainterAsset asset)
                {
                    result.AddRange(GetAssetsAndTheirChildren(GetVirtualChildren(asset)).ToList());
                }
            }

            return result;
        }

        public void ProcessDynamicAssets()
        {
            foreach (DataboundAsset dynamicAsset in DynamicAssets)
            {
                if (dynamicAsset != null)
                {
                    AddOrUpdateDynamicAsset(dynamicAsset);
                }
            }

            DynamicProcessedAssets.RemoveAll(y => DynamicAssets.All(p => p.Key != y.Key));

            UpdateAllAssets();
        }




        public virtual async Task Initialise()
        {
            Screen = new ScreenAbstractor();
            Screen.SetBounds(Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Solids.Instance.ScreenManager.ActiveScreen() == this)
            {
                foreach (KeyValuePair<InputService.InputBinding, List<Action>> be in BindingEvents.inputBindingEvents)
                {
                    if (be.Key.IsPressed(Solids.Instance.InputService.CurrentStack))
                    {
                        foreach (Action action in be.Value)
                        {
                            action?.Invoke();
                        }
                    }
                }
            }

            ProcessDynamicAssets();
            var tba = AllAssets.OfType<TextboxAsset>().Where(t => t.EditMode);
            if (HandleButtons || !tba.Any())
            {
                mouseActive = Solids.Instance.InputService.MouseActive;

                if (ActiveButton != null && AllAssets.Contains(ActiveButton) == false)
                {
                    ActiveButton = null;
                }

                if (ActiveButton == null && !mouseActive && firstUpdateComplete)
                {
                    ActiveButton = AllAssets.OfType<InteractiveAsset>().FirstOrDefault();
                }

                foreach (InteractiveAsset asset in AllAssets.OfType<InteractiveAsset>())
                {
                    asset.State.Value = asset == ActiveButton ? ButtonState.Hover : ButtonState.Normal;
                }

                if (this == Solids.Instance.ScreenManager.ActiveScreen())
                {
                    if ((AllAssets.OfType<InteractiveAsset>().Any() || AllAssets.OfType<InteractiveAsset>().Any()) && Solids.Instance.InputService != null && Solids.Instance.InputService.MousePosition.X > 0 && Solids.Instance.InputService.MousePosition.Y > 0)
                    {
                        int mx = (int)Solids.Instance.InputService.MousePosition.X;
                        int my = (int)Solids.Instance.InputService.MousePosition.Y;


                        if (mouseActive)
                        {
                            bool found = false;
                            foreach (InteractiveAsset buttonAsset in AllAssets.OfType<InteractiveAsset>())
                            {
                                Rectangle thing = Screen.Translate(buttonAsset.ActualPosition).ToRectangle();
                                if (mx > thing.X && my > thing.Y && mx < thing.Right && my < thing.Bottom)
                                {
                                    ActiveButton = buttonAsset;
                                    found = true;
                                }
                            }

                            foreach (InteractiveAsset buttonAsset in AllAssets.OfType<InteractiveAsset>())
                            {
                                Rectangle thing = Screen.Translate(buttonAsset.ActualPosition).ToRectangle();
                                if (mx > thing.X && my > thing.Y && mx < thing.Right && my < thing.Bottom)
                                {
                                    ActiveButton = buttonAsset;
                                    found = true;
                                }
                            }

                            if (!found)
                            {

                                if (ActiveButton != null)
                                {
                                    ActiveButton.State.Value = ButtonState.Normal;
                                }

                                ActiveButton = null;
                            }
                        }
                    }



                    if (ActiveButton != null)
                    {
                        if (HandleButtonNav)
                        {
                            if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIDown) || Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIUp) || Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UILeft) || Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIRight))
                            {
                                Dictionary<InteractiveAsset, Vector2> buttonPositions = new Dictionary<InteractiveAsset, Vector2>();
                                foreach (InteractiveAsset buttonAsset in AllAssets.OfType<InteractiveAsset>())
                                {
                                    var rect = Screen.Translate(buttonAsset.ActualPosition).ToRectangle();
                                    Vector2 centerPos = new Vector2(rect.X + (rect.Width / 2f), rect.Y + (rect.Width / 2f));

                                    buttonPositions.Add(buttonAsset, centerPos);
                                }

                                float direction = -(float)Math.PI;
                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIRight)) direction = (-(float)Math.PI) + (float)Math.PI * 0.5f;
                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIDown)) direction = (-(float)Math.PI) + (float)Math.PI;
                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UILeft)) direction = (-(float)Math.PI) + (float)Math.PI * 1.5f;


                                Dictionary<InteractiveAsset, Vector2> validButtonPositions = new Dictionary<InteractiveAsset, Vector2>();
                                foreach (var buttonToCheck in buttonPositions.Where(t => t.Key != ActiveButton))
                                {
                                    var differenceVector = (buttonPositions[ActiveButton] - buttonToCheck.Value);
                                    float angle = differenceVector.ToAngle();

                                    if (validButtonPositions.ContainsKey(buttonToCheck.Key) == false)
                                    {
                                        if (direction.DifferenceBetweenAnglesInDegrees(angle) > -45 && direction.DifferenceBetweenAnglesInDegrees(angle) < 45)
                                        {
                                            validButtonPositions.Add(buttonToCheck.Key, buttonToCheck.Value);
                                        }
                                    }

                                }

                                InteractiveAsset bestButton = ActiveButton;
                                float bestDistance = float.MaxValue;
                                foreach (KeyValuePair<InteractiveAsset, Vector2> validButtonPosition in validButtonPositions)
                                {
                                    if (Math.Abs(Vector2.Distance(buttonPositions[ActiveButton], validButtonPosition.Value)) < bestDistance)
                                    {
                                        bestDistance = Math.Abs(Vector2.Distance(buttonPositions[ActiveButton], validButtonPosition.Value));

                                        bestButton = validButtonPosition.Key;
                                    }
                                }

                                if (bestButton != ActiveButton)
                                {
                                    ActiveButton.State.Value = ButtonState.Normal;
                                    ActiveButton = bestButton;
                                    ActiveButton.State.Value = ButtonState.Hover;
                                }
                            }
                        }

                        //  if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.MainAction))

                    }
                }
            }

            //todo     AllAssets.Where(t => t is KeyedUpdatedAsset).ToList().ForEach(ku => ((KeyedUpdatedAsset)ku).Update(Solids.Instance.SpriteBatch, this.Screen, this.OpenCloseOpacity));
            //todo     RealTime.Where(t => t is KeyedUpdatedAsset).ToList().ForEach(ku => ((KeyedUpdatedAsset)ku).Update(Solids.Instance.SpriteBatch, this.Screen, this.OpenCloseOpacity));

            firstUpdateComplete = true;
        }

        public void HandleClick()
        {
            if (ActiveButton == null) return;

            ButtonClickEventArgs args = new ButtonClickEventArgs();
            if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.Position.Value))
            {
                args.ClickSource = ClickSource.Mouse;

                args.ClickPosition =
                    new Vector2(
                        (Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.Position.Value.X) / ActiveButton.Position.Value.Width,
                        (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.Position.Value.Y) / ActiveButton.Position.Value.Height

                    );
            }

            var bActiveButton = (ActiveButton as StaticButtonAsset);
            if (bActiveButton != null)
            {

                Task backgroundWorkTask = Task.Run(() => bActiveButton?.Clicked?.Fire(args));

            }

            var tActiveButton = (ActiveButton as TextboxAsset);
            if (tActiveButton != null)
            {
                tActiveButton.EditMode = true;
                foreach (TextboxAsset buttonAsset in AllAssets.OfType<TextboxAsset>())
                {
                    if (buttonAsset != tActiveButton)
                    {
                        buttonAsset.EditMode = false;
                    }
                }

                tActiveButton.EditMode = true;
            }
        }




        public void HandlePress()
        {
            if (ActiveButton == null) return;

            ButtonClickEventArgs args = new ButtonClickEventArgs();
            if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.Position.Value))
            {
                args.ClickSource = ClickSource.Mouse;

                args.ClickPosition = new Vector2((Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.Position.Value.X) / ActiveButton.Position.Value.Width, (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.Position.Value.Y) / ActiveButton.Position.Value.Height);
            }

            if (ActiveButton is StaticButtonAsset bActiveButton)
            {

                Task backgroundWorkTask = Task.Run(() => bActiveButton?.ButtonDown?.Fire(args));

            }
        }


        public void HandleRelease()
        {
            if (ActiveButton == null) return;

            ButtonClickEventArgs args = new ButtonClickEventArgs();
            if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.Position.Value))
            {
                args.ClickSource = ClickSource.Mouse;

                args.ClickPosition = new Vector2((Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.Position.Value.X) / ActiveButton.Position.Value.Width, (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.Position.Value.Y) / ActiveButton.Position.Value.Height);
            }

            if (ActiveButton is StaticButtonAsset bActiveButton)
            {

                Task backgroundWorkTask = Task.Run(() => bActiveButton?.ButtonUp?.Fire(args));

            }
        }


        public virtual void Draw(GameTime gameTime)
        {

        }
        public Texture2D Draw(GameTime gameTime, float opacity, Texture2D preDrawTexture)
        {
            UpdateAllAssets();

            if (DimBackground && BGdim < 0.7f)
            {
                BGdim = BGdim + dimSpeed;
            }

            if (!DimBackground && BGdim > 0.0f)
            {
                BGdim = BGdim - dimSpeed;
            }

            if (BlurBackground && BGblur < 18)
            {
                BGblur = BGblur + 1;
            }

            if (!BlurBackground && BGblur > 0)
            {
                BGblur = BGblur - 1;
            }

            if (preDrawTexture != null)
            {
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.None, null, null, null))
                {
                    Solids.Instance.SpriteBatch.Draw(preDrawTexture, Solids.Instance.Bounds, null, Color.White);
                }

                if (DimBackground)
                {
                    using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, null, null, null))
                    {
                        Solids.Instance.SpriteBatch.DrawSolidRectangle(new FloatRectangle(Solids.Instance.Bounds), Color.Black * BGdim);
                    }
                }

                if (BlurBackground)
                {
                    Solids.GaussianBlur.DoBlur(preDrawTexture, BGblur, 6f);
                }
            }

            if (BGdim > 0)
            {
                Solids.Instance.SpriteBatch.DrawSolidRectangle(new FloatRectangle(Solids.Instance.Bounds), Color.Black * BGdim);
            }

            if (WindowClip != null)
            {
                Solids.Instance.SpriteBatch.Scissor = Screen.Translate(WindowClip).ToRectangle();

            }

            DrawAssets(AllAssets, opacity, preDrawTexture);
            DrawAssets(RealTime.Select(x => (DataboundAsset)x).ToList(), opacity, preDrawTexture);

            Draw(gameTime);

            Solids.Instance.SpriteBatch.Scissor = null;

            return null;
        }

        void DrawAssets(List<DataboundAsset> assets, float opacity, Texture2D preDrawTexture)
        {
            foreach (DataboundAsset asset in assets.Where(t => !t.IsHidden.Value))
            {
                Solids.Instance.SpriteBatch.Scissor = asset.Clip.ToRectangle();

                //if (asset is DataboundContainterAsset containterAsset)
                //{
                //    containterAsset.SetChildrenOriginToMyOrigin();
                //}

                asset.Draw(Solids.Instance.SpriteBatch, Screen, opacity, asset.Clip, preDrawTexture);
                //if (asset is DataboundContainterAsset)
                //{
                //   DrawAssets(((DataboundContainterAsset)asset).Items.Value.Select(x=> (ScreenAsset)x).ToList(), opacity,preDrawTexture);
                //}
            }
        }

        public void SetBounds(Rectangle viewportBounds)
        {
            BoundsSet = true;
            var orig = viewportBounds;

            if (FixedAspectRatio != 0)
            {
                float w = viewportBounds.Width;
                float h = viewportBounds.Height;

                float ah = w / FixedAspectRatio;
                float aw = h * FixedAspectRatio;

                if (w < aw)
                {
                    viewportBounds.Width = (int)w;
                    viewportBounds.Height = (int)ah;
                }
                else
                {
                    viewportBounds.Width = (int)aw;
                    viewportBounds.Height = (int)h;
                }


                if (viewportBounds.Width < orig.Width)
                {
                    viewportBounds.X = (orig.Width - viewportBounds.Width) / 2;
                }

                if (viewportBounds.Height < orig.Height)
                {
                    viewportBounds.Y = (orig.Height - viewportBounds.Height) / 2;
                }
            }

            Bounds = viewportBounds;
            Screen.SetBounds(viewportBounds);
        }

        public void CloseMe()
        {
            this.CloseRequested = true;
        }


        public Rectangle Bounds;

        public BaseScreen()
        {
            BindingEvents = new InputBindingEvents(InputBindings.UserInterface.ClickButton, HandleClick);
            BindingEvents.Add(InputBindings.UserInterface.ButtonDown, HandlePress);
            BindingEvents.Add(InputBindings.UserInterface.ButtonDown, HandleRelease);
        }

        public void FireCloseEvent()
        {
            OnCloseScreen?.Invoke();
        }
    }
}
