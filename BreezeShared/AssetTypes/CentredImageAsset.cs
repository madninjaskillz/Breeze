using Breeze.Screens;
using Breeze.Helpers;
using Breeze.Services.InputService;
using Breeze.AssetTypes.DataBoundTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class CentredImageAsset : DataboundAsset
    {
        private Texture2D texture;
        private float aspectRatio;
        public ScaleMode ScaleMode { get; set; }
        public CentredImageAsset() { }
        
        public CentredImageAsset(string txture, FloatRectangle position, ScaleMode scalemode = ScaleMode.Fill, float rotation = 0f)
        {
            texture = Solids.Instance.AssetLibrary.GetTexture(txture, true);
            //texture = txture;
            Position.Value = position;
            ScaleMode = scalemode;
            aspectRatio = texture.Width / (float)texture.Height;
            Rotation.Value = rotation;
        }

        public DataboundValue<float> Rotation { get; set; } = new DataboundValue<float>();

        public DataboundValue<string> Texture { get; set; } = new DataboundValue<string>();


        //public DataboundValue<Texture2D> Texture2D { get; set; } = new DataboundValue<Texture2D>();

        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(1);
        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (Solids.Instance.InputService.IsPressed(InputService.ShiftKeys.LeftShift1))
            {
            //    return;

            }

            var rect = screen.Translate(Position.Value).ToRectangle(); //.Move(scrollOffset)

            if (ScaleMode == ScaleMode.BestFit)
            {
                float newWidth = rect.Width;
                float newHeight = newWidth / aspectRatio;

                if (newHeight > rect.Height)
                {
                    newHeight = rect.Height;
                    newWidth = newHeight * aspectRatio;
                }

                float xOffset = (rect.Width - newWidth) / 2f;
                float yOffset = (rect.Height - newHeight) / 2f;

                rect = new Rectangle((int)(rect.X + xOffset), (int)(rect.Y + yOffset), (int)newWidth, (int)newHeight);
            }

            if (ScaleMode == ScaleMode.None)
            {
                rect.Width = texture.Width;
                rect.Height = texture.Height;
            }

            using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
            {
                if (clip == null)
                {
                    rect = new Rectangle(rect.X + (int)(rect.Width / 2f), rect.Y + (int)(rect.Height / 2f), rect.Width,rect.Height);
                    spriteBatch.Draw(texture, rect, null, Color.White * opacity, Rotation.Value, new Vector2(texture.Width/2, texture.Height/2),SpriteEffects.None,1f);
                }
                else
                {
                    FloatRectangle? tclip = screen.Translate(clip);
                    Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
                    (Rectangle position, Rectangle? source) translator = TextureHelpers.GetAdjustedDestAndSourceAfterClip(new FloatRectangle(rect), source, tclip);
                    using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                    {
                        spriteBatch.Draw(texture, translator.position, translator.source, Color.White * opacity, Rotation.Value, new Vector2(translator.source.Value.Width/2f,translator.source.Value.Height/2f), SpriteEffects.None, 1f );
                    }
                }
            }
        }
    }
}
