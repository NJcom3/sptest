using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MazeGeneration
{
    public class MazeRenderer : MonoBehaviour
    {
        [SerializeField] 
        private Tilemap tilemap;
        
        [SerializeField]
        private TileBase floorTile;
        
        [SerializeField] 
        private TileBase wallTile;
        
        [SerializeField] 
        private TileBase startTile;
        
        [SerializeField] 
        private TileBase exitTile;
        
        private int _width;
        private int _height;
        private Cell[,] _grid;

        public Vector3 startPosition;
        public List<Vector3> exitPositions;
        
        public void RenderMaze(Cell[,] cells, int width, int height)
        {
            _grid = cells;
            _width = width;
            _height = height;

            exitPositions = new List<Vector3>();
            DrawToTilemap();
        }

        private void DrawToTilemap()
        {
            tilemap.ClearAllTiles();

            FillByWalls();
            DrawMaze();
            DrawStartTile();
            DrawExits();
        }

        private void FillByWalls()
        {
            var mapW = _width * 2 + 1;
            var mapH = _height * 2 + 1;

            for (var x = 0; x < mapW; x++)
            {
                for (var y = 0; y < mapH; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }

        private void DrawMaze()
        {
            for (var cx = 0; cx < _width; cx++)
            {
                for (var cy = 0; cy < _height; cy++)
                {
                    var cellPos = new Vector3Int(2 * cx + 1, 2 * cy + 1, 0);
                    tilemap.SetTile(cellPos, floorTile);

                    if (!_grid[cx, cy].walls[0])
                    {
                        tilemap.SetTile(cellPos + new Vector3Int(0, 1, 0), floorTile);
                    }

                    if (!_grid[cx, cy].walls[1])
                    {
                        tilemap.SetTile(cellPos + new Vector3Int(1, 0, 0), floorTile);
                    }

                    if (!_grid[cx, cy].walls[2])
                    {
                        tilemap.SetTile(cellPos + new Vector3Int(0, -1, 0), floorTile);
                    }

                    if (!_grid[cx, cy].walls[3])
                    {
                        tilemap.SetTile(cellPos + new Vector3Int(-1, 0, 0), floorTile);
                    }
                }
            }
        }

        private void DrawStartTile()
        {
            for (var cx = 0; cx < _width; cx++)
            {
                for (var cy = 0; cy < _height; cy++)
                {
                    if (!_grid[cx, cy].isEnterTile)
                    {
                        continue;
                    }

                    var startTilePos = new Vector3Int(2 * cx + 1, 2 * cy + 1, 0);
                    tilemap.SetTile(startTilePos, startTile);
                    startPosition = tilemap.GetCellCenterWorld(startTilePos);
                    break;
                }
            }
        }

        private void DrawExits()
        {
            for (var cx = 0; cx < _width; cx++)
            {
                for (var cy = 0; cy < _height; cy++)
                {
                    if (!_grid[cx, cy].isExitTile)
                    {
                        continue;
                    }

                    var cellPos = new Vector3Int(2 * cx + 1, 2 * cy + 1, 0);

                    var exitPos = cellPos;

                    switch (_grid[cx, cy].exitWallDir)
                    {
                        case 0:
                            exitPos += new Vector3Int(0, 1, 0); // Up
                            break;
                        case 1:
                            exitPos += new Vector3Int(1, 0, 0); // Right
                            break;
                        case 2:
                            exitPos += new Vector3Int(0, -1, 0); // Down
                            break;
                        case 3:
                            exitPos += new Vector3Int(-1, 0, 0); // Left
                            break;
                    }

                    tilemap.SetTile(exitPos, exitTile);
                    exitPositions.Add(tilemap.GetCellCenterWorld(exitPos));
                }
            }
        }
    }
}