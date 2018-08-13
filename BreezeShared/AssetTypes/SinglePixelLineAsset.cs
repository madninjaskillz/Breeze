using System.Collections.Generic;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Helpers;
using Breeze.Services.InputService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class SinglePixelLineAsset : KeyedAsset
    {
        public Color Color { get; set; }
        public Vector2 StartPositon { get; set; }
        public Vector2 EndPositon { get; set; }

        public int BrushSize { get; set; } = 1;

        public SinglePixelLineAsset()
        {
        }

        public SinglePixelLineAsset(Color color, Vector2 start, Vector2 end, int brushSize = 1)
        {
            Color = color;
            StartPositon = start;
            EndPositon = end;
            BrushSize = brushSize;
        }

        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {

            if (Solids.Instance.InputService.IsPressed(InputService.ShiftKeys.LeftShift1))
            {
              //  return;

            }

            Vector2 sp = new Vector2(StartPositon.X, StartPositon.Y).Move(scrollOffset);
            Vector2 ep = new Vector2(EndPositon.X, EndPositon.Y).Move(scrollOffset);
       


            if (clip != null && sp.X < clip.Value.X) sp.X = clip.Value.X;
            if (clip != null && sp.X > clip.Value.BottomRight.X) sp.X = clip.Value.BottomRight.X;
            if (clip != null && ep.X < clip.Value.X) ep.X = clip.Value.X;
            if (clip != null && ep.X > clip.Value.BottomRight.X) ep.X = clip.Value.BottomRight.X;


            if (clip != null && sp.Y < clip.Value.Y) sp.Y = clip.Value.Y;
            if (clip != null && sp.Y > clip.Value.BottomRight.Y) sp.Y = clip.Value.BottomRight.Y;
            if (clip != null && ep.Y < clip.Value.Y) ep.Y = clip.Value.Y;
            if (clip != null && ep.Y > clip.Value.BottomRight.Y) ep.Y = clip.Value.BottomRight.Y;


            Vector2 start = screen.Translate(sp);
            Vector2 end = screen.Translate(ep);

            FloatRectangle? tclip = screen.Translate(clip);
            if (tclip.HasValue)
            {
                if ((start.X < tclip.Value.X && end.X < tclip.Value.X) || (start.X > tclip.Value.Right && end.X > tclip.Value.Right))
                {
                    return;
                }
            }

            spriteBatch.DrawLine(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y), Color, null, BrushSize);
        }

        internal void Update(SinglePixelLineAsset singlePixelLineAsset)
        {
            this.StartPositon = singlePixelLineAsset.StartPositon;
            this.EndPositon = singlePixelLineAsset.EndPositon;
        }
    }

    public class LineListAsset : KeyedAsset
    {
        public List<SmartSpriteBatch.Line> Lines { get; set; }

        public LineListAsset(List<SmartSpriteBatch.Line> lines)
        {
            Lines = lines;
        }

        public override void Draw(SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            spriteBatch.DrawLines(TranslateLines(Lines, screen, opacity, scrollOffset), screen.Translate(clip));
        }

        internal void Update(LineListAsset singlePixelLineAsset)
        {
            this.Lines = singlePixelLineAsset.Lines;
        }

        List<SmartSpriteBatch.Line> TranslateLines(List<SmartSpriteBatch.Line> lines, ScreenAbstractor screen, float opacity, Vector2? scrollOffset = null)
        {
            List<SmartSpriteBatch.Line> result = new List<SmartSpriteBatch.Line>();
            foreach (SmartSpriteBatch.Line line in lines)
            {
                result.Add(new SmartSpriteBatch.Line(screen.Translate(line.Start.Move(scrollOffset)), screen.Translate(line.End.Move(scrollOffset)), line.Color * opacity, line.BrushSize));
            }

            return result;
        }
    }
}
