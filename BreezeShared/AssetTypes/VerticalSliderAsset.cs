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
    public class VerticalSliderAsset : InteractiveAsset
    {
        public VerticalSliderAsset()
        {
            this.InternalPressEvent = ClickEvent;
            this.InternalStickUpEvent = StickUpEvent;
            this.InternalStickDownEvent = StickDownEvent;
        }

        private void ClickEvent(ButtonClickEventArgs obj)
        {
            Debug.WriteLine("Vertical Slider Click:" + obj.ClickPosition + ", " + obj.ClickSource);

            this.Value.Value = MaxValue.Value() * (1f-obj.ClickPosition.Y);
        }

        private void StickUpEvent(ButtonClickEventArgs obj)
        {
            float stepSize = (this.MaxValue.Value() / 16f);
            if (this.StepSize.HasValue())
            {
                stepSize = this.StepSize.Value();
            }

            float tmp = this.Value.Value() + stepSize;
            if (tmp > this.MaxValue.Value())
            {
                tmp = this.MaxValue.Value();
            }

            this.Value.SetDVValue(tmp);
        }

        private void StickDownEvent(ButtonClickEventArgs obj)
        {
            float stepSize = (this.MaxValue.Value() / 16f);
            if (this.StepSize.HasValue())
            {
                stepSize = this.StepSize.Value();
            }

            float tmp = this.Value.Value() - stepSize;
            if (tmp < 0)
            {
                tmp = 0;
            }

            this.Value.SetDVValue(tmp);
        }

        public DataboundValue<float> BarHeight { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> SliderWidth { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> Value { get; set; } = new DataboundValue<float>();

        public DataboundValue<float> StepSize { get; set; } = new DataboundValue<float>();
        public DataboundValue<float> MaxValue { get; set; } = new DataboundValue<float>();

        public DataboundValue<Color?> SliderColor { get; set; } = new DataboundValue<Color?>();
        public DataboundValue<Color?> BarColor { get; set; } = new DataboundValue<Color?>();

        public DataboundValue<string> SliderTexture { get; set; } = new DataboundValue<string>();
        public DataboundValue<string> BarTexture { get; set; } = new DataboundValue<string>();
        public DataboundValue<Color?> ActiveColor { get; set; } = new DataboundValue<Color?>();
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            float width = ActualPosition.Width;
            float height = ActualPosition.Height;
            float barHeight = BarHeight.Value();
            if (barHeight == 0)
            {
                barHeight = 0.001f;
            }


            float availableHeight = height - barHeight;

            float percentage = Value.Value() / MaxValue.Value();

            float sliderWidth = SliderWidth.Value();
            if (sliderWidth == 0)
            {
                sliderWidth = 0.001f;
            }

            float sliderLeft = (width/ 2f) - (sliderWidth / 2f);

            FloatRectangle sliderRect = new FloatRectangle( 
                sliderLeft + ActualPosition.X, 
                ActualPosition.Y,
                sliderWidth,
                height);

            FloatRectangle barRect = new FloatRectangle(
                ActualPosition.X, 
                (availableHeight-(availableHeight * percentage)) + ActualPosition.Y,
                width, 
                barHeight);

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

            if (ActiveColor.HasValue() && this.State.Value == ButtonState.Hover)
            {
                spriteBatch.DrawSolidRectangle(screen.Translate(ActualPosition).Value, ActiveColor.Value().Value * opacity, clip);
            }

        }
    }
}
