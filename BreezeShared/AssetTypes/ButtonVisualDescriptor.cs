using System.Xml.Serialization;
using Breeze.AssetTypes;
using Breeze.FontSystem;
using Microsoft.Xna.Framework;

namespace Breeze.Screens
{
    public class ButtonVisualDescriptor
    {
        public string Text { get; set; }
        public FontAsset.FontJustification TextJustification { get; set; }
        public float FontScale { get; set; }
        public Color FontColor { get; set; }
        public Color BorderColor { get; set; }
        public int BorderBrushSize { get; set; }
        public Color BackgroundColor { get; set; }
        public string BackgroundTexture { get; set; }
        public TileMode BackgroundTileMode { get; set; }

        public float ShadowDepth { get; set; } = 0.07f;
        [XmlIgnore]
        public FontFamily FontFamily { get; set; }

        private string font;
        public string Font
        {
            get { return font; }
            set
            {
                font = value;
                FontFamily = Solids.Instance.Fonts.Fonts[value];
            }
        }

        public int BlurAmount { get; set; }
    }
}