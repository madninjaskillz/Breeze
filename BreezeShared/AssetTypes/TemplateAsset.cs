using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Breeze.AssetTypes;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.Helpers;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.Shared.AssetTypes
{
    public class TemplateAsset : DataboundContainterAsset
    {
        public DataboundValue<string> Template { get; set; } = new DataboundValue<string>(string.Empty, null);

        public TemplateAsset()
        {
        }

        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            string template = Template.Value;
            SetChildrenOriginToMyOrigin();
        }

        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
            if (childNodeAttributes.GetNamedItem("Template")?.Value != null) Template = UIElement.GetDBValue<string>(childNodeAttributes.GetNamedItem("Template")?.Value);
        }

        public void ProcessContents()
        {
            //todo fix me

            //List<DataboundAsset> childs = this.Children.Value.DeepClone();
            //    foreach (DataboundAsset databoundAsset in childs)
            //    {
            //        databoundAsset.ParentPosition = this.ActualPosition;
            //    }
            //    List<ContentAsset> content = UIHelpers.FindChildrenOfType<ContentAsset>(templateContainterAsset);



            //    foreach (var cont in content)
            //    {
            //        cont.Children.Value = childs;
            //        cont.ParentPosition = templateAsset.ActualPosition;
            //    }

              //  templateContainterAsset.Children = new DataboundAsset.DataboundValue<List<DataboundAsset>>();
        }
    }

    public class ContentAsset : DataboundContainterAsset
    {
        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
           // SetChildrenOriginToMyOrigin();
        }

        public override void LoadFromXml(XmlAttributeCollection childNodeAttributes)
        {
            //if (childNodeAttributes.GetNamedItem("Position")?.Value != null) Position = UIElement.GetDBValue<FloatRectangle>(childNodeAttributes.GetNamedItem("Position")?.Value);
            //if (childNodeAttributes.GetNamedItem("Template")?.Value != null) Template = UIElement.GetDBValue<string>(childNodeAttributes.GetNamedItem("Template")?.Value);
        }
    }
}
