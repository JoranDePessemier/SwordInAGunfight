using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinState : GameState
{
    private SceneTransition _transition;
    private WinScreen _screen;

    public WinState(GameManager manager) : base(manager)
    {
    }

    public override void OnEnter()
    {
        if(SceneManager.GetActiveScene().name != "WinScreen")
        {
            SceneManager.LoadSceneAsync("WinScreen", LoadSceneMode.Single).completed += InitialiseScene;
        }
        else
        {
            InitialiseScene(null);
        }
        MusicManager.Instance.ChangeMusic("EndTheme");
    }

    private void InitialiseScene(AsyncOperation obj)
    {
        _transition = GameObject.FindAnyObjectByType<SceneTransition>();
        _transition.MoveOut();
        
        _screen = GameObject.FindObjectOfType<WinScreen>();
        _screen.Score = Manager.LatestGameStats.Points;
        _screen.HighScore = Manager.HighScores[Manager.NextLevel];
        _screen.Retry += RetryLevel;
        _screen.Menu += GoToMenu;
    }

    private void GoToMenu(object sender, EventArgs e)
    {
        _transition.MoveIn(() => Manager.ChangeState(GameStateName.MainMenu));
    }

    private void RetryLevel(object sender, EventArgs e)
    {
        _transition.MoveIn(() => Manager.ChangeState(GameStateName.Loading));
    }

    public override void OnExit()
    {
        _screen.Retry -= RetryLevel;
        _screen.Menu -= GoToMenu;
    }
}
