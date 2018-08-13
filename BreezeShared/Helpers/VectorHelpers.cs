using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.AssetTypes.DataBoundTypes;
using Microsoft.Xna.Framework;

namespace Breeze.Helpers
{
    public static class VectorHelpers
    {
        public static Vector2 ToVector2(this DataboundAsset.DataboundValue<FloatRectangle> rect)
        {
            return rect.Value.ToVector2;
        }

        public static FloatRectangle ConstrainTo(this FloatRectangle input, FloatRectangle constrain)
        {
            return new FloatRectangle(
                constrain.X + ( input.X),
                constrain.Y + ( input.Y),
                input.Width,
                 input.Height
            );

            return new FloatRectangle(
                constrain.X + (constrain.Width * input.X),
                constrain.Y + (constrain.Width * input.Y),
                constrain.Width*input.Width,
                constrain.Height*input.Height
                );
        }

        public static Color ToColor(this uint argb)
        {
            return new Color(
                (byte)((argb & 0xff0000) >> 0x10),
                (byte)((argb & 0xff00) >> 8),
                (byte)(argb & 0xff),
                (byte)((argb & -16777216) >> 0x18));
        }
        public static Color ToColor(this string argb)
        {
            argb = argb.Replace("#", "");
            byte a = System.Convert.ToByte("ff", 16);
            byte pos = 0;
            if (argb.Length == 8)
            {
                a = System.Convert.ToByte(argb.Substring(pos, 2), 16);
                pos = 2;
            }
            byte r = System.Convert.ToByte(argb.Substring(pos, 2), 16);
            pos += 2;
            byte g = System.Convert.ToByte(argb.Substring(pos, 2), 16);
            pos += 2;
            byte b = System.Convert.ToByte(argb.Substring(pos, 2), 16);
            return new Color(r, g, b, a);
        }
        public static Vector2 ToVector(this float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }

        public static float DifferenceBetweenAnglesInDegrees(this float angle1, float angle2)
        {
            float a1 = MathHelper.ToDegrees(angle1);
            float a2 = MathHelper.ToDegrees(angle2);

            float dif = (float)Math.Abs(a1 - a2) % 360;

            if (dif > 180)
                dif = 360 - dif;

            return dif;

        }

        public static Vector2 Clip(this Vector2 input, FloatRectangle? clip)
        {
            Vector2 result = new Vector2(input.X, input.Y);
            if (clip == null) return result;

            if (result.X < clip.Value.X) result.X = clip.Value.X;
            if (result.Y < clip.Value.Y) result.Y = clip.Value.Y;

            if (result.X > clip.Value.BottomRight.X) result.X = clip.Value.BottomRight.X;
            if (result.Y > clip.Value.BottomRight.Y) result.X = clip.Value.BottomRight.Y;

            return result;
        }

        public static float FontScale
        {
            get
            {
                float returnValue = 1920f / Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds.Width;
                float alt = 1080f / Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds.Height;
                if (alt > returnValue) returnValue = alt;
                return returnValue;
            }
        }

        public static void DoBorderAction(Action<int,int> action)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    action(x, y);
                }
            }
        }

        public static Rectangle Add(this Rectangle rect, Vector2 input)
        {
            return new Rectangle(rect.X + (int) input.X, rect.Y + (int) input.Y, rect.Width, rect.Height);
        }

        public static Rectangle Clip(this Rectangle rect, FloatRectangle? clip)
        {
            if (clip == null) return rect;

            int x = rect.X;
            int y = rect.Y;
            int x2 = rect.Right;
            int y2 = rect.Bottom;

            if (x < clip.Value.X) x = (int)clip.Value.X;
            if (y < clip.Value.Y) y = (int)clip.Value.Y;
            if (x > clip.Value.X + clip.Value.Width) x = (int)(clip.Value.X + clip.Value.Width);
            if (y > clip.Value.Y + clip.Value.Height) y = (int)(clip.Value.Y + clip.Value.Height);
            if (x2 < clip.Value.X) x2 = (int)clip.Value.X;
            if (y2 < clip.Value.Y) y2 = (int)clip.Value.Y;
            if (x2 > clip.Value.X + clip.Value.Width) x2 = (int)(clip.Value.X + clip.Value.Width);
            if (y2 > clip.Value.Y + clip.Value.Height) y2 = (int)(clip.Value.Y + clip.Value.Height);

            return new Rectangle(x, y, x2 - x, y2 - y);
        }

        public static FloatRectangle Clip(this FloatRectangle rect, FloatRectangle? clip)
        {
            if (clip == null) return rect;

            float x = rect.X;
            float y = rect.Y;
            float x2 = rect.Right;
            float y2 = rect.Bottom;

            if (x < clip.Value.X) x = (int)clip.Value.X;
            if (y < clip.Value.Y) y = (int)clip.Value.Y;
            if (x > clip.Value.X + clip.Value.Width) x = (int)(clip.Value.X + clip.Value.Width);
            if (y > clip.Value.Y + clip.Value.Height) y = (int)(clip.Value.Y + clip.Value.Height);
            if (x2 < clip.Value.X) x2 = (int)clip.Value.X;
            if (y2 < clip.Value.Y) y2 = (int)clip.Value.Y;
            if (x2 > clip.Value.X + clip.Value.Width) x2 = (int)(clip.Value.X + clip.Value.Width);
            if (y2 > clip.Value.Y + clip.Value.Height) y2 = (int)(clip.Value.Y + clip.Value.Height);

            return new FloatRectangle(x, y, x2 - x, y2 - y);
        }


        public static FloatRectangle Clamp(this FloatRectangle rect, Rectangle? clamp, bool leftClampOnly = false)
        {
            if (!clamp.HasValue) return rect;

            return Clamp(rect, new FloatRectangle(clamp.Value), leftClampOnly);
        }

        public static FloatRectangle Clamp(this FloatRectangle? rect, Rectangle? clamp, bool leftClampOnly = false)
        {
            if (!clamp.HasValue)
            {
                if (rect.HasValue)
                {

                    return rect.Value;
                }
                else
                {
                    return new FloatRectangle(Solids.Bounds);
                }
            }

            return Clamp(rect.Value, new FloatRectangle(clamp.Value), leftClampOnly);
        }


        public static FloatRectangle Clamp(this FloatRectangle rect, Rectangle clamp, bool leftClampOnly = false)
        {
            return Clamp(rect, new FloatRectangle(clamp), leftClampOnly);
        }

        public static FloatRectangle Clamp(this FloatRectangle? rect, Rectangle clamp, bool leftClampOnly = false)
        {
            if (rect == null)
            {
                return Clamp(new FloatRectangle(Solids.Bounds), new FloatRectangle(clamp), leftClampOnly);
            }

            return Clamp(rect.Value, new FloatRectangle(clamp), leftClampOnly);
        }
        public static FloatRectangle? Move(this FloatRectangle? fr, Vector2? vector2d)
        {
            if (fr.HasValue)
            {
                if (vector2d.HasValue)
                {
                    Vector2 vector2 = vector2d.Value;
                    return new FloatRectangle(fr.Value.X + vector2.X, fr.Value.Y + vector2.Y, fr.Value.Width, fr.Value.Height);
                }
                else
                {
                    return fr.Value;
                }
            }
            else
            {
                return null;
            }

        }

        public static Vector2 Move(this Vector2 input, Vector2? translationVector)
        {
            if (!translationVector.HasValue) return input;
            return input + translationVector.Value;
        }
        public static FloatRectangle Clamp(this FloatRectangle rect, FloatRectangle? clamp, bool leftClampOnly = false)
        {
//            using (new BenchMark())
            {
                if (clamp.HasValue == false) return rect;

                float tx = rect.X;
                float ty = rect.Y;
                float bx = rect.BottomRight.X;
                float by = rect.BottomRight.Y;

                if (tx < clamp.Value.X) tx = clamp.Value.X;
                if (ty < clamp.Value.Y) ty = clamp.Value.Y;
                if (!leftClampOnly)
                {
                    if (bx > clamp.Value.BottomRight.X) bx = clamp.Value.BottomRight.X;
                    if (by > clamp.Value.BottomRight.Y) by = clamp.Value.BottomRight.Y;
                }

                float width = bx - tx;
                float height = by - ty;

                if (width < 0 | height < 0)
                {
                    return new FloatRectangle(0, 0, 0, 0);
                }

                return new FloatRectangle(tx, ty, width, height);
            }
        }

        public static Vector2 Clamp(this Vector2 rect, FloatRectangle clamp, bool leftClampOnly = false)
        {
            float tx = rect.X;
            float ty = rect.Y;


            if (tx < clamp.X) tx = clamp.X;
            if (ty < clamp.Y) ty = clamp.Y;
            if (!leftClampOnly)
            {
                if (tx > clamp.BottomRight.X) tx = clamp.BottomRight.X;
                if (ty > clamp.BottomRight.Y) ty = clamp.BottomRight.Y;
            }

            return new Vector2(tx, ty);
        }

        public static bool Intersects(this Rectangle rect, Vector2 pos)
        {
            return pos.X > rect.X && pos.X < rect.Right && pos.Y > rect.Y && pos.Y < rect.Bottom;
        }

        public static bool Intersects(this FloatRectangle rect, Vector2 pos)
        {
            return pos.X > rect.X && pos.X < rect.Right && pos.Y > rect.Y && pos.Y < rect.Bottom;
        }


        public static bool Intersects(this  Vector2 pos, Rectangle rect)
        {
            return pos.X > rect.X && pos.X < rect.Right && pos.Y > rect.Y && pos.Y < rect.Bottom;
        }

        public static bool Intersects(this Vector2 pos, FloatRectangle rect)
        {
            return pos.X > rect.X && pos.X < rect.Right && pos.Y > rect.Y && pos.Y < rect.Bottom;
        }
    }
}
