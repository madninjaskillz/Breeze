using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;
using Breeze.FontSystem;
using Breeze.Screens;
using Breeze.Services.InputService;
using Breeze.AssetTypes.StaticTemplates;
using Breeze.AssetTypes.XMLClass;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Breeze.AssetTypes
{
    public class TextboxAsset : InteractiveAsset
    {
        private bool isPasswordField = false;
        private bool dirty = true;
        public bool EditMode { get; set; }
        public DataboundValue<MDL2Symbols?> Symbol { get; set; } = new DataboundValue<MDL2Symbols?>();
        public DataboundValue<bool> Enabled { get; set; } = new DataboundValue<bool>(true);

        public DataboundValue<float> FontMargin { get; set; } = new DataboundValue<float>();
        public DataboundValue<string> Text { get; set; } = new DataboundValue<string>();


        public bool EnabledInit
        {
            get => Enabled.Value;
            set => Enabled.Value = value;
        }

        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            
            if (childNodeAttributes.GetNamedItem("Position")?.Value != null)
                Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
            if (childNodeAttributes.GetNamedItem("Margin")?.Value != null)
                Margin = UIElement.GetDBValue<Thickness>(childNodeAttributes.GetNamedItem("Margin")?.Value);
            if (childNodeAttributes.GetNamedItem("Symbol")?.Value != null)
                Symbol = UIElement.GetDBValue<MDL2Symbols?>(childNodeAttributes.GetNamedItem("Symbol")?.Value);
            if (childNodeAttributes.GetNamedItem("Enabled")?.Value != null)
                Enabled = UIElement.GetDBValue<bool>(childNodeAttributes.GetNamedItem("Enabled")?.Value);
            if (childNodeAttributes.GetNamedItem("FontMargin")?.Value != null)
                FontMargin = UIElement.GetDBValue<float>(childNodeAttributes.GetNamedItem("FontMargin")?.Value);
            if (childNodeAttributes.GetNamedItem("Text")?.Value != null)
                Text = UIElement.GetDBValue<string>(childNodeAttributes.GetNamedItem("Text")?.Value);
        }

        [JsonIgnore]
        public DataboundEvent<ButtonClickEventArgs> Clicked { get; set; } = new DataboundEvent<ButtonClickEventArgs>();

        public object Tag { get; set; }

        public TextboxAsset()
        {
        }


        public TextboxAsset(FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null, bool isPassword = false)
        {
            CreateButtonAsset(position, text, clickAction, fontMargin, backgroundColor, hoverColor, isPassword: isPassword);
        }

        private void CreateButtonAsset(FloatRectangle position, string text, Action<ButtonClickEventArgs> clickAction = null, float fontMargin = 0, Color? backgroundColor = null, Color? hoverColor = null, FontFamily font = null, bool isPassword = false)
        {
            isPasswordField = isPassword;
            if (font == null)
            {
                font = Solids.Instance.Fonts.SegoeLight;
            }

            Position.Value = position;

            FontMargin.Value = fontMargin;

            Text.Value = text;

            Clicked.Action = clickAction;

            this.Position.SetChangeAction(() => dirty = true);
            this.State.SetChangeAction(() => dirty = true);
            this.Symbol.SetChangeAction(() => dirty = true);
            this.Enabled.SetChangeAction(() => dirty = true);

        }

        private int currentCursorPos = 0;
        private void HandleKBInput()
        {
            if (EditMode)
            {
                KeyboardState ks = Keyboard.GetState();
                var keys = Solids.Instance.InputService.JustPressedKeys();
                foreach (var k in keys)
                {
                    CheckKeys(k, ks);
                }

                foreach (char inputServicePressedChar in Solids.Instance.InputService.PressedChars)
                {
                    AddTextAtCaret(inputServicePressedChar.ToString());
                }
            }
        }


        public void AddTextAtCaret(string txt)
        {
            if (Text.Value == null)
            {
                Text.Value=String.Empty;
            }

            Text.Value = Text.Value.Insert(currentCursorPos, txt);
            currentCursorPos = currentCursorPos + txt.Length;
        }

        public void CheckKeys(Keys key, KeyboardState ks)
        {
            bool shift = Solids.Instance.InputService.IsPressed(InputService.ShiftKeys.LeftShift1) || Solids.Instance.InputService.IsPressed(InputService.ShiftKeys.RightShift1);

            last = key;
            repeat = 0;

            switch (key)
            {
                case Keys.Enter:
                case Keys.Escape:
                    EditMode = false;
                    //todo - fire event here?

                    break;

                case Keys.Back:
                    if (currentCursorPos > 0)
                    {
                        string tst = Text.Value.Substring(0, currentCursorPos - 1) + Text.Value.Substring(currentCursorPos);
                        Text.Value = tst;
                        currentCursorPos--;
                    }
                    break;


                case (Keys.Delete):
                    {
                        if (currentCursorPos < Text.Value.Length - 1)
                        {
                            string tst = Text.Value.Substring(0, currentCursorPos) + Text.Value.Substring(currentCursorPos + 1);
                            Text.Value = tst;
                        }
                        break;
                    }

                case (Keys.Left): currentCursorPos = Math.Max(0, currentCursorPos - 1); break;
                case (Keys.Right): currentCursorPos = Math.Min(Text.Value.Length, currentCursorPos + 1); break;
            }
        }

        int repeat;
        Keys last;



        List<DataboundAsset> assetscache = new List<DataboundAsset>();
        private string assetscachekey = "";



        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            HandleKBInput();

            FloatRectangle? tclip = screen.Translate(clip);
            FloatRectangle possy = ActualPosition;//.Clamp(clip);

            List<DataboundAsset> assets = null;
            string key = Text.Value + State.Value.ToString() + ActualPosition.ToString() + Enabled.Value.ToString() + EditMode.ToString()+ currentCursorPos;

            if (dirty || assetscache == null || assetscache.Count == 0 || assetscachekey != key || EditMode)
            {
         
                assets = new List<DataboundAsset>();


                ButtonVisualDescriptor currentVisual = TextBox.Template.GetState(State.Value, Enabled.Value);

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

                string textToDisplay = currentVisual.Text;
                if (string.IsNullOrWhiteSpace(textToDisplay))
                {
                    textToDisplay = Text.Value;
                }
                if (isPasswordField)
                {
                    textToDisplay = new String('•', textToDisplay.Length);
                }

                if (!string.IsNullOrEmpty(textToDisplay) || EditMode)
                {
                    var all = currentVisual.TextJustification;
                    float xx = 0;
                    if (Symbol.Value != null)
                    {
                        xx = Position.Value.Height * 0.75f;
                        all = FontAsset.FontJustification.Left;
                    }

                    FloatRectangle fontPos = new FloatRectangle(possy.X + xx, ActualPosition.Y + FontMargin.Value, possy.Width, possy.Height - (FontMargin.Value * 2f));


                    string ttd = "";
                    if (Text.Value != null)
                    {
                        ttd = Text.Value;
                    }

                    if (!EditMode)
                    {
                        assets.Add(new FontAsset(ttd, currentVisual.FontColor, fontPos, currentVisual.FontFamily, all) {Clip = clip});

                    }
                    else
                    {
                        
                        if (isPasswordField)
                        {

                        }

                        fontPos = fontPos.Move(new Vector2(0.01f, 0));

                        var tFontPos = screen.Translate(fontPos);
                        var fnt = currentVisual.FontFamily.GetFont((int) tFontPos.Value.Height);
                        float afterOffset = 0;
                        string before = "";
                        if (currentCursorPos > 0)
                        {
                            before = textToDisplay.Substring(0, currentCursorPos);
                            Vector2 res = fnt.MeasureString(before);
                            afterOffset = res.X / screen.bounds.Width;
                        }

                        assets.Add(new FontAsset(before, currentVisual.FontColor, fontPos, currentVisual.FontFamily, all) {Clip = clip});

                        string after = textToDisplay;
                        if (currentCursorPos > 0)
                        {
                            after = textToDisplay.Substring(currentCursorPos);
                        }

                        assets.Add(new FontAsset(" " + after, currentVisual.FontColor, fontPos.Move(new Vector2(afterOffset, 0)), currentVisual.FontFamily, all) {Clip = clip});
                        if (DateTime.Now.Millisecond < 500)
                        {
                            assets.Add(new FontAsset(MDL2Symbols.Caret.AsChar(), currentVisual.FontColor*0.5f, fontPos.Move(new Vector2(afterOffset-0.01f, 0)), Solids.Instance.Fonts.MDL2, all) {Clip = clip});
                        }
                        else
                        {
                            assets.Add(new FontAsset(MDL2Symbols.Caret.AsChar(), currentVisual.FontColor * 0.25f, fontPos.Move(new Vector2(afterOffset - 0.01f, 0)), Solids.Instance.Fonts.MDL2, all) { Clip = clip });
                        }

                    }
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
                screenAsset.Draw(spriteBatch, screen, opacity, clip, bgTexture);
            }
        }
    }
}
