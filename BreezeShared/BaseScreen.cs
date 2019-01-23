using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using Microsoft.Xna.Framework.Input;
using ButtonState = Breeze.AssetTypes.ButtonState;

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

        public class Resources
        {
            public Dictionary<string, string> TemplateXMLs = new Dictionary<string, string>();

            public DataboundAsset GetTemplate(string name)
            {
                if (TemplateXMLs.ContainsKey(name) == false)
                {
                    return null;
                }

                string xml = TemplateXMLs[name];

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                var fing = ScreenXMLHelpers.HandleChildren(this, xmlDoc);
                return fing.First();
            }
        }

        public Resources ScreenResources { get; set; } = new Resources();
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

        public DataboundAsset RootAsset => FixedAssets.FirstOrDefault();

        public VirtualizedDataContext RootContext
        {
            get => RootAsset?.VirtualizedDataContext;
            set => RootAsset.VirtualizedDataContext = value;
        }
        public void LoadXAML()
        {
            Type type = this.GetType();

            string path = "Screens\\" + type.Name.Substring(0, type.Name.Length - ("Screen").Length) + "\\" + type.Name + ".xml";

            string xmlTest = Solids.Instance.Storage.FileSystemStorage.ReadText(path);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTest);
            LoadScreen(xmlDoc);
        }

        public void LoadScreen(XmlNode node)
        {
            XmlNode firstChild = node.FirstChild;
            if (firstChild.Attributes.GetNamedItem("FixedAspectRatio")?.Value != null) FixedAspectRatio = float.Parse(node.FirstChild.Attributes.GetNamedItem("FixedAspectRatio")?.Value);

            List<DataboundAsset> assetsFromXml = ScreenXMLHelpers.HandleChildren(ScreenResources, firstChild);

            FixedAssets.AddRange(assetsFromXml);

            foreach (DataboundAsset databoundAsset in FixedAssets)
            {
                FixChildParentRelationShips(databoundAsset);
            }
        }

        public void FixChildParentRelationShips(DataboundAsset asset)
        {
            if (asset is DataboundContainterAsset containerAsset)
            {
                if (containerAsset.Children?.Value != null)
                {
                    foreach (DataboundAsset databoundAsset in containerAsset.Children.Value)
                    {
                        databoundAsset.ParentAsset = asset;
                    }
                }
            }
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
            dragOffset = args.ClickPosition;
            dragging = true;
        }


        internal void StopWindowDrag(ButtonClickEventArgs args)
        {
            dragOffset = null;
            dragging = false;
        }


        private bool dragging = false;
        private Vector2? dragOffset = null;

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

        private int allAssetHash = 0;
        public void UpdateAllAssets()
        {

            var tmp = FlattenTree(FixedAssets);
            var hash = tmp.Count;
            if (hash != allAssetHash)
            {
                AllAssets = tmp;
                allAssetHash = hash;
            }

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
                    if ((databoundAsset as T) != null)
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
            List<DataboundAsset> result = new List<DataboundAsset> { asset };

            if (asset is DataboundContainterAsset dbc)
            {
                foreach (DataboundAsset databoundAsset in GetVirtualChildren(dbc))
                {
                    result.AddRange(FlattenTree(databoundAsset));
                }
            }

            return result.Where(t => !(t is ContentAsset)).ToList();
        }

        public List<DataboundAsset> FlattenTree(List<DataboundAsset> assets)
        {
            return assets.SelectMany(FlattenTree).ToList();
        }

        public List<DataboundAsset> GetVirtualChildren(DataboundAsset screenAsset)
        {
            List<DataboundAsset> result = new List<DataboundAsset>();

            if (screenAsset is DataboundContainterAsset asset)
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
            foreach (DataboundAsset screenAsset in assets)
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
            UpdateAllAssets();
        }

        public virtual async Task Initialise()
        {
            LoadXAML();
            Screen = new ScreenAbstractor();
            Screen.SetBounds(Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds);


        }

        public void InitViewModel(VirtualizedDataContext vm)
        {
            this.RootAsset.FixBinds();

            this.RootContext = vm;
            this.RootContext.Screen = this;
            this.IsFullScreen = true;
            UpdateAllAssets();
            Update(new GameTime());
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

            UpdateAllAssets();

            mouseActive = Solids.Instance.InputService.MouseActive;
            if (mouseActive)
            {
                if (!AllAssets.Any(x => Solids.Instance.InputService.MouseScreenPosition.Intersects(x.ActualPosition)))
                {
                    ActiveButton = null;
                }
            }

            var tba = AllAssets.OfType<TextboxAsset>().Where(t => t.EditMode);
            if (HandleButtons || !tba.Any())
            {


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

                            if (ActiveButton != null)
                            {
                                Rectangle tthing = Screen.Translate(ActiveButton.ActualPosition).ToRectangle();
                                if (mx > tthing.X && my > tthing.Y && mx < tthing.Right && my < tthing.Bottom)
                                {
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                foreach (InteractiveAsset buttonAsset in AllAssets.OfType<InteractiveAsset>())
                                {
                                    Rectangle thing = Screen.Translate(buttonAsset.ActualPosition).ToRectangle();
                                    if (mx > thing.X && my > thing.Y && mx < thing.Right && my < thing.Bottom)
                                    {
                                        ActiveButton = buttonAsset;
                                        found = true;
                                    }
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
                            bool up = false;
                            bool down = false;
                            bool left = false;
                            bool right = false;

                            float angleWideness = 85;
                            if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIDown) || Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIUp) || Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UILeft) || Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIRight))
                            {
                                //Dictionary<InteractiveAsset, Vector2> buttonPositions = new Dictionary<InteractiveAsset, Vector2>();

                                List<Vector2> activeButtonPositions = new List<Vector2>();

                                List<ButtonPositionsListItem> buttonPositions = new List<ButtonPositionsListItem>();
                                foreach (InteractiveAsset buttonAsset in AllAssets.OfType<InteractiveAsset>())
                                {
                                    Rectangle rect = Screen.Translate(buttonAsset.ActualPosition).ToRectangle();
                                    Vector2 centerPos = new Vector2(rect.X + (rect.Width / 2f), rect.Y + (rect.Width / 2f));

                                    //buttonPositions.Add(new ButtonPositionsListItem(buttonAsset, centerPos));
                                    //buttonPositions.Add(new ButtonPositionsListItem(buttonAsset, new Vector2(rect.X, rect.Y)));
                                    //buttonPositions.Add(new ButtonPositionsListItem(buttonAsset, new Vector2(rect.Right, rect.Y)));

                                    //buttonPositions.Add(new ButtonPositionsListItem(buttonAsset, new Vector2(rect.X, rect.Bottom)));
                                    //buttonPositions.Add(new ButtonPositionsListItem(buttonAsset, new Vector2(rect.Right, rect.Bottom)));
                                }

                                activeButtonPositions = buttonPositions.Where(x => x.Asset == ActiveButton).Select(x => x.Position).ToList();

                                float direction = 0;
                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIUp))
                                {
                                    direction = -(float)Math.PI;
                                    up = true;
                                }

                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIRight))
                                {
                                    direction = (-(float)Math.PI) + (float)Math.PI * 0.5f;
                                    right = true;
                                }

                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UIDown))
                                {
                                    direction = (-(float)Math.PI) + (float)Math.PI;
                                    down = true;
                                }

                                if (Solids.Instance.InputService.JustPressed(InputService.ActionKeys.UILeft))
                                {
                                    direction = (-(float)Math.PI) + (float)Math.PI * 1.5f;
                                    left = true;
                                }

                                List<AssetScoring> scores = new List<AssetScoring>();
                                foreach (InteractiveAsset buttonAsset in AllAssets.OfType<InteractiveAsset>().Where(x => x != ActiveButton))
                                {
                                    var differenceVector = (ActiveButton.ActualPosition.Centre - buttonAsset.ActualPosition.Centre);
                                    float angle = differenceVector.ToAngle();
                                    var actualDistance = Math.Abs(Vector2.Distance(ActiveButton.ActualPosition.Centre, buttonAsset.ActualPosition.Centre));
                                    var angleDiff = direction.DifferenceBetweenAnglesInDegrees(angle);
                                    var distance = actualDistance * (1f + (10f * Math.Abs(angleDiff)));

                                    if (up || down)
                                    {
                                        if (ActiveButton.ActualPosition.Centre.X >= buttonAsset.ActualPosition.X &&
                                            ActiveButton.ActualPosition.Centre.X <= buttonAsset.ActualPosition.Right)
                                        {
                                            angleDiff = 0;
                                        }
                                    }

                                    int actuallyValid = 1;
                                    if (direction.DifferenceBetweenAnglesInDegrees(angle) > -angleWideness && direction.DifferenceBetweenAnglesInDegrees(angle) < angleWideness)
                                    {
                                        actuallyValid = 0;
                                    }

                                    if (left || right)
                                    {
                                        if (ActiveButton.ActualPosition.Centre.Y >= buttonAsset.ActualPosition.Y &&
                                            ActiveButton.ActualPosition.Centre.Y <= buttonAsset.ActualPosition.Bottom)
                                        {
                                            angleDiff = 0;
                                        }
                                    }

                                    scores.Add(new AssetScoring
                                    {
                                        Asset = buttonAsset,
                                        Angle = angle,
                                        AngleDiff = angleDiff,
                                        Distance = actualDistance,
                                        Score = distance,
                                        Valid = actuallyValid
                                    });
                                }

                                List<AssetScoring> ByScore = scores.OrderBy(x => x.Score).ToList();
                                List<AssetScoring> ByDistance = scores.OrderBy(x => x.Valid).ThenBy(x => x.Distance).ThenBy(x => x.AngleDiff).ThenBy(x => x.Asset.ActualPosition.X).ToList();
                                List<AssetScoring> ByAngleDiff = scores.OrderBy(x => x.Valid).ThenBy(x => x.AngleDiff).ThenBy(x => x.Distance).ThenBy(x => x.Asset.ActualPosition.X).ToList();

                                foreach (AssetScoring assetScoring in scores)
                                {
                                    assetScoring.CountScore = ByDistance.IndexOf(assetScoring) + ByAngleDiff.IndexOf(assetScoring);
                                }

                                List<AssetScoring> ByCountScore = scores.OrderBy(x => x.CountScore).ToList();

                                InteractiveAsset bestButton = ActiveButton;
                                
                                if (ByScore.Any())
                                {
                                    bestButton = ByCountScore.First().Asset;
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

        private class AssetScoring
        {
            public InteractiveAsset Asset { get; set; }

            public float AngleDiff { get; set; }
            public float Distance { get; set; }
            public float Score { get; set; }
            public float Angle { get; set; }
            public int CountScore { get; set; }
            public int Valid { get; set; }

            public override string ToString()
            {
                return $"{Asset.GetType()}, {Asset.ActualPosition.Centre} Angle:{Angle}, AD:{AngleDiff}, Dist:{Distance}, Score:{Score}, CountScore:{CountScore}";
            }
        }

        private class ButtonPositionsListItem
        {
            public InteractiveAsset Asset { get; set; }
            public Vector2 Position { get; set; }

            public ButtonPositionsListItem(InteractiveAsset asset, Vector2 pos)
            {
                Asset = asset;
                Position = pos;
            }
        }

        public Action NoActiveButtonClick;
        public void HandleClick()
        {
            if (ActiveButton == null)
            {
                NoActiveButtonClick?.Invoke();
                return;
            }

            ButtonClickEventArgs args = new ButtonClickEventArgs();
            if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.ActualPosition))
            {
                args.ClickSource = ClickSource.Mouse;

                args.ClickPosition =
                    new Vector2(
                        (Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.ActualPosition.X) / ActiveButton.ActualPosition.Width,
                        (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.ActualPosition.Y) / ActiveButton.ActualPosition.Height

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

            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                args.Sender = interactiveAsset;
                ActiveButton.InternalClickEvent?.Invoke(args);
                interactiveAsset.FireEvent(interactiveAsset.OnClickEvent, new object[] { args });
            }

        }


        public ButtonClickEventArgs GetClickArgs()
        {
            ButtonClickEventArgs args = new ButtonClickEventArgs();
            if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.ActualPosition))
            {
                args.ClickSource = ClickSource.Mouse;

                args.ClickPosition =
                    new Vector2(
                        (Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.ActualPosition.X) / ActiveButton.ActualPosition.Width,
                        (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.ActualPosition.Y) / ActiveButton.ActualPosition.Height
                    );
            }


            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                args.Sender = interactiveAsset;
            }

            return args;
        }

        public void HandlePress()
        {
            if (ActiveButton == null) return;

            var args = GetClickArgs();

            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                interactiveAsset.FireEvent(interactiveAsset.OnPressEvent, new object[] { args });
            }

        }

        public void HandleRelease()
        {
            if (ActiveButton == null) return;

            var args = GetClickArgs();

            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                interactiveAsset.FireEvent(interactiveAsset.OnReleaseEvent, new object[] { args });
            }

        }

        public void HandleRightStickUp()
        {
            if (ActiveButton == null) return;

            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                ButtonClickEventArgs args = new ButtonClickEventArgs();
                interactiveAsset.InternalStickUpEvent?.Invoke(args);
            }
        }

        public void HandleRightStickDown()
        {
            if (ActiveButton == null) return;

            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                ButtonClickEventArgs args = new ButtonClickEventArgs();
                interactiveAsset.InternalStickDownEvent?.Invoke(args);
            }
        }

        public void HandleHeld()
        {
            if (ActiveButton == null) return;

            ButtonClickEventArgs args = new ButtonClickEventArgs();
            if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.ActualPosition))
            {
                args.ClickSource = ClickSource.Mouse;

                args.ClickPosition =
                    new Vector2(
                        (Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.ActualPosition.X) / ActiveButton.ActualPosition.Width,
                        (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.ActualPosition.Y) / ActiveButton.ActualPosition.Height

                    );
            }

            if (ActiveButton is InteractiveAsset interactiveAsset)
            {
                args.Sender = interactiveAsset;
                ActiveButton.InternalPressEvent?.Invoke(args);
                //interactiveAsset.FireEvent(interactiveAsset.OnClickEvent, new object[] { args });
            }

        }


        //public void HandleRelease()
        //{
        //    if (ActiveButton == null) return;

        //    ButtonClickEventArgs args = new ButtonClickEventArgs();
        //    if (Solids.Instance.InputService.MouseScreenPosition.Intersects(ActiveButton.Position.Value))
        //    {
        //        args.ClickSource = ClickSource.Mouse;

        //        args.ClickPosition = new Vector2((Solids.Instance.InputService.MouseScreenPosition.X - ActiveButton.Position.Value.X) / ActiveButton.Position.Value.Width, (Solids.Instance.InputService.MouseScreenPosition.Y - ActiveButton.Position.Value.Y) / ActiveButton.Position.Value.Height);
        //    }

        //    if (ActiveButton is StaticButtonAsset bActiveButton)
        //    {

        //        Task backgroundWorkTask = Task.Run(() => bActiveButton?.ButtonUp?.Fire(args));

        //    }
        //}


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
            var nonHiddenAssets = assets.Where(t => !t.IsHiddenOrParentHidden());
            foreach (DataboundAsset asset in nonHiddenAssets)
            {
                Solids.Instance.SpriteBatch.Scissor = asset.Clip.ToRectangle();

                asset.Draw(ScreenResources, Solids.Instance.SpriteBatch, Screen, opacity, asset.Clip, preDrawTexture);

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
            BindingEvents.Add(InputBindings.UserInterface.ButtonHeld, HandleHeld);
            BindingEvents.Add(InputBindings.UserInterface.ButtonUp, HandleRelease);

            BindingEvents.Add(new InputService.InputBinding(new InputService.InputStack(new InputService.GamepadControl(Buttons.RightThumbstickUp, InputService.PressType.PressThenHolding))), HandleRightStickUp);
            BindingEvents.Add(new InputService.InputBinding(new InputService.InputStack(new InputService.GamepadControl(Buttons.RightThumbstickDown, InputService.PressType.PressThenHolding))), HandleRightStickDown);
        }

        public void FireCloseEvent()
        {
            OnCloseScreen?.Invoke();
        }
    }
}
