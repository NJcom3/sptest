namespace MazeGeneration
{
    public class Cell
    {
        public bool[] walls = {true, true, true, true};
        public bool inMaze = false;
        public bool isEnterTile = false;
        public bool isExitTile = false;
        public int exitWallDir;
    }
}