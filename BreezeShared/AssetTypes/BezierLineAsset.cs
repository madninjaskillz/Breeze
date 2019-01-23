using System;
using System.Collections.Generic;
using System.Linq;
using Breeze.Services.InputService;
using Breeze.Helpers;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze.AssetTypes
{
    public class BezierLineAsset : DataboundAsset
    {
        private Vector2[] approxPoints;
        public DataboundValue<Color> Color { get; set; } = new DataboundValue<Color>();
        public DataboundValue<Color> ToColor { get; set; } = new DataboundValue<Color>();

        public DataboundValue<List<Vector2>> Points { get; set; } = new DataboundValue<List<Vector2>>();

        public DataboundValue<int> BrushSize { get; set; } = new DataboundValue<int>(1);

        public BezierLineAsset()
        {
        }

        public BezierLineAsset(Color color, List<Vector2> points, int brushSize = 1)
        {
            Color.Value = color;
            Points.Value = points;
            SetPoints(Points.Value);
            BrushSize.Value = brushSize;

            Points.SetChangeAction(OnPointsChange);
        }

        public override void Draw(BaseScreen.Resources screenResources, SmartSpriteBatch spriteBatch, ScreenAbstractor screen, float opacity, FloatRectangle? clip = null, Texture2D bgTexture = null, Vector2? scrollOffset = null)
        {
            //    if (approxPoints == null)

            if (Points == null || Points.Value() == null)
            {
                return;
            }

            SetPoints(Points.Value());

            var drawPoints = approxPoints.Select(screen.Translate).ToArray();

            if (ToColor.HasValue())
            {
                spriteBatch.DrawLine(drawPoints, Color.Value(), ToColor.Value(), null, BrushSize.Value);
            }
            else
            {
                spriteBatch.DrawLine(drawPoints, Color.Value(), null, BrushSize.Value);
            }
        }

        private void OnPointsChange()
        {
            SetPoints(Points.Value());
        }
        private void SetPoints(List<Vector2> controlPoints)
        {
            approxPoints = BezierHelper.GetBezierApproximation(controlPoints.ToArray(), 64);
        }
    }


}
