using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Breeze.FontSystem;
using Breeze.Helpers;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class FontAsset : DataboundAssetWhereChildIsContentAsset
    {
        private string renderedKey;

        private float fetchedHeight = 6;

        public override void LoadContent(string content)
        {
            Text = DataboundAssetExtensions.GetDBValue<string>(content.Trim());
        }

        public DataboundValue<FontJustification> Justification { get; set; } = new DataboundValue<FontJustification>();
        public DataboundValue<string> Text { get; set; } = new DataboundValue<string>(string.Empty, null);
        public DataboundValue<string> Symbol { get; set; } = new DataboundValue<string>(string.Empty, null);
        public DataboundValue<Color> FontColor { get; set; } = new DataboundValue<Color>(null);
        [Asset("FontFamily")]
        public DataboundValue<string> FontName { get; set; } = new DataboundValue<string>(null);
        public DataboundValue<int?> CaretPos { get; set; } = new DataboundValue<int?>();

        [Asset("AntiAlias")]
        public DataboundValue<bool> PseudoAntiAlias { get; set; } = new DataboundValue<bool>();
        public DataboundValue<bool> MultiLine { get; set; } = new DataboundValue<bool>();
        public DataboundValue<bool> WordWrap { get; set; } = new DataboundValue<bool>();
        public DataboundValue<Color?> OutlineColor { get; set; } = new DataboundValue<Color?>();
        public DataboundValue<float> FontMargin { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> FontSize { get; set; } = new DataboundValue<float>();
        public DataboundValue<float?> LineHeight { get; set; } = new DataboundValue<float?>();

        //public FontFamily FontFamily { get; set; }

        //private string font;
        //public string Font
        //{
        //    get { return font; }
        //    set
        //    {
        //        font = value;
        //        FontFamily = Solids.FontFamilies.Fonts[value];
        //    }
        //}

        public FontAsset()
        {

        }



        public FontAsset(string text, Color color, FloatRectangle position, FontFamily fontFamily, FontJustification justification = FontJustification.Left, int? caretPos = null, bool? multiLine = false)
        {
            Text.Value = text;
            FontColor.Value = color;
            Position.Value = position;


            if (fontFamily != null)
                FontName.Value = Solids.Instance.Fonts.Fonts.First(t => t.Value.FontName == fontFamily.FontName).Key;
            //FontFamily = fontFamily;

            Justification.Value = justification;

            CaretPos.Value = caretPos;
            if (multiLine.HasValue && multiLine.Value)
            {
                MultiLine.Value = true;
            }

            Text.SetChangeAction(() => fontKey = "x");
        }

        private float scale = 1f;
        private BMFont myFont = null;
        private string fontKey = "x";
        private Vector2 size;
        private bool autoSize = false;
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            string txt = Text.Value();
            
            if (!string.IsNullOrWhiteSpace(Symbol.Value()))
            {
                MDL2Symbols symbol = (MDL2Symbols)Enum.Parse(typeof(MDL2Symbols), Symbol.Value());
                txt = symbol.AsChar();
            }

            if (FontName.Value != null)
            {
                this.ActualSize = DrawFontAsset(spriteBatch, screen, opacity, txt, FontColor.Value,
                    ActualPosition, FontSize.Value, LineHeight.Value, FontName.Value, PseudoAntiAlias.Value, WordWrap.Value,
                    Justification.Value, FontMargin.Value, Margin.Value, OutlineColor.Value, Clip, bgTexture, scrollOffset, CaretPos.Value,
                    MultiLine.Value, ScissorRect);
            }


            if (Position.Value.Height == 0)
            {
                autoSize = true;
            }

            if (autoSize)
            {
                this.Position.Value = new FloatRectangle(Position.Value.X, Position.Value.Y, Position.Value.Width, ActualSize.Y);
            }
        }


        public enum FontJustification
        {
            Left,
            Center,
            Right
        }

        public static Vector2 DrawFontAsset(
         SmartSpriteBatch spriteBatch,
         ScreenAbstractor screen,
         float opacity,
         string text,
         Color? fontColor,
         FloatRectangle? position,
         float fontSize,
         float? lineHeight,
         string fontName,
         bool pseudoAntiAlias,
         bool wordWrap,
         FontJustification justification,
         float fontMargin,
         Thickness margin = null,
         Color? outlineColor = null,
         FloatRectangle? clip = null,
         Texture2D bgTexture = null,
         Vector2? scrollOffset = null,
         int? caretPos = null,
         bool multiLine = false,
         Rectangle? scissorRect = null)
        {
            if (position == null)
            {
                return Vector2.Zero;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return Vector2.Zero;
            }

            List<string> textValues = new List<string> { text };

            if (multiLine)
            {
                textValues = text.ToList();
            }

            if (fontSize == 0)
            {
                fontSize = 24;
            }

            float lHeight = fontSize * 1.25f;

            if (lineHeight.HasValue)
            {
                lHeight = lineHeight.Value;
            }

            float translatedBaseSize = screen.Translate(new Vector2(1f, 1f)).Y;
            float fontSizeRatio = (translatedBaseSize / 1080f);
            float translatedFontSize = fontSizeRatio * fontSize;
            FontFamily fontFamily = Solids.Instance.Fonts.Fonts[fontName];

            if (fontFamily == null)
            {
                return Vector2.Zero;
            }

            string currentKey = translatedFontSize.ToString() + fontFamily.ToString();


            BMFont myFont = fontFamily.GetFont((int)translatedFontSize);

            Vector2 sizeOfI = myFont.MeasureString("Igj'#" + FontSystem.MDL2Symbols.Download.AsChar());

            float translatedWidth = screen.Translate(position).Value.Width;

            float wordWrapWidth = translatedWidth;

            if (margin != null)
            {
                wordWrapWidth = wordWrapWidth - screen.Translate(new Vector2(margin.Left + margin.Right, 0)).X;
                position = new FloatRectangle(position.Value.X, position.Value.Y + margin.Top, position.Value.Width, position.Value.Height);
            }

            Vector2 size = myFont.MeasureString(text);

            float scale = translatedFontSize / sizeOfI.Y;
            
            if (wordWrap && multiLine)
            {
                List<string> lines = textValues.ToList();

                textValues = new List<string>();

                string reconstructedLine = "";
                foreach (string line in lines)
                {
                    string[] words = line.Split(' ');
                    
                    for (int i = 0; i < words.Length; i++)
                    {
                        string withoutCurrentWord = reconstructedLine;
                        reconstructedLine = reconstructedLine + words[i] + " ";

                        Vector2 wwSize = myFont.MeasureString(reconstructedLine);
                        if (wwSize.X * scale > wordWrapWidth)
                        {
                            textValues.Add(withoutCurrentWord);
                            reconstructedLine = "";
                            i = i - 1;
                        }
                    }

                }

                textValues.Add(reconstructedLine);
            }



            float ulHeight = lHeight / translatedBaseSize;
            Vector2 calcSize = Vector2.Zero;
            for (int i = 0; i < textValues.Count; i++)
            {
                FloatRectangle positionValue = new FloatRectangle(position.Value.X, position.Value.Y + (ulHeight * i), position.Value.Width, ulHeight);
                FloatRectangle fontPos = new FloatRectangle(positionValue.X, positionValue.Y + fontMargin, positionValue.Width, positionValue.Height - (fontMargin * 2f));

                string textValue = textValues[i];

                FloatRectangle? tclip = screen.Translate(clip);

                FloatRectangle positionRectangle = new FloatRectangle();

                FloatRectangle translatedPosition = screen.Translate(fontPos).Value;

                if (tclip.HasValue)
                {
                    if (translatedPosition.Right < tclip.Value.X || translatedPosition.X > tclip.Value.Right)
                    {
                        return Vector2.Zero;
                    }
                }

                if (string.IsNullOrWhiteSpace(fontName))
                {
                    return Vector2.Zero;
                }

                float width = size.X * scale;

                switch (justification)
                {
                    case FontJustification.Left:
                        {
                            positionRectangle = new FloatRectangle(translatedPosition.X, translatedPosition.Y, width, translatedPosition.Height);
                            break;
                        }
                    case FontJustification.Right:
                        {
                            positionRectangle = new FloatRectangle(translatedPosition.Right - width, translatedPosition.Y, width, translatedPosition.Height);
                            break;
                        }
                    case FontJustification.Center:
                        {
                            positionRectangle = new FloatRectangle(translatedPosition.X + (translatedPosition.Width / 2f) - (width / 2f), translatedPosition.Y, width, translatedPosition.Height);
                            break;
                        }
                }

                if (caretPos != null)
                {
                    using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch, scissorRect: tclip))
                    {
                        float actualCarotPos = (width / text.Length) * caretPos.Value;

                        float oppax = (DateTime.Now.Millisecond % 250) / 250f;
                        spriteBatch.DrawLine(
                            new Vector2(positionRectangle.X + actualCarotPos, positionRectangle.Y),
                            new Vector2(positionRectangle.X + actualCarotPos, positionRectangle.Bottom),
                            Color.White * oppax, tclip, 1f);
                    }
                }

                Rectangle source = new Rectangle(0, 0, (int)size.X, (int)size.Y);

                tclip = tclip?.Clamp(scissorRect);

                (Rectangle position, Rectangle? source) translator = TextureHelpers.GetAdjustedDestAndSourceAfterClip(positionRectangle, source, tclip);

                Solids.Instance.SpriteBatch.Scissor = tclip.ToRectangle();
                
                Rectangle tmp = Solids.Instance.Bounds;
                if (tclip.HasValue)
                {
                    tmp = tclip.Clamp(scissorRect).ToRectangle;
                }

                Solids.Instance.SpriteBatch.Scissor = tmp;

                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                {
                    if ((pseudoAntiAlias || outlineColor.HasValue))
                    {
                        Color? cl = fontColor * opacity * 0.3f;

                        if (outlineColor.HasValue)
                        {
                            cl = outlineColor.Value * opacity;
                        }

                        for (int y = -1; y <= 1; y = y + 2)
                        {
                            for (int x = -1; x <= 1; x = x + 2)
                            {
                                myFont.DrawText(Solids.Instance.SpriteBatch, translator.position.X + x, translator.position.Y + y, textValue, cl, scale);
                            }
                        }
                    }

                    myFont.DrawText(Solids.Instance.SpriteBatch, translator.position.X, translator.position.Y, textValue, fontColor * opacity, scale);
                    calcSize = new Vector2(position.Value.Width, calcSize.Y + lHeight);
                }

                Solids.Instance.SpriteBatch.Scissor = null;

            }


            float actHeight = screen.Untranslate(calcSize.Y);

            if (margin != null)
            {
                calcSize.Y = calcSize.Y + margin.Bottom;
            }

            return new Vector2(position.Value.Width, actHeight);
        }
    }


}

