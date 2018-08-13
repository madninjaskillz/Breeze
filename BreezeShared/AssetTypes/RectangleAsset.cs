using System.Xml;
using System.Xml.Serialization;
using Breeze.Helpers;
using Breeze.Screens;
using Breeze.Services.InputService;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class UIRectangle : UIElement
    {
        [System.Xml.Serialization.XmlAttribute()]
        public string Color { get; set; }

        [System.Xml.Serialization.XmlAttribute("BorderColor")]
        public string BorderColor { get; set; }

        [System.Xml.Serialization.XmlAttribute("BackgroundColor")]
        public string BackgroundColor { get; set; }

        [System.Xml.Serialization.XmlAttribute("NoisePerc")]
        public string NoisePerc { get; set; }

        [System.Xml.Serialization.XmlAttribute("BlurAmount")]
        public string BlurAmount { get; set; }

        [System.Xml.Serialization.XmlAttribute("TileMode")]
        public string TileMode { get; set; }

        [System.Xml.Serialization.XmlAttribute("FillTexture")]
        public string FillTexture { get; set; }

        [System.Xml.Serialization.XmlAttribute("BrushSize")]
        public string BrushSize { get; set; }

        [System.Xml.Serialization.XmlAttribute("Scale")]
        public string Scale { get; set; }

        public override DataboundContainterAsset ToSystem()
        {
            var result = new RectangleAsset()
            {
                Position = GetDBValue<FloatRectangle>(Position),
                Margin = GetDBValue<Thickness>(Margin),
                BorderColor = GetDBValue<Color>(BorderColor),
                BackgroundColor = GetDBValue<Color?>(BackgroundColor),
                NoisePerc = GetDBValue<int>(NoisePerc),
                BlurAmount = GetDBValue<int>(BlurAmount),
                FillTexture = GetDBValue<string>(FillTexture),
                BrushSize = GetDBValue<int>(BrushSize),
                Scale = GetDBValue<float>(Scale)
            };

            return result;
        }

    }
    public class RectangleAsset : DataboundContainterAsset
    {
        public RectangleAsset() { }
        public RectangleAsset(Color color, int brushSize, FloatRectangle position, Color? backgroundColor = null, int blurAmout = 0, int noisePerc = 7)
        {
            NoisePerc.Value = noisePerc;
            BorderColor.Value = color;
            BrushSize.Value = brushSize;
            Position.Value = position;
            BackgroundColor.Value = backgroundColor;
            BlurAmount.Value = blurAmout;
        }

        public RectangleAsset(Color color, int brushSize, FloatRectangle position, string fillTexture, Color? backgroundColor = null, TileMode tileMode = TileMode.JustStretch, int blurAmout = 0, int noisePerc = 7)
        {
            NoisePerc.Value = noisePerc;
            BorderColor.Value = color;
            BrushSize.Value = brushSize;
            Position.Value = position;
            FillTexture.Value = fillTexture;
            BackgroundColor.Value = backgroundColor ?? Color.White;
            TilingMode.Value = tileMode;
         //   FillTexture2D.Value = Solids.AssetLibrary.GetTexture(FillTexture.Value);
            BlurAmount.Value = blurAmout;

        }


        public RectangleAsset(Color color, int brushSize, FloatRectangle position, Texture2D fillTexture, Color? backgroundColor = null, TileMode tileMode = TileMode.JustStretch, int blurAmout = 0, int noisePerc = 7)
        {
            NoisePerc.Value = noisePerc;
            BorderColor.Value = color;
            BrushSize.Value = brushSize;
            Position.Value = position;
            //FillTexture.Value = "from texture";
            BackgroundColor.Value = backgroundColor ?? Color.White;
            TilingMode.Value = tileMode;
          //  FillTexture2D.Value = fillTexture;
            BlurAmount.Value = blurAmout;
        }

        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
            if (childNodeAttributes.GetNamedItem("BackgroundColor")?.Value != null) BackgroundColor = UIElement.GetDBValue<Color?>(childNodeAttributes.GetNamedItem("BackgroundColor")?.Value);

            if (childNodeAttributes.GetNamedItem("BorderColor")?.Value != null)
                BorderColor = UIElement.GetDBValue<Color>(childNodeAttributes.GetNamedItem("BorderColor")?.Value);

            if (childNodeAttributes.GetNamedItem("Noise")?.Value != null)
                NoisePerc = UIElement.GetDBValue<int>(childNodeAttributes.GetNamedItem("Noise")?.Value);

            if (childNodeAttributes.GetNamedItem("Blur")?.Value != null)
                BlurAmount = UIElement.GetDBValue<int>(childNodeAttributes.GetNamedItem("Blur")?.Value);

            if (childNodeAttributes.GetNamedItem("TileMode")?.Value != null)
                TilingMode = UIElement.GetDBValue<TileMode>(childNodeAttributes.GetNamedItem("TileMode")?.Value);

            if (childNodeAttributes.GetNamedItem("FillTexture")?.Value != null)
                FillTexture = UIElement.GetDBValue<string>(childNodeAttributes.GetNamedItem("FillTexture")?.Value);

            if (childNodeAttributes.GetNamedItem("BrushSize")?.Value != null)
                BrushSize = UIElement.GetDBValue<int>(childNodeAttributes.GetNamedItem("BrushSize")?.Value);

            if (childNodeAttributes.GetNamedItem("Scale")?.Value != null)
                Scale = UIElement.GetDBValue<float>(childNodeAttributes.GetNamedItem("Scale")?.Value);


        }

        //public DataboundValue<FloatRectangle> Position { get; set; } = new DataboundValue<FloatRectangle>();
        public DataboundValue<int> NoisePerc { get; set; } = new DataboundValue<int>();
        public DataboundValue<int> BlurAmount { get; set; } = new DataboundValue<int>(0);
        public DataboundValue<TileMode> TilingMode { get; set; } = new DataboundValue<TileMode>();
        public DataboundValue<string> FillTexture { get; set; } =new DataboundValue<string>();
        public DataboundValue<Color?> BackgroundColor { get; set; } = new DataboundValue<Color?>();
        public DataboundValue<Color> BorderColor { get; set; }=new DataboundValue<Color>();
        public DataboundValue<int> BrushSize { get; set; }=new DataboundValue<int>();
        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(1);
        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (clip.HasValue && ParentPosition.HasValue)
            {
                clip = clip.Value.ConstrainTo(ParentPosition.Value);
            }

            clip = screen.Translate(clip);


            FloatRectangle tmp = screen.Translate(ActualPosition).Value;
            
            if (clip.HasValue)
            {
                
                if (tmp.Right < clip.Value.X || tmp.X > clip.Value.Right)
                {
                    return;
                }
            }

            if (BlurAmount.Value > 0)
            {
                Solids.GaussianBlur.DoBlur(bgTexture, BlurAmount.Value, (BackgroundColor.Value.Value * opacity) * ((BlurAmount.Value / Solids.MaxBlur)), tmp.ToRectangle, ScissorRect, clip, NoisePerc.Value);
            }

            if (BackgroundColor.Value.HasValue && FillTexture.Value == null)
            {
                spriteBatch.DrawSolidRectangle(tmp, BackgroundColor.Value.Value * opacity, clip);
            }

            if (FillTexture.Value != null)
            {
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                {
                    spriteBatch.DrawTexturedRectangle(tmp, BackgroundColor.Value.Value * opacity, Solids.Instance.AssetLibrary.GetTexture(FillTexture.Value), TilingMode.Value, clip);
                }
            }

            if (BrushSize.Value > 0)
            {

                spriteBatch.DrawBorder(tmp, BorderColor.Value * opacity, BrushSize.Value, clip);
            }

            this.ActualSize = new Vector2(Position.Value.Width, Position.Value.Height);

            SetChildrenOriginToMyOrigin();
        }
    }
}
