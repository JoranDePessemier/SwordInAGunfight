using UnityEngine;

public class PlayingState : GameState
{
    private GameLoop _gameLoop;
    private SceneTransition _transition;
    public string LevelName { get; private set; }

    public PlayingState(GameManager manager, string levelName) : base(manager)
    {
        LevelName = levelName;
    }

    public override void OnEnter()
    {
        _gameLoop = GameObject.FindObjectOfType<GameLoop>();
        _gameLoop.GameEnded += OnGameEnded;
        _transition = GameObject.FindObjectOfType<SceneTransition>();
        _transition.MoveOut();

        MusicManager.Instance.ChangeMusic("GameThemeNormal");
    }

    private void OnGameEnded(object sender, GameStatsEventArgs e)
    {
        Manager.LatestGameStats = e;
        if (e.WonGame)
        {
            OnGameWin();
        }
        else
        {
            OnGameLoss();
        }
    }

    private void OnGameLoss()
    {
        _transition.MoveIn(() => Manager.ChangeState(GameStateName.GameOver));
    }

    private void OnGameWin()
    {
        _transition.MoveIn(() => Manager.ChangeState(GameStateName.Win));
    }

    public override void OnExit()
    {
        _gameLoop.GameEnded -= OnGameEnded;
    }
}

