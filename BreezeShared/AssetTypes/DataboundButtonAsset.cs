using System;
using System.Collections.Generic;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.FontSystem;
using Breeze.Screens;
using Breeze.AssetTypes.StaticTemplates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Breeze.AssetTypes
{

    public class StaticButtonAsset : InteractiveAsset
    {
        private bool dirty = true;

        public DataboundValue<MDL2Symbols?> Symbol { get; set; } = new DataboundValue<MDL2Symbols?>();
        public DataboundValue<bool> Enabled { get; set; } = new DataboundValue<bool>(true);
        public DataboundValue<ButtonVisualDescriptor> Normal { get; set; } = new DataboundValue<ButtonVisualDescriptor>();
        public DataboundValue<ButtonVisualDescriptor> Hover { get; set; } = new DataboundValue<ButtonVisualDescriptor>();
        public DataboundValue<ButtonVisualDescriptor> Pressing { get; set; } = new DataboundValue<ButtonVisualDescriptor>();
        public DataboundValue<ButtonVisualDescriptor> Disabled { get; set; } = new DataboundValue<ButtonVisualDescriptor>();
        public DataboundValue<float> FontMargin { get; set; } = new DataboundValue<float>();
        public DataboundValue<string> Text { get; set; } = new DataboundValue<string>();

        public float FontMarginInit
        {
            get => FontMargin.Value;
            set => FontMargin.Value = value;
        }

        public bool EnabledInit
        {
            get => Enabled.Value;
            set => Enabled.Value = value;
        }



//        [JsonIgnore]
        public DataboundEvent<ButtonClickEventArgs> Clicked { get; set; } = new DataboundEvent<ButtonClickEventArgs>();


  //      [JsonIgnore]
        public DataboundEvent<ButtonClickEventArgs> ButtonDown { get; set; } = new DataboundEvent<ButtonClickEventArgs>();


        //[JsonIgnore]
        public DataboundEvent<ButtonClickEventArgs> ButtonUp { get; set; } = new DataboundEvent<ButtonClickEventArgs>();

        public object Tag { get; set; }

        public StaticButtonAsset()
        {
        }

        public StaticButtonAsset(FloatRectangle position, MDL2Symbols symbol, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        {
            CreateButtonAsset(position, symbol.AsChar(), clickAction, normal, hover, pressing, disabled, fontMargin, backgroundColor, hoverColor, Solids.Instance.Fonts.MDL2);

            //Position.SetChangeAction(() => { base.Position = this.Position.Value; });
        }

        public StaticButtonAsset(StaticTemplate template, FloatRectangle position, MDL2Symbols symbol, string text, Action<ButtonClickEventArgs> clickAction = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        {
            Symbol.Value = symbol;
            CreateButtonAsset(position, text, clickAction, template.Normal, template.Hover, template.Pressing, template.Disabled, fontMargin, backgroundColor, hoverColor, Solids.Instance.Fonts.MDL2);
        }

        public StaticButtonAsset(StaticTemplate template, FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        {
            CreateButtonAsset(position, text, clickAction, template?.Normal, template?.Hover, template?.Pressing, template?.Disabled, fontMargin, backgroundColor, hoverColor);
        }

        public StaticButtonAsset(FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null)
        {
            CreateButtonAsset(position, text, clickAction, normal, hover, pressing, disabled, fontMargin, backgroundColor, hoverColor);
        }


        private void CreateButtonAsset(FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, ButtonVisualDescriptor normal = null, ButtonVisualDescriptor hover = null, ButtonVisualDescriptor pressing = null, ButtonVisualDescriptor disabled = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null, FontFamily font = null)
        {
            if (font == null)
            {
                font = Solids.Instance.Fonts.EuroStile;
            }

            Position.Value = position;

            Normal.Value = normal;
            Hover.Value = hover;
            Pressing.Value = pressing;
            Disabled.Value = disabled;
            FontMargin.Value = fontMargin;

            Text.Value = text;

            Clicked.Action = clickAction;

            if (Normal.Value == null)
            {
                Normal.Value = new ButtonVisualDescriptor
                {
                    FontFamily = font,
                    BackgroundColor = backgroundColor ?? Color.Black * 0.68f,
                    Text = text,
                    BorderBrushSize = 0,
                    BorderColor = Color.LightGray,
                    FontColor = Color.White,
                    FontScale = 1f,
                    TextJustification = FontAsset.FontJustification.Center,
                    BlurAmount = 12
                };
            }
            else
            {
                if (Normal.Value.Text == null) Normal.Value.Text = text;
            }

            if (Hover.Value == null)
            {
                Hover.Value = new ButtonVisualDescriptor
                {
                    FontFamily = font,
                    BackgroundColor = hoverColor ?? Color.Green * 0.5f,
                    Text = text,
                    BorderBrushSize = 0,
                    BorderColor = Color.Purple,
                    FontColor = Color.White,
                    FontScale = 1f,
                    TextJustification = FontAsset.FontJustification.Center,
                    ShadowDepth = 0.095f,
                    BlurAmount = 20
                };
            }
            else
            {
                if (Hover.Value.Text == null) Hover.Value.Text = text;
            }

            if (Pressing.Value == null)
            {
                Pressing.Value = new ButtonVisualDescriptor
                {
                    FontFamily = font,
                    BackgroundColor = Color.Purple,
                    Text = text,
                    BorderBrushSize = 0,
                    BorderColor = Color.Purple,
                    FontColor = Color.White,
                    FontScale = 1f,
                    TextJustification = FontAsset.FontJustification.Center,
                    ShadowDepth = 0.045f,
                    BlurAmount = 10
                };
            }
            else
            {
                if (Pressing.Value.Text == null) Pressing.Value.Text = text;
            }


            if (Disabled.Value == null)
            {
                Disabled.Value = new ButtonVisualDescriptor
                {
                    FontFamily = font,
                    BackgroundColor = backgroundColor ?? Color.Black * 0.48f,
                    Text = text,
                    BorderBrushSize = 0,
                    BorderColor = Color.LightGray,
                    FontColor = Color.White * 0.5f,
                    FontScale = 1f,
                    TextJustification = FontAsset.FontJustification.Center,
                    BlurAmount = 12
                };
            }
            else
            {
                if (Disabled.Value.Text == null) Disabled.Value.Text = text;
            }

            this.Position.SetChangeAction(() => dirty = true);
            this.Text.SetChangeAction(TextChangeEvent);
            this.State.SetChangeAction(() => dirty = true);
            this.Symbol.SetChangeAction(() => dirty = true);
            this.Enabled.SetChangeAction(() => dirty = true);

        }

        private void TextChangeEvent()
        {
            Normal.Value.Text = Text.Value;
            Hover.Value.Text = Text.Value;
            Pressing.Value.Text = Text.Value;
            Disabled.Value.Text = Text.Value;
        }

        List<DataboundAsset> assetscache = new List<DataboundAsset>();
        private string assetscachekey = "";



        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            FloatRectangle? tclip = screen.Translate(clip);
            FloatRectangle possy = ActualPosition;//.Clamp(clip);

            List<DataboundAsset> assets = null;
            string key = Text.Value + State.Value.ToString() + Position.Value.ToString() + Enabled.Value.ToString();

            if (dirty || assetscache == null || assetscache.Count == 0 || assetscachekey != key)
            {
                assets = new List<DataboundAsset>();


                ButtonVisualDescriptor currentVisual = null;
                switch (State.Value)
                {
                    case ButtonState.Normal:
                        {
                            currentVisual = Normal.Value;
                            break;
                        }
                    case ButtonState.Hover:
                        {
                            currentVisual = Hover.Value;
                            break;
                        }
                    case ButtonState.Pressing:
                        {
                            currentVisual = Pressing.Value;
                            break;
                        }
                }

                if (!Enabled.Value)
                {
                    currentVisual = Disabled.Value;
                }

                if (currentVisual.ShadowDepth > 0)
                {
                    assets.Add(new BoxShadowAsset(Color.White, currentVisual.ShadowDepth, possy) { Clip = clip });
                }

                if (!string.IsNullOrEmpty(currentVisual?.BackgroundTexture))
                {
                    assets.Add(new RectangleAsset(currentVisual.BackgroundColor, 0, possy, currentVisual.BackgroundTexture, null, currentVisual.BackgroundTileMode) { Clip = clip });
                }

                assets.Add(new RectangleAsset(currentVisual.BorderColor, 0, possy, currentVisual.BackgroundColor, currentVisual.BlurAmount) { Clip = clip });



                if (Symbol.Value != null)
                {
                    FloatRectangle fontPos = new FloatRectangle(FontMargin.Value + possy.X, possy.Y + FontMargin.Value, ActualPosition.Width, ActualPosition.Height - (FontMargin.Value * 2f));
                    assets.Add(new FontAsset(Symbol.Value.Value.AsChar(), currentVisual.FontColor, fontPos, Solids.Instance.Fonts.MDL2, FontAsset.FontJustification.Left) { Clip = tclip });
                }

                if (!string.IsNullOrEmpty(currentVisual.Text))
                {
                    var all = currentVisual.TextJustification;
                    float xx = 0;
                    if (Symbol != null)
                    {
                        xx = ActualPosition.Height * 0.75f;
                        all = FontAsset.FontJustification.Left;
                    }

                    FloatRectangle fontPos = new FloatRectangle(possy.X + xx, ActualPosition.Y + FontMargin.Value, possy.Width, possy.Height - (FontMargin.Value * 2f));
                    assets.Add(new FontAsset(Text.Value, currentVisual.FontColor, fontPos, currentVisual.FontFamily, all) { Clip = clip });
                }




                if (currentVisual.BorderBrushSize > 0)
                {
                    assets.Add(new RectangleAsset(currentVisual.BorderColor, currentVisual.BorderBrushSize, possy)
                    {
                        Clip = clip,
                    });
                }

                dirty = false;

                assetscache = assets;
                assetscachekey = key;
            }
            else
            {
                assets = assetscache;
            }

            foreach (DataboundAsset screenAsset in assets)
            {

                screenAsset.Draw(screenResources, spriteBatch, screen, opacity, clip, bgTexture);
            }
        }
    }
}
