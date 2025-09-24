using System.Collections.Generic;

namespace MazeGeneration
{
    using UnityEngine;

    public class WilsonMaze
    {
        private int _mazeWidth = 10;
        private int _mazeHeight = 10;
        private int _exitsCount = 1;

        private System.Random _rng;
        private Cell[,] _grid;

        private readonly Vector2Int[] _dirs;
        bool InBounds(Vector2Int v) => v.x >= 0 && v.y >= 0 && v.x < _mazeWidth && v.y < _mazeHeight;

        public WilsonMaze()
        {
            _dirs = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left,
            };
        }

        public Cell[,] Generate()
        {
            _rng = new System.Random();

            _mazeHeight = GameController.Instance.Height;
            _mazeWidth = GameController.Instance.Width;
            _exitsCount = GameController.Instance.ExitCount;

            GenerateMaze();
            return _grid;
        }
        
                private void InitializeGrid()
        {
            _grid = new Cell[_mazeWidth, _mazeHeight];

            for (var x = 0; x < _mazeWidth; x++)
            {
                for (var y = 0; y < _mazeHeight; y++)
                {
                    _grid[x, y] = new Cell();
                }
            }
        }

        private void GenerateMaze()
        {
            InitializeGrid();
            GeneratePassages();
            SetEnter();
            SetExits();
        }

        private void GeneratePassages()
        {
            var start = new Vector2Int(_mazeWidth / 2, _mazeHeight / 2);
            _grid[start.x, start.y].inMaze = true;

            var remaining = _mazeWidth * _mazeHeight - 1;

            while (remaining > 0)
            {
                Vector2Int walkStart;

                do
                {
                    walkStart = RandomCell();
                } while (_grid[walkStart.x, walkStart.y].inMaze);

                var path = new List<Vector2Int>();
                var indexOf = new Dictionary<Vector2Int, int>();

                var current = walkStart;
                path.Add(current);
                indexOf[current] = 0;

                while (!_grid[current.x, current.y].inMaze)
                {
                    current = RandomNeighbor(current);

                    if (indexOf.ContainsKey(current))
                    {
                        var cut = indexOf[current];
                        for (var i = path.Count - 1; i > cut; i--)
                        {
                            indexOf.Remove(path[i]);
                            path.RemoveAt(i);
                        }
                    }
                    else
                    {
                        indexOf[current] = path.Count;
                        path.Add(current);
                    }
                }

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    Carve(a, b);

                    if (_grid[a.x, a.y].inMaze)
                    {
                        continue;
                    }

                    _grid[a.x, a.y].inMaze = true;
                    remaining--;
                }

                var last = path[^1];

                if (_grid[last.x, last.y].inMaze)
                {
                    continue;
                }

                _grid[last.x, last.y].inMaze = true;
                remaining--;
            }
        }

        private void Carve(Vector2Int a, Vector2Int b)
        {
            var d = b - a;

            if (d == Vector2Int.up)
            {
                _grid[a.x, a.y].walls[0] = false;
                _grid[b.x, b.y].walls[2] = false;
            }
            else if (d == Vector2Int.right)
            {
                _grid[a.x, a.y].walls[1] = false;
                _grid[b.x, b.y].walls[3] = false;
            }
            else if (d == Vector2Int.down)
            {
                _grid[a.x, a.y].walls[2] = false;
                _grid[b.x, b.y].walls[0] = false;
            }
            else if (d == Vector2Int.left)
            {
                _grid[a.x, a.y].walls[3] = false;
                _grid[b.x, b.y].walls[1] = false;
            }
        }

        private Vector2Int RandomCell()
        {
            return new(_rng.Next(0, _mazeWidth), _rng.Next(0, _mazeHeight));
        }

        private Vector2Int RandomNeighbor(Vector2Int cell)
        {
            var neigh = new List<Vector2Int>();

            foreach (var d in _dirs)
            {
                var p = cell + d;
                if (InBounds(p)) neigh.Add(p);
            }

            return neigh[_rng.Next(0, neigh.Count)];
        }

        private void SetEnter()
        {
            var startLogic = new Vector2Int(_mazeWidth / 2, _mazeHeight / 2);
            _grid[startLogic.x, startLogic.y].isEnterTile = true;
        }

        private void SetExits()
        {
            var boundary = new List<(Vector2Int, int)>();

            for (var x = 0; x < _mazeWidth; x++)
            {
                boundary.Add((new Vector2Int(x, 0), 2));
                boundary.Add((new Vector2Int(x, _mazeHeight - 1), 0));
            }

            for (var y = 0; y < _mazeHeight; y++)
            {
                boundary.Add((new Vector2Int(0, y), 3));
                boundary.Add((new Vector2Int(_mazeWidth - 1, y), 1));
            }

            if (_exitsCount > boundary.Count)
            {
                _exitsCount = boundary.Count;
            }

            for (var i = 0; i < _exitsCount; i++)
            {
                var idx = _rng.Next(0, boundary.Count);
                var (logicCell, wallDir) = boundary[idx];

                boundary.RemoveAt(idx);

                int lx = logicCell.x, ly = logicCell.y;

                _grid[lx, ly].walls[wallDir] = false;
                _grid[lx, ly].isExitTile = true;
                _grid[lx, ly].exitWallDir = wallDir;
            }
        }
    }
}