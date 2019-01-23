using System;
using System.Collections.Generic;
using System.Text;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Breeze.AssetTypes
{
    public class GroupAsset : DataboundContainterAsset
    {
        public DataboundValue<string> Template { get; set; } = new DataboundValue<string>(); //todo constrain to DataboundTemplate

        public DataboundValue<float> ItemWidth { get; set; } = new DataboundValue<float>(0.1f);
        public DataboundValue<float> ItemHeight { get; set; } = new DataboundValue<float>(0.25f);

        public DataboundValue<IEnumerable<VirtualizedDataContext>> Items { get; set; } = new DataboundValue<IEnumerable<VirtualizedDataContext>>();
        private string previousHash = "";

        private int ctx = 0;
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            ctx = 0;
            var items = Items?.Value();
            string hash = "";

            hash = items?.GetHashCode().ToString();

            if (previousHash != hash)
            {

                this.Children.Value = new List<DataboundAsset>();
                if (items != null)
                {
                    foreach (VirtualizedDataContext dataContext in items)
                    {
                        var newItem = screenResources.GetTemplate(Template.Value());
                        newItem.VirtualizedDataContext = dataContext;
                        newItem.ParentAsset = this;

                        this.Children.Value.Add(newItem);
                    }
                }

                this.FixParentChildRelationship();
                this.FixBinds();
                previousHash = hash;
            }


            SetChildrenOriginToMyOrigin();

        }
    }
}
