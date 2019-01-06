using System.Xml.Serialization;
using Breeze.Screens;
using Breeze.Helpers;
using Breeze.Services.InputService;
using Breeze.AssetTypes.DataBoundTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class ImageAsset : DataboundAsset
    {
        public ImageAsset()
        {
        }

        public ImageAsset(Texture2D txture, FloatRectangle position, Color? color = null, float rotation = 0)
        {
            Texture2D.Value = txture;
            Position.Value = position;
            this.ImageColor.Value = color ?? Color.White;
            Rotation.Value = rotation;
        }

        public ImageAsset(Texture2D txture, FloatRectangle position, Rectangle sourceRectangle, Color? color = null, float rotation = 0)
        {
            Texture2D.Value = txture;
            Position.Value = position;
            SourceRectangle.Value = sourceRectangle;
            this.ImageColor.Value = color ?? Color.White;
            Rotation.Value = rotation;
        }

        public DataboundValue<Color> ImageColor { get; set; } = new DataboundValue<Color>();
        public DataboundValue<FloatRectangle> Position { get; set; } = new DataboundValue<FloatRectangle>();

        [XmlIgnore]
        public DataboundValue<Texture2D> Texture2D { get; set; } = new DataboundValue<Texture2D>();

        public DataboundValue<string> Texture { get; set; } = new DataboundValue<string>();

        public DataboundValue<Rectangle?> SourceRectangle { get; set; } = new DataboundValue<Rectangle?>(null,null);
        public DataboundValue<float> Rotation { get; set; } = new DataboundValue<float>(0);
        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(0);
   
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (Solids.Instance.InputService.IsPressed(InputService.ShiftKeys.LeftShift1))
            {
             //   return;

            }

            if (Texture2D != null)
            {

                FloatRectangle? pos = screen.Translate(Position.Value.Move(scrollOffset));
                FloatRectangle? translatedClip = screen.Translate(clip);

                Rectangle? fixedSource = SourceRectangle.Value;

                (Rectangle position, Rectangle? source) thing = TextureHelpers.GetAdjustedDestAndSourceAfterClip(pos, fixedSource, translatedClip);
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                {
                    //spriteBatch.Draw(Texture2D.Value, thing.Item1, thing.Item2, ImageColor.Value * opacity, Rotation.Value, new Vector2(thing.Item1.Width/2f, thing.Item1.Height/2f),SpriteEffects.None,1f);

                    spriteBatch.Draw(Texture2D.Value, pos.Value.ToRectangle, null, ImageColor.Value * opacity);
                }

            }
        }

        //internal void Update(ImageAsset rectangleAsset)
        //{
        //    SourceRectangle = rectangleAsset.SourceRectangle;
        //    Position = rectangleAsset.Position;
        //    Texture2D = rectangleAsset.Texture2D;
        //    ImageColor = rectangleAsset.ImageColor;
        //    Rotation = rectangleAsset.Rotation;
        //}
    }

    public enum TileMode
    {
        JustStretch,
        Tile,
        StretchToFill
    }


    public enum ScaleMode
    {
        Fill,
        BestFit,
        FillToScale,
        None
    }
}
