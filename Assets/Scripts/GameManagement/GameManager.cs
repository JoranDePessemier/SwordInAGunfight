using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR.Haptics;

public enum GameStateName
{
    MainMenu,
    Loading,
    Level1,
    Level2,
    Level3,
    GameOver,
    Win
}

public class GameManager : MonoBehaviour
{
    public Dictionary<GameStateName, GameState> States { get; private set; } = new Dictionary<GameStateName, GameState>();

    public GameState CurrentState { get { return States[_currentStateName]; } }

    private GameStateName _currentStateName;

    [SerializeField]
    private GameStateName _initialStateName;

    private GameStatsEventArgs _latestGameStats = new GameStatsEventArgs(false,0,0);

    public GameStateName NextLevel { get; set; } = GameStateName.Level3;

    public GameStatsEventArgs LatestGameStats
    {
        get
        {
            return _latestGameStats;
        }
        set
        {
            _latestGameStats = value;
            if (value.WonGame && HighScores.ContainsKey(_currentStateName))
            {
                if(_latestGameStats.Points > HighScores[_currentStateName])
                    HighScores[_currentStateName] = _latestGameStats.Points;
            }
        }
    }

    public Dictionary<GameStateName, int> HighScores { get; private set; } = new Dictionary<GameStateName, int>();

    private void Awake()
    {
        if(FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        InitialiseStates();
    }

    private void Start()
    {
        _currentStateName = _initialStateName;
        CurrentState.OnEnter();
    }

    private void InitialiseStates()
    {
        States.Add(GameStateName.Level1, new PlayingState(this, "Rooms"));
        States.Add(GameStateName.Level2, new PlayingState(this, "WaveFunctionCollapse"));
        States.Add(GameStateName.Level3, new PlayingState(this, "RandomWalk"));
        States.Add(GameStateName.Loading, new LoadingState(this));
        States.Add(GameStateName.Win, new WinState(this));

        HighScores.Add(GameStateName.Level1, 0);
        HighScores.Add(GameStateName.Level2, 0);
        HighScores.Add(GameStateName.Level3, 0);
    }

    public void ChangeState(GameStateName stateName)
    {
        CurrentState.OnExit();
        _currentStateName = stateName;
        CurrentState.OnEnter();
    }
}
