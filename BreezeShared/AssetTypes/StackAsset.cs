using System.Collections.Generic;
using System.Xml;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class StackAsset : DataboundContainterAsset
    {
        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
            if (childNodeAttributes.GetNamedItem("Margin")?.Value != null) Margin = UIElement.GetDBValue<Thickness>(childNodeAttributes.GetNamedItem("Margin")?.Value);

        }
        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            float pos = 0;

            foreach (DataboundAsset item in Children.Value)
            {
                if (item.Margin != null && item.Margin.Value != null)
                {
                    pos = pos + item.Margin.Value.Top;
                }


                float height = item.Position.Value.Height;
                if (item.ActualSize.Y > 0)
                {
                    height = item.ActualSize.Y;
                }

                float lm=0;
                float bm = 0;

                if (item.Margin != null && item.Margin.Value != null)
                {
                    lm = item.Margin.Value.Left;
                    bm = item.Margin.Value.Bottom;
                }
                
                item.Position.Value = new FloatRectangle(lm, pos, item.Position.Value.Width, item.Position.Value.Height);

                pos = pos + height + bm;
            }

            this.ActualSize = new Vector2(this.Position.Value.Width, pos - this.Position.ToVector2().Y);

            SetChildrenOriginToMyOrigin();
        }

    }
}