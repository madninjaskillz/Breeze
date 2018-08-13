//TODO FIX ME

//using System;
//using System.Collections.Generic;
//using System.Text;
//using Breeze.AssetTypes.DataBoundTypes;
//using Breeze.Helpers;
//using Breeze.Screens;
//using Breeze.Templates;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Breeze.AssetTypes
//{
//    public class ListAsset : DataboundAsset
//    {
//        public DataboundValue<Type> Template { get; set; } = new DataboundValue<Type>(); //todo constrain to DataboundTemplate

//        public DataboundValue<float> ItemWidth { get; set; } = new DataboundValue<float>(0.1f);
//        public DataboundValue<float> ItemHeight { get; set; } = new DataboundValue<float>(0.25f);

//        public DataboundValue<List<MGUIViewModel>> Items { get; set; } = new DataboundValue<List<MGUIViewModel>>(new List<MGUIViewModel>());

//        private Dictionary<Guid, DataboundTemplate> populatedTemplates = new Dictionary<Guid, DataboundTemplate>();
//        private int ctx = 0;
//        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
//        {
//            ctx++;
//            if (Template.Value != null)
//            {
//                Vector2 pos = this.Position.ToVector2();

//                foreach (MGUIViewModel item in Items.Value)
//                {
//                    Guid id = item.Id;

//                    if (!populatedTemplates.ContainsKey(id))
//                    {
//                        DataboundTemplate dt = Activator.CreateInstance(Template.Value) as DataboundTemplate;
//                        dt.Screen = new ScreenAbstractor();
//                        dt.DataContext = item;
//                        dt.Initialise().GetAwaiter().GetResult();
//                        populatedTemplates.Add(id, dt);

//                        dt.Bind(item);


//                    }

//                    var tp = populatedTemplates[id];

//                    tp.ProcessDynamicAssets();
//                    var sp = new FloatRectangle(pos.X, pos.Y, ItemWidth.Value, ItemHeight.Value);
//                    var tsp = screen.Translate(sp);
//                    tp.Screen.SetBounds(tsp.Value.ToRectangle);


//                    tp.Draw(new GameTime(), opacity, null);


//                    pos = new Vector2(pos.X, pos.Y + ItemHeight.Value);
//                }
//            }
//        }
//    }
//}
