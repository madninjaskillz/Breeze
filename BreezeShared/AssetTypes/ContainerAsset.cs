using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    //public class ContainerAsset : KeyedAsset
    //{
    //    private FloatRectangle floatRectangle;

    //    public ContainerAsset()
    //    {
    //    }

    //    public ContainerAsset(FloatRectangle floatRectangle)
    //    {
    //        Position = floatRectangle;
    //    }
        
    //    public List<KeyedAsset> FixedAssets { get; set; } = new List<KeyedAsset>();
    //    public List<KeyedAsset> ScrollingAssets { get; set; } = new List<KeyedAsset>();

    //    public Vector2 Offset { get; set; } = Vector2.Zero;

    //    public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null,Vector2? scrollOffset = null)
    //    {
    //        //base.Draw(spriteBatch, screen, opacity, clip, bgTexture);
    //        clip = Position;
    //        foreach (KeyedAsset screenAsset in ScrollingAssets)
    //        {
    //            screenAsset.Draw(spriteBatch, screen, opacity, clip, bgTexture, Position.Move(Offset.Move(scrollOffset)).ToVector2);
    //        }

    //        foreach (KeyedAsset screenAsset in FixedAssets)
    //        {
    //            screenAsset.Draw(spriteBatch, screen, opacity, clip, bgTexture);
    //        }
    //    }

    //    internal void Update(ContainerAsset rectangleAsset)
    //    {
    //        Position = rectangleAsset.Position;
    //    }
    //}
}
