using System;
using System.Collections.Generic;
using System.Diagnostics;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;
using Breeze.FontSystem;
using Breeze.Screens;
using Breeze.Services.InputService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class ButtonClickEventArgs
    {
        public ClickSource ClickSource { get; set; }
        public Vector2 ClickPosition { get; set; }
        public ButtonAsset ButtonAsset { get; set; }

        public object Sender { get; set; }

    }
    public enum ClickSource
    {
        Keyboard,
        GamePad,
        Mouse,
        TouchScreen
    }
    public class ButtonHelpers : DataboundAsset
    {
        public MDL2Symbols? Symbol { get; set; }
        public bool Enabled { get; set; } = true;

        //public void Update(_ButtonAsset source)
        //{
        //    this.Position = source.Position;
        //    this.Symbol = source.Symbol;
        //    this.ClickAction = source.ClickAction;
        //    this.Enabled = source.Enabled;
        //    this.Normal = source.Normal;
        //    this.Hover = source.Hover;
        //    this.Disabled = source.Disabled;
        //    this.Pressing = source.Pressing;

            
        //}

        public static List<StaticButtonAsset> CreateButtonStack(Vector2 position, float width, float height, float margin, List<ButtonBasics> buttonBasics, float fontMargin = 0)
        {
            List<StaticButtonAsset> result = new List<StaticButtonAsset>();

            Vector2 currentPos = position;
            foreach (ButtonBasics bb in buttonBasics)
            {
                var txt = bb.Text;
                var theight = height;
                if (bb.Text == "---")
                {
                    txt = "-------------------------";
                    theight = theight / 4;
                }
                var buttonAsset = new StaticButtonAsset(new FloatRectangle(currentPos.X, currentPos.Y, width, theight), txt, (args) => { Debug.WriteLine(bb.Text + " clicked"); }, fontMargin: fontMargin);
                if (bb.Symbol != null)
                {
                    buttonAsset.Symbol.Value = bb.Symbol;
                }

                //if (bb.Text == "---")
                //{
                //    buttonAsset.
                //}

                if (bb.ClickAction != null)
                {
                    buttonAsset.Clicked.Action = bb.ClickAction;
                }
                else
                {
                    buttonAsset.Clicked.Action = (args) => { Debug.WriteLine(bb.Text + " autoclicked"); };
                }
                buttonAsset.Enabled.Value = bb.Enabled;

                result.Add(buttonAsset);
                currentPos = new Vector2(currentPos.X, currentPos.Y + theight + margin);
            }

            return result;
        }

        public static StaticButtonAsset CreateButton(FloatRectangle position, ButtonBasics bb, float fontMargin = 0)
        {
            var buttonAsset = new StaticButtonAsset(position, bb.Text, (args) => { Debug.WriteLine(bb.Text + " clicked"); }, fontMargin: fontMargin);
            if (bb.Symbol != null)
            {
                buttonAsset.Symbol.Value = bb.Symbol;
            }

            //if (bb.Text == "---")
            //{
            //    buttonAsset.
            //}

            if (bb.ClickAction != null)
            {
                buttonAsset.Clicked.Action = bb.ClickAction;
            }
            else
            {
                buttonAsset.Clicked.Action = (args) => { Debug.WriteLine(bb.Text + " autoclicked"); };
            }
            buttonAsset.Enabled.Value = bb.Enabled;

            if (bb.OverrideBgColor != null)
            {
                buttonAsset.Normal.Value.BackgroundColor = bb.OverrideBgColor.Value;
            }

            return buttonAsset;
        }

        public static List<StaticButtonAsset> CreateButtonHorizontalStack(FloatRectangle position, float height, float margin, List<ButtonBasics> buttonBasics, float fontMargin = 0)
        {
            return CreateButtonHorizontalStack(new Vector2(position.X, position.Y), ((position.Width) / buttonBasics.Count) - (buttonBasics.Count * margin), height, margin, buttonBasics, fontMargin);
        }

        public static List<StaticButtonAsset> CreateButtonHorizontalStack(Vector2 position, float width, float height, float margin, List<ButtonBasics> buttonBasics, float fontMargin = 0)
        {
            List<StaticButtonAsset> result = new List<StaticButtonAsset>();

            Vector2 currentPos = position + new Vector2(margin, 0);
            foreach (ButtonBasics bb in buttonBasics)
            {
                var buttonAsset = new StaticButtonAsset(new FloatRectangle(currentPos.X, currentPos.Y, width- margin, height), bb.Text, fontMargin: fontMargin);
                buttonAsset.Clicked.Action = bb.ClickAction;
                buttonAsset.Enabled.Value = bb.Enabled;
                result.Add(buttonAsset);
                currentPos = new Vector2(currentPos.X + width + margin * 2, currentPos.Y);
            }

            return result;
        }

        public static float GetButtonStackHeight(float height, float margin,
            List<ButtonBasics> buttonBasics)
        {


            float currentPos = 0;
            foreach (ButtonBasics bb in buttonBasics)
            {

                currentPos = currentPos + height + margin;
            }

            return currentPos - margin;
        }

        public ButtonVisualDescriptor Normal { get; set; }
        public ButtonVisualDescriptor Hover { get; set; }
        public ButtonVisualDescriptor Pressing { get; set; }
        public float FontMargin { get; set; }
        public string Text { get; set; }

        public ButtonState State { get; set; }
        private Action<ButtonClickEventArgs> cAction;

        public Action<ButtonClickEventArgs> ClickAction
        {
            get { return cAction; }
            set
            {
                cAction = value;
            }
        }

        public object Tag { get; set; }

        //public ButtonHelpers(FloatRectangle position, MDL2Symbols symbol, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        //{
        //    CreateButtonAsset(position, symbol.AsChar(), clickAction, normal, hover, pressing, disabled, fontMargin, backgroundColor, hoverColor, Solids.FontFamilies.MDL2);
        //}

        //public ButtonHelpers(FloatRectangle position, MDL2Symbols symbol,string text, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        //{
        //    Symbol = symbol;
        //    CreateButtonAsset(position, text, clickAction, normal, hover, pressing, disabled, fontMargin, backgroundColor, hoverColor, Solids.FontFamilies.MDL2);
        //}

        //public ButtonHelpers(FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        //{
        //    CreateButtonAsset(position, text, clickAction, normal, hover, pressing, disabled, fontMargin, backgroundColor, hoverColor);
        //}

        //private void CreateButtonAsset(FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null, FontFamily font = null)
        //{
        //    if (font == null)
        //    {
        //        font = Solids.FontFamilies.EuroStile;
        //    }

        //    Position = position;

        //    Normal = normal;
        //    Hover = hover;
        //    Pressing = pressing;
        //    Disabled = disabled;
        //    FontMargin = fontMargin;

        //    Text = text;

        //    ClickAction = clickAction;

        //    if (Normal == null)
        //    {
        //        Normal = new ButtonVisualDescriptor
        //        {
        //            FontFamily = font,
        //            BackgroundColor = backgroundColor ?? Color.Black * 0.68f,
        //            Text = text,
        //            BorderBrushSize = 0,
        //            BorderColor = Color.LightGray,
        //            FontColor = Color.White,
        //            FontScale = 1f,
        //            TextJustification = FontAsset.FontJustification.Center,
        //            BlurAmount = 12
        //        };
        //    }
        //    else
        //    {
        //        if (Normal.Text == null) Normal.Text = text;
        //    }

        //    if (Hover == null)
        //    {
        //        Hover = new ButtonVisualDescriptor
        //        {
        //            FontFamily = font,
        //            BackgroundColor = hoverColor ?? Color.Green * 0.5f,
        //            Text = text,
        //            BorderBrushSize = 0,
        //            BorderColor = Color.Purple,
        //            FontColor = Color.White,
        //            FontScale = 1f,
        //            TextJustification = FontAsset.FontJustification.Center,
        //            ShadowDepth = 0.095f,
        //            BlurAmount = 20
        //        };
        //    }
        //    else
        //    {
        //        if (Hover.Text == null) Hover.Text = text;
        //    }

        //    if (Pressing == null)
        //    {
        //        Pressing = new ButtonVisualDescriptor
        //        {
        //            FontFamily = font,
        //            BackgroundColor = Color.Purple,
        //            Text = text,
        //            BorderBrushSize = 0,
        //            BorderColor = Color.Purple,
        //            FontColor = Color.White,
        //            FontScale = 1f,
        //            TextJustification = FontAsset.FontJustification.Center,
        //            ShadowDepth = 0.045f,
        //            BlurAmount = 10
        //        };
        //    }
        //    else
        //    {
        //        if (Pressing.Text == null) Pressing.Text = text;
        //    }


        //    if (Disabled == null)
        //    {
        //        Disabled = new ButtonVisualDescriptor
        //        {
        //            FontFamily = font,
        //            BackgroundColor = backgroundColor ?? Color.Black * 0.48f,
        //            Text = text,
        //            BorderBrushSize = 0,
        //            BorderColor = Color.LightGray,
        //            FontColor = Color.White*0.5f,
        //            FontScale = 1f,
        //            TextJustification = FontAsset.FontJustification.Center,
        //            BlurAmount = 12
        //        };
        //    }
        //    else
        //    {
        //        if (Disabled.Text == null) Disabled.Text = text;
        //    }
        //}

        public ButtonVisualDescriptor Disabled { get; set; }

        List<DataboundAsset> assetscache = new List<DataboundAsset>();
        private string assetscachekey = "";

        //public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        //{

        //    if (Solids.Instance.InputService.IsPressed(InputService.ShiftKeys.LeftShift1))
        //    {
        //        return;

        //    }
        //    //ScreenAbstractor fakeScreen = new ScreenAbstractor();
        //    //fakeScreen.SetBounds(screen.Translate(Position).ToRectangle());
        //    Rectangle ps = screen.Translate(Position).ToRectangle();
        //    FloatRectangle? tclip = screen.Translate(clip);
        //    FloatRectangle possy = Position;//.Clamp(clip);

        //    if (scrollOffset.HasValue)
        //    {
        //        // possy.Move(scrollOffset);
        //    }


        //    List<ScreenAsset> assets = null;

        //    string key = State.ToString() + Position.ToString();




        //    if (assetscachekey == key)
        //    {
        //        assets = assetscache;
        //    }
        //    else
        //    {
        //        assets = new List<ScreenAsset>();


        //        ButtonVisualDescriptor currentVisual = null;
        //        switch (State)
        //        {
        //            case ButtonState.Normal:
        //                {
        //                    currentVisual = Normal;
        //                    break;
        //                }
        //            case ButtonState.Hover:
        //                {
        //                    currentVisual = Hover;
        //                    break;
        //                }
        //            case ButtonState.Pressing:
        //                {
        //                    currentVisual = Pressing;
        //                    break;
        //                }
        //        }
                
        //        if (!Enabled)
        //        {
        //            currentVisual = Disabled;
        //        }

        //        if (currentVisual.ShadowDepth > 0)
        //        {
        //            assets.Add(new BoxShadowAsset(Color.White, currentVisual.ShadowDepth, possy) { Clip = clip });
        //        }

        //        if (!string.IsNullOrEmpty(currentVisual?.BackgroundTexture))
        //        {
        //            assets.Add(new RectangleAsset(currentVisual.BackgroundColor, 0, possy, currentVisual.BackgroundTexture, null, currentVisual.BackgroundTileMode) { Clip = clip });
        //        }

        //        assets.Add(new RectangleAsset(currentVisual.BorderColor, 0, possy, currentVisual.BackgroundColor, currentVisual.BlurAmount) { Clip = clip });



        //        if (Symbol != null)
        //        {
        //            FloatRectangle fontPos = new FloatRectangle(FontMargin + possy.X, possy.Y + FontMargin, Position.Width, Position.Height - (FontMargin * 2f));
        //            assets.Add(new FontAsset(Symbol.Value.AsChar(), currentVisual.FontColor, fontPos, Solids.FontFamilies.MDL2, FontAsset.FontJustification.Left) { Clip = tclip });
        //        }

        //        if (!string.IsNullOrEmpty(currentVisual.Text))
        //        {
        //            var all = currentVisual.TextJustification;
        //            float xx = 0;
        //            if (Symbol != null)
        //            {
        //                xx = Position.Height*0.75f;
        //                all = FontAsset.FontJustification.Left;
        //            }

        //            FloatRectangle fontPos = new FloatRectangle(possy.X + xx, Position.Y + FontMargin, possy.Width, possy.Height - (FontMargin * 2f));
        //            assets.Add(new FontAsset(currentVisual.Text, currentVisual.FontColor, fontPos, currentVisual.FontFamily, all) { Clip = clip });
        //        }




        //        if (currentVisual.BorderBrushSize > 0)
        //        {
        //            assets.Add(new RectangleAsset(currentVisual.BorderColor, currentVisual.BorderBrushSize, possy)
        //            {
        //                Clip = clip,
        //            });
        //        }



        //        assetscache = assets;
        //        assetscachekey = key;
        //    }

        //    foreach (ScreenAsset screenAsset in assets)
        //    {
        //        //Solids.Instance.SpriteBatch.DoEnd();
        //        //Solids.Instance.SpriteBatch.Scissor = this.ScissorRect;
        //        //Solids.Instance.SpriteBatch.DoEnd();

        //        screenAsset.Draw(spriteBatch, screen, opacity, clip, bgTexture);
        //    }
        //}
    }


    public enum ButtonState
    {
        Normal,
        Hover,
        Pressing
    }
}
