namespace Breeze
{
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

        public Thickness(float top, float right, float bottom, float left)
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
            float top = float.Parse(parts[0]);
            float right = float.Parse(parts[1]);
            float bottom = float.Parse(parts[2]);
            float left = float.Parse(parts[3]);

            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left = left;

        }

        public float HorizontalMargin => this.Left + this.Right;
        public float VerticalMargin => this.Top + this.Bottom;
    }
}