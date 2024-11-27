using System;

namespace RCi.Toolbox
{
    public readonly record struct Borders
    {
        public static readonly Borders None = new(Border.None);
        public static readonly Borders Normal = new(Border.Normal);
        public static readonly Borders Bold = new(Border.Bold);

        private readonly Border _left;
        public Border Left
        {
            get => _left;
            init
            {
                if (value is not (Border.None or Border.Normal or Border.Bold))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _left = value;
            }
        }

        private readonly Border _top;
        public Border Top
        {
            get => _top;
            init
            {
                if (value is not (Border.None or Border.Normal or Border.Bold))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _top = value;
            }
        }

        private readonly Border _right;
        public Border Right
        {
            get => _right;
            init
            {
                if (value is not (Border.None or Border.Normal or Border.Bold))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _right = value;
            }
        }

        private readonly Border _bottom;
        public Border Bottom
        {
            get => _bottom;
            init
            {
                if (value is not (Border.None or Border.Normal or Border.Bold))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _bottom = value;
            }
        }

        public Borders(Border left, Border top, Border right, Border bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Borders(Border horizontal, Border vertical) :
            this(horizontal, vertical, horizontal, vertical)
        {
        }

        public Borders(Border uniform) :
            this(uniform, uniform, uniform, uniform)
        {
        }

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }
}
