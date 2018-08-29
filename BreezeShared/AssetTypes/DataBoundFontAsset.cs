using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Breeze.FontSystem;
using Breeze.Helpers;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class FontAsset : DataboundAsset
    {
        private string renderedKey;

        private float fetchedHeight = 6;

        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            Color dbg1 = Color.White;
            Color dbg2 = Color.White*0.5f;
            if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
            if (childNodeAttributes.GetNamedItem("Margin")?.Value != null) Margin = UIElement.GetDBValue<Thickness>(childNodeAttributes.GetNamedItem("Margin")?.Value);
            if (childNodeAttributes.GetNamedItem("FontMargin")?.Value != null) FontMargin = UIElement.GetDBValue<float>(childNodeAttributes.GetNamedItem("FontMargin")?.Value);
            if (childNodeAttributes.GetNamedItem("FontColor")?.Value != null) FontColor = UIElement.GetDBValue<Color>(childNodeAttributes.GetNamedItem("FontColor")?.Value);
            if (childNodeAttributes.GetNamedItem("OutlineColor")?.Value != null) OutlineColor = UIElement.GetDBValue<Color?>(childNodeAttributes.GetNamedItem("OutlineColor")?.Value);

            if (childNodeAttributes.GetNamedItem("Scale")?.Value != null) Scale = UIElement.GetDBValue<float>(childNodeAttributes.GetNamedItem("Scale")?.Value);
            if (childNodeAttributes.GetNamedItem("FontFamily")?.Value != null) FontName = UIElement.GetDBValue<string>(childNodeAttributes.GetNamedItem("FontFamily")?.Value);
            if (childNodeAttributes.GetNamedItem("Text")?.Value != null) Text = UIElement.GetDBValue<string>(childNodeAttributes.GetNamedItem("Text")?.Value);

            if (childNodeAttributes.GetNamedItem("CaretPos")?.Value != null) CaretPos = UIElement.GetDBValue<int?>(childNodeAttributes.GetNamedItem("CaretPos")?.Value);
            if (childNodeAttributes.GetNamedItem("AntiAlias")?.Value != null) PseudoAntiAlias = UIElement.GetDBValue<bool>(childNodeAttributes.GetNamedItem("AntiAlias")?.Value);
            if (childNodeAttributes.GetNamedItem("MultiLine")?.Value != null) MultiLine = UIElement.GetDBValue<bool>(childNodeAttributes.GetNamedItem("MultiLine")?.Value);
        }

        [XmlIgnore]
        public float Width { get; private set; }
        public DataboundValue<FontJustification> Justification { get; set; } = new DataboundValue<FontJustification>();
        public DataboundValue<string> Text { get; set; } = new DataboundValue<string>(string.Empty, null);

        //public DataboundValue<FloatRectangle?> Position { get; set; } = new DataboundValue<FloatRectangle?>(null);
        public DataboundValue<Color> FontColor { get; set; } = new DataboundValue<Color>(null);
        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(0.5f,null);
        public DataboundValue<string> FontName { get; set; } = new DataboundValue<string>(null);

        public DataboundValue<int?> CaretPos { get; set; } = new DataboundValue<int?>();
        public DataboundValue<bool> PseudoAntiAlias { get; set; } = new DataboundValue<bool>();
        public DataboundValue<bool>MultiLine { get; set; } = new DataboundValue<bool>();
        public DataboundValue<Color?> OutlineColor { get; set; } = new DataboundValue<Color?>();
        public DataboundValue<float> FontMargin { get; set; } = new DataboundValue<float>();

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



        public FontAsset(string text, Color color, FloatRectangle position, FontFamily fontFamily, FontJustification justification = FontJustification.Left, int? caretPos = null, bool? multiLine= false)
        {
            Text.Value = text;
            FontColor.Value = color;
            Position.Value = position;

            if (fontFamily!=null)
                FontName.Value = Solids.Instance.Fonts.Fonts.First(t => t.Value.FontName == fontFamily.FontName).Key;
            //FontFamily = fontFamily;

            Justification.Value = justification;

            CaretPos.Value = caretPos;
            if (multiLine.HasValue && multiLine.Value)
            {
                MultiLine.Value = true;
            }

            Text.SetChangeAction(()=>fontKey="x");
        }
        
        private float scale = 1f;
        private BMFont myFont = null;
        private string fontKey = "x";
        private Vector2 size;

        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (FontName.Value != null)
            {
                this.ActualSize = DrawFontAsset(spriteBatch, screen, opacity, Text.Value, FontColor.Value,
                    ActualPosition, FontName.Value, PseudoAntiAlias.Value,
                    Justification.Value, FontMargin.Value, OutlineColor.Value, Clip, bgTexture, scrollOffset, CaretPos.Value,
                    MultiLine.Value, ScissorRect);
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
            string fontName,  
            bool pseudoAntiAlias,
            FontJustification justification,
            float fontMargin,
            Color? outlineColor = null,
            FloatRectangle? clip = null, 
            Texture2D bgTexture = null, 
            Vector2? scrollOffset = null, 
            int? caretPos = null, 
            bool multiLine = false, 
            Rectangle? scissorRect=null)
        {
            
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
                    //todo FIX ME
                    //textValues = text.ToList();
                }

                float lineHeight = position.Value.Height;

                for (int i = 0; i < textValues.Count; i++)
                {
                    FloatRectangle positionValue = new FloatRectangle(position.Value.X, position.Value.Y + (lineHeight * i), position.Value.Width, lineHeight);
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

                    FontFamily fontFamily = Solids.Instance.Fonts.Fonts[fontName];

                    if (fontFamily == null)
                    {
                        return Vector2.Zero;
                    }

                    string currentKey = translatedPosition.Height.ToString() + fontFamily.ToString();

          
                        var myFont = fontFamily.GetFont((int)translatedPosition.Height);

                        Vector2 sizeOfI = myFont.MeasureString("Igj'#" + FontSystem.MDL2Symbols.Download.AsChar());
                        var size = myFont.MeasureString(text);

                        var scale = translatedPosition.Height / sizeOfI.Y;
                        var fontKey = currentKey;
          

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


                    //FloatRectangle fontPos = new FloatRectangle(positionRectangle.X , positionRectangle.Y + fontMargin, positionRectangle.Width, positionRectangle.Height - (fontMargin * 2f));

                    (Rectangle position, Rectangle? source) translator = TextureHelpers.GetAdjustedDestAndSourceAfterClip(positionRectangle, source, tclip);



                    Solids.Instance.SpriteBatch.Scissor = tclip.ToRectangle();


                    Rectangle tmp = Solids.Bounds;
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
                    }

                    Solids.Instance.SpriteBatch.Scissor = null;

                  var  Width = translator.position.Width;
                }

                return new Vector2(position.Value.Width,position.Value.Height* textValues.Count);
            }
        }
    }
}

