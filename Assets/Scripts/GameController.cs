using MazeGeneration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameObject exitPrefab;

    private GameSessionController _gameSessionController;
    public static GameController Instance = null;
    private Cell[,] _grid;

    private int _height;
    private int _width;
    private int _exitsCount;

    public int Height => _height;
    public int Width => _width;
    public int ExitCount => _exitsCount;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void PlayNewGame(int width, int height, int countOfExits)
    {
        _height = height;
        _width = width;
        _exitsCount = countOfExits;

        var generator = new WilsonMaze();
        _grid = generator.Generate();
        SceneManager.LoadScene("PlayScene");
    }

    public void OnPlaySceneLoaded(GameSessionController gameSessionController)
    {
        _gameSessionController = gameSessionController;
        _gameSessionController.renderer.RenderMaze(_grid, _grid.GetLength(0), _grid.GetLength(1));
      
        PlacePlayer();
        PlaceExits();
        
        _gameSessionController.StartGame();
    }

    private void PlacePlayer()
    {
        var player = Instantiate(playerPrefab);
        player.transform.position = _gameSessionController.renderer.startPosition;
        player.SetAddDistanceAction(_gameSessionController.AddDistance);
        player.SetExitFoundedAction(_gameSessionController.ExitFounded);
    }

    private void PlaceExits()
    {
        foreach (var exitPosition in _gameSessionController.renderer.exitPositions)
        {
            var exit = Instantiate(exitPrefab);
            exit.transform.position = exitPosition;
        }
    }
}