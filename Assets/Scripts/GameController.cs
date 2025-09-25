using System;
using DG.Tweening;
using GameSession;
using MazeGeneration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private Image loader;

    private GameSessionController _gameSessionController;
    private Cell[,] _grid;

    private int _height;
    private int _width;
    private int _exitsCount;

    // player offset on start 4 pretty placing on spawn point
    private static readonly Vector3 additionalStartPosHeight = new Vector3(0, 0.5f, 0);

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

    private void Start()
    {
        HideLoader();
    }

    public void PlayNewGame(int width, int height, int countOfExits)
    {
        _height = height;
        _width = width;
        _exitsCount = countOfExits;

        var generator = new WilsonMaze();
        _grid = generator.Generate();
        ShowLoader(() => SceneManager.LoadScene("PlayScene"));
    }

    public void GoToMenuScene()
    {
        ShowLoader(LoadMenuSceneAndHideLoader);
    }

    private void LoadMenuSceneAndHideLoader()
    {
        SceneManager.LoadScene("MenuScene");
        HideLoader();
    }

    public void OnPlaySceneLoaded(GameSessionController gameSessionController)
    {
        _gameSessionController = gameSessionController;
        _gameSessionController.mazeRenderer.RenderMaze(_grid, _grid.GetLength(0), _grid.GetLength(1));

        PlacePlayer();
        PlaceExits();
        HideLoader(() => _gameSessionController.StartGame());
    }

    private void PlacePlayer()
    {
        var player = Instantiate(
            playerPrefab,
            _gameSessionController.mazeRenderer.startPosition + additionalStartPosHeight,
            default
        );

        player.SetAddDistanceAction(_gameSessionController.AddDistance);
        player.SetExitFoundedAction(_gameSessionController.ExitFounded);
    }

    private void PlaceExits()
    {
        foreach (var exitPosition in _gameSessionController.mazeRenderer.exitPositions)
        {
            Instantiate(exitPrefab, exitPosition, default);
        }
    }

    private void ShowLoader(Action onComplete = null)
    {
        loader.gameObject.SetActive(true);
        loader.DOFade(1f, 0.5f).OnComplete(() => onComplete?.Invoke());
    }

    private void HideLoader(Action onComplete = null)
    {
        loader.DOFade(0f, 0.5f).OnComplete(() =>
        {
            loader.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}