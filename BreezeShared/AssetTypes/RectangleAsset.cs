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
            BlurAmount.Value = blurAmout;

        }


        [Asset("Noise")]
        public DataboundValue<int> NoisePerc { get; set; } = new DataboundValue<int>();
        [Asset("Blur")]
        public DataboundValue<int> BlurAmount { get; set; } = new DataboundValue<int>(0);
        [Asset("TileMode")]
        public DataboundValue<TileMode> TilingMode { get; set; } = new DataboundValue<TileMode>();
        public DataboundValue<string> FillTexture { get; set; } =new DataboundValue<string>();
        public DataboundValue<Color?> BackgroundColor { get; set; } = new DataboundValue<Color?>();
        public DataboundValue<Color> BorderColor { get; set; }=new DataboundValue<Color>();
        public DataboundValue<int> BrushSize { get; set; }=new DataboundValue<int>();
        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(1);
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (clip.HasValue && ParentPosition.HasValue)
            {
                clip = clip.Value.ConstrainTo(ParentPosition.Value);
            }

            clip = screen.Translate(clip);

            var t = Position.Value();

            FloatRectangle tmp = screen.Translate(ActualPosition.AdjustForMargin(Margin)).Value;

            if (clip.HasValue)
            {
                
                if (tmp.Right < clip.Value.X || tmp.X > clip.Value.Right)
                {
                    return;
                }
            }

            if (BlurAmount.Value() > 0)
            {
                Solids.GaussianBlur.DoBlur(bgTexture, BlurAmount.Value(), (BackgroundColor.Value().Value * opacity) * ((BlurAmount.Value() / Solids.MaxBlur)), tmp.ToRectangle, ScissorRect, clip, NoisePerc.Value());
            }

            if (BackgroundColor.Value().HasValue && FillTexture.Value() == null)
            {
                spriteBatch.DrawSolidRectangle(tmp, BackgroundColor.Value().Value * opacity, clip);
            }

            if (FillTexture.Value() != null)
            {
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                {
                    spriteBatch.DrawTexturedRectangle(tmp, BackgroundColor.Value().Value * opacity, Solids.Instance.AssetLibrary.GetTexture(FillTexture.Value()), TilingMode.Value, clip);
                }
            }

            if (BrushSize.Value() > 0)
            {

                spriteBatch.DrawBorder(tmp, BorderColor.Value() * opacity, BrushSize.Value(), clip);
            }

            this.ActualSize = new Vector2(Position.Value().Width, Position.Value().Height).PadForMargin(Margin);

            SetChildrenOriginToMyOrigin();
        }
    }
}
