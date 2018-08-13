using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Breeze.Helpers
{
    public static class BezierHelper
    {
        public static Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
        {
            
            {
                Vector2[] points = new Vector2[outputSegmentCount + 1];
                for (int i = 0; i <= outputSegmentCount; i++)
                {
                    double t = (double) i / outputSegmentCount;
                    points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
                }

                return points;
            }
        }

        public static Vector2 GetBezierPoint(double t, Vector2[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2((float)((1 - t) * P0.X + t * P1.X), (float)((1 - t) * P0.Y + t * P1.Y));
        }
    }
}
