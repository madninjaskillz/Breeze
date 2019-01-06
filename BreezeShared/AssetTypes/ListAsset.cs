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
    public class ListAsset : DataboundContainterAsset
    {
        public DataboundValue<string> Template { get; set; } = new DataboundValue<string>(); //todo constrain to DataboundTemplate

        public DataboundValue<float> ItemWidth { get; set; } = new DataboundValue<float>(0.1f);
        public DataboundValue<float> ItemHeight { get; set; } = new DataboundValue<float>(0.25f);

        public DataboundValue<IEnumerable<VirtualizedDataContext>> Items { get; set; } = new DataboundValue<IEnumerable<VirtualizedDataContext>>();
        private string previousHash = "";
        
        private int ctx = 0;
        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            ctx++;
            if (ctx > 60)
            {
                ctx = 0;
                var items = Items?.Value();
                string hash = items.GetHashCode().ToString();
                if (previousHash != hash)
                {
                    previousHash = hash;
                    this.Children.Value = new List<DataboundAsset>();
                    foreach (VirtualizedDataContext dataContext in items)
                    {
                        var newItem = screenResources.GetTemplate(Template.Value());
                        newItem.VirtualizedDataContext = dataContext;
                        newItem.ParentAsset = this;

                        this.Children.Value.Add(newItem);
                    }

                    this.FixParentChildRelationship();
                    this.FixBinds();
                }
            }

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

                float lm = 0;
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
