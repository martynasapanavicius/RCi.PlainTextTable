using System;

namespace RCi.PlainTextTable
{
    public readonly record struct Margin
    {
        private readonly int _left;
        public int Left
        {
            get => _left;
            init
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
                _left = value;
            }
        }

        private readonly int _top;
        public int Top
        {
            get => _top;
            init
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
                _top = value;
            }
        }

        private readonly int _right;
        public int Right
        {
            get => _right;
            init
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
                _right = value;
            }
        }

        private readonly int _bottom;
        public int Bottom
        {
            get => _bottom;
            init
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
                _bottom = value;
            }
        }

        public Margin(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Margin(int horizontal, int vertical) :
            this(horizontal, vertical, horizontal, vertical)
        {
        }

        public Margin(int uniform) :
            this(uniform, uniform, uniform, uniform)
        {
        }

        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public static implicit operator Margin(int universal) =>
            new(universal);

        public static implicit operator Margin((int horizontal, int vertical) tuple) =>
            new(tuple.horizontal, tuple.vertical);

        public static implicit operator Margin((int left, int top, int right, int bottom) tuple) =>
            new(tuple.left, tuple.top, tuple.right, tuple.bottom);

        public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
    }
}
