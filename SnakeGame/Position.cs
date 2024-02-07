namespace SnakeGame
{
    public class Position
    {
        public int Row { get; }

        public int Column { get; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Position Translate(Direction direction)
        {
            return new Position(Row + direction.RowOffset, Column + direction.ColumnOffset);
        }
    }
}
