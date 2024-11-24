using System;

namespace RCi.PlainTextTable
{
    public readonly record struct Coordinate(int Row, int Col) : IComparable<Coordinate>
    {
        public static implicit operator Coordinate((int, int) t) => new(t.Item1, t.Item2);

        public int CompareTo(Coordinate other)
        {
            var rowComparison = Row.CompareTo(other.Row);
            return rowComparison != 0
                ? rowComparison
                : Col.CompareTo(other.Col);
        }

        public Coordinate MoveLeft() => this with { Col = Col - 1 };
        public Coordinate MoveRight() => this with { Col = Col + 1 };
        public Coordinate MoveUp() => this with { Row = Row - 1 };
        public Coordinate MoveDown() => this with { Row = Row + 1 };
    }
}
