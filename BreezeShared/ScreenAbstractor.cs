using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breeze
{
    public class ScreenAbstractor
    {
        public FloatRectangle bounds;

        public Vector2 Translate(Vector2 input) => new Vector2(bounds.X+(bounds.Width*input.X), bounds.Y+(bounds.Height*input.Y));

        public FloatRectangle? Translate(FloatRectangle? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            var b = bounds;

            if (input.Value.Boundless)
            {
                b = new FloatRectangle(Solids.Bounds);
            }

            return new FloatRectangle(
                b.X + (b.Width * input.Value.X),
                b.Y + (b.Height * input.Value.Y),
                b.Width * input.Value.Width,
                b.Height * input.Value.Height
            );
        }

        public Vector3 FromScreenSpace(Vector2 v)
        {
            float hw = bounds.Width / 2f;
            float hh = bounds.Height / 2f;

            float x = v.X - hw;
            x = x / hw;

            float y = v.Y - hh;
            y = y / hh;

            var result = new Vector3(x,-y,0);

            return result;
        }

        public void SetBounds(Rectangle bounds)
        {
            this.bounds = new FloatRectangle(bounds);
        }

        public void SetBounds(FloatRectangle bounds)
        {
            this.bounds = bounds;
        }

        public Vector2 AbstractedSize(Texture2D texture)
        {
            return new Vector2(texture.Width/bounds.Width, texture.Height/bounds.Height);
        }

        public bool IsWithinBounds(FloatRectangle fr)
        {
            return ((fr.BottomRight.X > bounds.X || fr.TopLeft.X < bounds.BottomRight.X )&&
                    (fr.BottomRight.Y > bounds.Y || fr.TopLeft.Y < bounds.BottomRight.Y));
        }
    }

    public class Thickness 
    {
        public readonly float Top;
        public readonly float Left;
        public readonly float Right;
        public readonly float Bottom;

        public Thickness()
        {
            Top = 0;
            Left = 0;
            Right = 0;
            Bottom = 0;
        }
        
        public Thickness(float uniform)
        {
            Top = uniform;
            Left = uniform;
            Right = uniform;
            Bottom = uniform;
        }

        public Thickness(float topAndBottom, float leftAndRight)
        {
            Top = topAndBottom;
            Left = leftAndRight;
            Right = leftAndRight;
            Bottom = topAndBottom;
        }

        public Thickness(float top,float right, float bottom,float left)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public Thickness(string input)
        {
            if (input.StartsWith("\"")) input = input.Substring(1);
            if (input.EndsWith("\"")) input = input.Substring(0, input.Length - 1);

            var parts = input.Split(',');
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float w = float.Parse(parts[2]);
            float h = float.Parse(parts[3]);

            this.Top = x;
            this.Right = y;
            this.Bottom = w;
            this.Left = h;

        }
    }

    public struct FloatRectangle : IComparable
    {
        public override string ToString()
        {
            return $"X:{X}, Y:{Y}, W:{Width}, H:{Height}";
        }

        public float X;
        public float Y;
        public float Width;
        public float Height;
        public bool Boundless;
        public Vector2 TopLeft => new Vector2(X,Y);
        public Vector2 BottomRight =>new Vector2(X+Width,Y+Height);

        public float Right => X + Width;
        public float Bottom => Y + Height;
        public Rectangle ToRectangle => new Rectangle((int)X,(int)Y,(int)Width,(int)Height);
        public Vector2 ToVector2 => new Vector2(X, Y);
        public FloatRectangle(float x, float y, float width, float height, bool boundless = false)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Boundless = boundless;
        }

        public FloatRectangle(Rectangle rectangle, bool boundless = false)
        {
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;

            this.Boundless = boundless;
        }

        public FloatRectangle(FloatRectangle rectangle, bool boundless = false)
        {
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;

            this.Boundless = boundless;

        }

        public FloatRectangle(string input)
        {
            if (input.StartsWith("\"")) input = input.Substring(1);
            if (input.EndsWith("\"")) input = input.Substring(0,input.Length-1);

            var parts = input.Split(',');
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float w = float.Parse(parts[2]);
            float h = float.Parse(parts[3]);

            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;

            this.Boundless = false;
        }

        public static FloatRectangle FullSize()
        {
            return new FloatRectangle(0, 0, 1, 1);
        }


        public FloatRectangle Move(Vector2 vector2)
        {
            return new FloatRectangle(this.X + vector2.X, this.Y + vector2.Y, this.Width, this.Height);
        }

        public FloatRectangle Move(Vector2? vector2d)
        {
            if (vector2d.HasValue)
            {
                Vector2 vector2 = vector2d.Value;
                return new FloatRectangle(this.X + vector2.X, this.Y + vector2.Y, this.Width, this.Height);
            }
            else
            {
                return this;
            }
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
