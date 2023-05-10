using System.Drawing;

namespace AlgVlny;

public class Wave
{
    private readonly int[,] _maze;

    public Wave(int[,] maze)
    {
        this._maze = maze;
    }

    public Wave(int dimension)
    {
        _maze = new int[dimension, dimension];
        Random rnd = new Random();
        for (int i = 0; i < _maze.GetLength(0); i++)
        {
            for (int j = 0; j < _maze.GetLength(1); j++)
            {
                //začátek
                if (i == 0 && j == 0)
                    _maze[i, j] = (int)Cell.Start;
                //cíl
                else if (i == dimension - 1 && j == dimension - 1)
                {
                    _maze[i, j] = (int)Cell.End;
                }
                //zeď
                else if (rnd.Next(1, 5) == 3)
                {
                    _maze[i, j] = (int)Cell.Wall;
                }
                //prázdné místo
                else
                {
                    _maze[i, j] = (int)Cell.Empty;
                }
            }
        }
    }

    public Wave(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Soubor neexistuje!!");
            return;
        }

        var lines = File.ReadAllLines(path);
        _maze = new int[lines.Length, lines[0].Length];

        for (var x = 0; x < lines.Length; x++)
        {
            var t = lines[x];
            for (var y = 0; y < t.Length; y++)
            {
                var c = t[y];
                switch (c)
                {
                    case 'S':
                        _maze[x, y] = (int)Cell.Start;
                        _nextSteps.Add(new Point(x, y));
                        break;
                    case 'E':
                        _maze[x, y] = (int)Cell.End;
                        _nextPathStep = new Point(x, y);
                        break;
                    case 'x':
                        _maze[x, y] = (int)Cell.Wall;
                        break;
                    case ' ':
                        _maze[x, y] = (int)Cell.Empty;
                        break;
                }
            }
        }

        MakeWaves();
        _path.Add(_nextPathStep);
        FindPath();
    }

    private List<Point> _nextSteps = new();
    private List<Point> _path = new();


    private Point _nextPathStep;

    private void FindPath()
    {
        Point? smallestPoint = null;
        if (_path[^1].X - 1 > -1)
        {
            var futurePoint = new Point(_path[^1].X - 1, _path[^1].Y);

            if (_maze[futurePoint.X, futurePoint.Y] > -1)
            {
                smallestPoint = futurePoint;
            }
        }

        if (_path[^1].X + 1 < _maze.GetLength(0))
        {
            var futurePoint = new Point(_path[^1].X + 1, _path[^1].Y);
            if (_maze[futurePoint.X, futurePoint.Y] > -1)
            {
                if (smallestPoint == null)
                {
                    smallestPoint = futurePoint;
                }
                else
                {
                    if (_maze[futurePoint.X, futurePoint.Y] < _maze[smallestPoint.Value.X, smallestPoint.Value.Y])
                    {
                        smallestPoint = futurePoint;
                    }
                }
            }
        }

        if (_path[^1].Y - 1 > -1)
        {
            var futurePoint = new Point(_path[^1].X, _path[^1].Y - 1);
            if (_maze[futurePoint.X, futurePoint.Y] > -1)
            {
                if (smallestPoint == null)
                {
                    smallestPoint = futurePoint;
                }
                else
                {
                    if (_maze[futurePoint.X, futurePoint.Y] < _maze[smallestPoint.Value.X, smallestPoint.Value.Y])
                    {
                        smallestPoint = futurePoint;
                    }
                }
            }
        }

        if (_path[^1].Y + 1 < _maze.GetLength(1))
        {
            var futurePoint = new Point(_path[^1].X, _path[^1].Y + 1);
            if (_maze[futurePoint.X, futurePoint.Y] > -1)
            {
                if (smallestPoint == null)
                {
                    smallestPoint = futurePoint;
                }
                else
                {
                    if (_maze[futurePoint.X, futurePoint.Y] < _maze[smallestPoint.Value.X, smallestPoint.Value.Y])
                    {
                        smallestPoint = futurePoint;
                    }
                }
            }
        }

        _path.Add((Point)smallestPoint);
        
        if (_maze[smallestPoint.Value.X, smallestPoint.Value.Y] != (int)Cell.Start)
        {
            _maze[smallestPoint.Value.X, smallestPoint.Value.Y] = (int)Cell.Path;
            FindPath();
        }
    }

    private void MakeWaves()
    {
        var futureNextSteps = new List<Point>();
        foreach (var step in _nextSteps)
        {
            // -X

            //  X-

            //  |
            //  X

            //  X
            //  |
            if (step.X - 1 > -1)
            {
                var futurePoint = new Point(step.X - 1, step.Y);
                if (_maze[futurePoint.X, futurePoint.Y] == (int)Cell.Empty)
                {
                    _maze[futurePoint.X, futurePoint.Y] = _maze[step.X, step.Y] + 1;
                    futureNextSteps.Add(futurePoint);
                }
            }

            if (step.X + 1 < _maze.GetLength(0))
            {
                var futurePoint = new Point(step.X + 1, step.Y);
                if (_maze[futurePoint.X, futurePoint.Y] == (int)Cell.Empty)
                {
                    _maze[futurePoint.X, futurePoint.Y] = _maze[step.X, step.Y] + 1;
                    futureNextSteps.Add(futurePoint);
                }
            }

            if (step.Y - 1 > -1)
            {
                var futurePoint = new Point(step.X, step.Y - 1);
                if (_maze[futurePoint.X, futurePoint.Y] == (int)Cell.Empty)
                {
                    _maze[futurePoint.X, futurePoint.Y] = _maze[step.X, step.Y] + 1;
                    futureNextSteps.Add(futurePoint);
                }
            }

            if (step.Y + 1 < _maze.GetLength(1))
            {
                var futurePoint = new Point(step.X, step.Y + 1);
                if (_maze[futurePoint.X, futurePoint.Y] == (int)Cell.Empty)
                {
                    _maze[futurePoint.X, futurePoint.Y] = _maze[step.X, step.Y] + 1;
                    futureNextSteps.Add(futurePoint);
                }
            }
        }

        if (futureNextSteps.Count > 0)
        {
            _nextSteps = futureNextSteps;
            MakeWaves();
        }
    }

    public void Print()
    {
        for (int i = 0; i < _maze.GetLength(0); i++)
        {
            for (int j = 0; j < _maze.GetLength(1); j++)
            {
                switch ((Cell)_maze[i, j])
                {
                    case Cell.Start:
                        Console.Write("S");
                        break;
                    case Cell.End:
                        Console.Write("E");
                        break;
                    case Cell.Wall:
                        Console.Write("▓");
                        break;
                    case Cell.Empty:
                        Console.Write("░");
                        break;
                    case Cell.Path:
                        Console.Write(".");
                        break;
                    default:
                        Console.Write(" ");
                        break;
                }
            }

            Console.WriteLine();
        }
    }
}