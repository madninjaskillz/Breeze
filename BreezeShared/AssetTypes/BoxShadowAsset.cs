using System;
using System.Xml;
using Breeze.Helpers;
using Breeze.Screens;
using Breeze.Services.InputService;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class BoxShadowAsset : DataboundContainterAsset
    {
        public BoxShadowAsset()
        {
        }

        public BoxShadowAsset(Color color, float borderSize, FloatRectangle position)
        {
            Color.Value = color;
            BorderSizePercentage.Value = borderSize;
            Position.Value = position;
        }

        internal void Update(BoxShadowAsset rectangleAsset)
        {
            Color = rectangleAsset.Color;
            BorderSizePercentage = rectangleAsset.BorderSizePercentage;
            Position = rectangleAsset.Position;
        }

        //public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        //{
        //    if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
        //    if (childNodeAttributes.GetNamedItem("BackgroundColor")?.Value != null) BackgroundColor = UIElement.GetDBValue<Color?>(childNodeAttributes.GetNamedItem("BackgroundColor")?.Value);
        //    if (childNodeAttributes.GetNamedItem("Color")?.Value != null) Color = UIElement.GetDBValue<Color>(childNodeAttributes.GetNamedItem("Color")?.Value);

        //    if (childNodeAttributes.GetNamedItem("TileMode")?.Value != null) TileMode = UIElement.GetDBValue<TileMode>(childNodeAttributes.GetNamedItem("TileMode")?.Value);
        //    if (childNodeAttributes.GetNamedItem("BorderSizePercentage")?.Value != null) BorderSizePercentage = UIElement.GetDBValue<float>(childNodeAttributes.GetNamedItem("BorderSizePercentage")?.Value);
        //    if (childNodeAttributes.GetNamedItem("Scale")?.Value != null) Scale = UIElement.GetDBValue<float>(childNodeAttributes.GetNamedItem("Scale")?.Value);
        //}

        //public DataboundValue<FloatRectangle> Position { get; set; } = new DataboundValue<FloatRectangle>();
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DataboundValue<TileMode> TileMode { get; set; } = new DataboundValue<TileMode>();

        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public DataboundValue<string> FillTexture { get; set; } = new DataboundValue<string>(null,null);

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DataboundValue<Color?> BackgroundColor { get; set; } = new DataboundValue<Color?>();

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DataboundValue<Color> Color { get; set; } = new DataboundValue<Color>();

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DataboundValue<float> BorderSizePercentage { get; set; } = new DataboundValue<float>();
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DataboundValue<float> Scale { get; set; } = new DataboundValue<float>(1f);
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            using (new SmartSpriteBatchManager(Solids.Instance.SpriteBatch))
            {
                Rectangle tmp = screen.Translate(Position.Value.Move(scrollOffset)).ToRectangle().Clip(screen.Translate(clip));

                int borderSize = (int) (Math.Min(tmp.Width, tmp.Height) * BorderSizePercentage.Value);
                
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\tl.png"), new Rectangle(tmp.X - borderSize, tmp.Y - borderSize, borderSize, borderSize), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\tr.png"), new Rectangle(tmp.Right, tmp.Y - borderSize, borderSize, borderSize), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\bl.png"), new Rectangle(tmp.X - borderSize, tmp.Bottom, borderSize, borderSize), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\br.png"), new Rectangle(tmp.Right, tmp.Bottom, borderSize, borderSize), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\t.png"), new Rectangle(tmp.X, tmp.Y - borderSize, tmp.Width, borderSize), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\b.png"), new Rectangle(tmp.X, tmp.Bottom, tmp.Width, borderSize), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\l.png"), new Rectangle(tmp.X - borderSize, tmp.Y, borderSize, tmp.Height), Color.Value * opacity);
                spriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture("Images\\9grid\\r.png"), new Rectangle(tmp.Right, tmp.Y, borderSize, tmp.Height), Color.Value * opacity);
            }

            SetChildrenOriginToMyOrigin();
        }
    }
}
