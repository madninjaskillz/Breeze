﻿using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.FontSystem;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class ButtonAsset : InteractiveAsset
    {
        public ButtonAsset() { }

        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (this.Children.Value == null || this.Children.Value.Count == 0)
            {
                return;
            }

            foreach (DataboundAsset databoundAsset in this.Children.Value)
            {
                databoundAsset.IsHidden.Value = true;
            }

            DataboundAsset select = this.Children.Value.First();

            if (this.State.Value == ButtonState.Hover && this.Children.Value.Count > 1) select = this.Children.Value[1];
            if (this.State.Value == ButtonState.Pressing && this.Children.Value.Count > 1) select = this.Children.Value[2];

            select.IsHidden.Value = false;

            SetChildrenOriginToMyOrigin();
        }

        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
        }
    }
}
