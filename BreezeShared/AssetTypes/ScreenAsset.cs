using System;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    //public class ScreenAsset
    //{
    //    public int ZIndex { get; set; } = 0;

    //    public FloatRectangle? Clip { get; set; } = null;

    //    public Rectangle? ScissorRect { get; set; }
    //    //public FloatRectangle Position { get; set; }

    //    public virtual void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
    //    {
    //        throw new NotImplementedException();
    //    }

        
    //}

    //public class KeyedAsset : ScreenAsset
    //{
    //    public string Key { get; set; }
    //    public bool IsDirty { get; set; }
    //    public void Update(KeyedAsset asset, KeyedAsset source)
    //    {
    //        if (!string.IsNullOrWhiteSpace(asset.Key))
    //        {
    //            (asset as LineListAsset)?.Update(source as LineListAsset);
    //         //   (asset as ButtonAsset)?.Update(source as ButtonAsset);
    //            //(asset as RectangleAsset)?.Update(source as RectangleAsset);
    //            //(asset as FontAsset)?.Update(source as FontAsset);
    //            //(asset as PatternAsset)?.Update(source as PatternAsset);
    //            (asset as SinglePixelLineAsset)?.Update(source as SinglePixelLineAsset);
    //            //(asset as BezierLineAsset)?.Update(source as BezierLineAsset);
    //            //(asset as ImageAsset)?.Update(source as ImageAsset);
    //           //  (asset as CentredImageAsset)?.Update(source as CentredImageAsset);
    //            //(asset as ContainerAsset)?.Update(source as ContainerAsset);
    //            (asset as BoxShadowAsset)?.Update(source as BoxShadowAsset);
    //            // (asset as ViewportAsset)?.Update(source as ViewportAsset);

    //            asset.Clip = source.Clip;
    //            asset.ScissorRect = source.ScissorRect;
    //        }
    //    }
    //}

    //public class KeyedUpdatedAsset : KeyedAsset
    //{
    //    public virtual void Update(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
