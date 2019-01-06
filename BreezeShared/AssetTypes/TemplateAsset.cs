using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Breeze.AssetTypes;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.AssetTypes.XMLClass;
using Breeze.Helpers;
using Breeze.Screens;
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

        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            if (!haveLoadedTemplate)
            {
                string template = Template.Value();

                List<DataboundAsset> children = this.Children.Value;

                DataboundAsset templateContent = screenResources.GetTemplate(template);
                templateContent.ParentAsset = this;

                if (templateContent is DataboundContainterAsset containerAsset)
                {
                    //containerAsset.Children.Value = children;

                    List<DataboundAsset> contentAssets = containerAsset.FindChildrenOfType<ContentAsset>();
                    if (contentAssets.Count>0)
                    {
                        foreach (var contentAsset in contentAssets)
                        {
                            ((DataboundContainterAsset)contentAsset.ParentAsset).Children.Value = children;
                        }
                    }
                }
                
                this.Children.Value = new List<DataboundAsset>{templateContent};
                this.FixParentChildRelationship();

                this.FixBinds();
                haveLoadedTemplate = true;
            }

            SetChildrenOriginToMyOrigin();
            if (ActualSize == Vector2.Zero)
            {
                this.ActualSize = this.Children.Value.First().ActualSize;

            }
        }

        private bool haveLoadedTemplate;

    }

    public class ContentAsset : DataboundContainterAsset
    {
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            SetChildrenOriginToMyOrigin();
            this.ActualSize = this.Children.Value.First().ActualSize;
        }
    }
}
