using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class GameState
    {
        //positions of the snake parts in the game field
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();

        //randomly assign food fields with a help of this random variable
        private readonly Random random = new Random();

        //buffer for avoiding collisions with snake movement changes
        private readonly LinkedList<Direction> directionChanges = new LinkedList<Direction>();

        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }


        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[rows, columns];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }
        private void AddSnake()
        {
            int row = Rows / 2;

            for (int col = 1; col <= 3; col++) 
            {
                Grid[row, col] = GridValue.Snake;
                snakePositions.AddFirst(new Position(row, col));
            }
        }
        private IEnumerable<Position> EmptyPositions()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Grid[row, col] == GridValue.Empty)
                    {
                        yield return new Position(row, col);
                    }
                }
            }
        }
        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position position = empty[random.Next(empty.Count)];
            Grid[position.Row, position.Column] = GridValue.Food;
        }
        public Position SnakeHeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Position SnakeTailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position position)
        {
            snakePositions.AddFirst(position);
            Grid[position.Row, position.Column] = GridValue.Snake;
        }
        private void RemoveTail() 
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if (directionChanges.Count == 0)
            {
                return Dir;
            }

            return directionChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDirection)
        {
            if (directionChanges.Count == 2)
            {
                return false;
            }

            Direction lastDirection = GetLastDirection();
            return newDirection != lastDirection && newDirection != lastDirection.Opposite();
        }

        public void ChangeDirection(Direction direction)
        {
            if (CanChangeDirection(direction))
            {
            directionChanges.AddLast(direction);
            }
        }
        private bool OutsideGrid(Position position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Column < 0 || position.Column >= Columns; 
        }
        private GridValue NextMove(Position newHeadPosition)
        {
            if(OutsideGrid(newHeadPosition)) 
            {
                return GridValue.Outside;
            }

            if(newHeadPosition == SnakeTailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPosition.Row, newHeadPosition.Column];
        }

        public void Move()
        {
            if (directionChanges.Count  > 0)
            {
                Dir = directionChanges.First.Value;
                directionChanges.RemoveFirst();
            }

            Position newHeadPosition = SnakeHeadPosition().Translate(Dir);
            GridValue hit = NextMove(newHeadPosition);

            if (hit == GridValue.Outside || hit == GridValue.Snake) 
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty) 
            {
                RemoveTail();
                AddHead(newHeadPosition);
            }
            else if (hit == GridValue.Food) 
            {
                AddHead(newHeadPosition);
                Score++;
                AddFood();
            }
        }
    }
}
