using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseState : GameState
{
    private SceneTransition _transition;
    private LoseScreen _screen;

    public LoseState(GameManager manager) : base(manager)
    {
    }

    public override void OnEnter()
    {
        if (SceneManager.GetActiveScene().name != "LoseScreen")
        {
            SceneManager.LoadSceneAsync("LoseScreen", LoadSceneMode.Single).completed += InitialiseScene;
        }
        else
        {
            InitialiseScene(null);
        }
        MusicManager.Instance.StopAllMusic();
    }

    private void InitialiseScene(AsyncOperation obj)
    {
        _transition = GameObject.FindAnyObjectByType<SceneTransition>();
        _transition.MoveOut();

        _screen = GameObject.FindObjectOfType<LoseScreen>();
        _screen.SetLoseText(Manager.LatestGameStats.RemainingTime);
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
