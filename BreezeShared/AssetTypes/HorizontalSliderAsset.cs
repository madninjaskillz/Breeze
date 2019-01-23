using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class HorizontalSliderAsset : InteractiveAsset
    {
        public HorizontalSliderAsset()
        {
            this.InternalClickEvent = ClickEvent;
        }

        private void ClickEvent(ButtonClickEventArgs obj)
        {
            Debug.WriteLine("Horizontal Slider Click:" + obj.ClickPosition + ", " + obj.ClickSource);

            this.Value.Value = MaxValue.Value() * obj.ClickPosition.X;
        }

        public DataboundValue<float> BarWidth { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> SliderHeight { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> Value { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> MaxValue { get; set; } = new DataboundValue<float>();

        public DataboundValue<Color?> SliderColor { get; set; } = new DataboundValue<Color?>();
        public DataboundValue<Color?> BarColor { get; set; } = new DataboundValue<Color?>();

        public DataboundValue<string> SliderTexture { get; set; } = new DataboundValue<string>();
        public DataboundValue<string> BarTexture { get; set; } = new DataboundValue<string>();

        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            float width = ActualPosition.Width;
            float height = ActualPosition.Height;
            float barWidth = BarWidth.Value();
            if (barWidth == 0)
            {
                barWidth = 0.001f;
            }


            float availableWidth = width - barWidth;

            float percentage = Value.Value() / MaxValue.Value();

            float sliderHeight = SliderHeight.Value();
            if (sliderHeight == 0)
            {
                sliderHeight = 0.001f;
            }

            float sliderTop = (height / 2f) - (sliderHeight / 2f);

            FloatRectangle sliderRect = new FloatRectangle(0 + ActualPosition.X, sliderTop + ActualPosition.Y, width, sliderHeight);
            FloatRectangle barRect = new FloatRectangle(availableWidth * percentage + ActualPosition.X, ActualPosition.Y, barWidth, height);

            if (SliderTexture.HasValue())
            {
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                {
                    spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture(SliderTexture.Value(), true), screen.Translate(sliderRect).Value.ToRectangle, Color.White);
                }
            }
            else
            {
                spriteBatch.DrawSolidRectangle(screen.Translate(sliderRect).Value, SliderColor.Value().Value * opacity, clip);
            }

            if (BarTexture.HasValue())
            {
                using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
                {
                    spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture(BarTexture.Value(), true), screen.Translate(barRect).Value.ToRectangle, Color.White);
                }
            }
            else
            {
                spriteBatch.DrawSolidRectangle(screen.Translate(barRect).Value, BarColor.Value().Value * opacity, clip);
            }

        }
    }
}
