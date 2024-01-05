using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingState : GameState
{
    private SceneTransition _transition;
    private GameObject _camera;

    public LoadingState(GameManager manager) : base(manager)
    {
    }

    public override void OnEnter()
    {
        if(SceneManager.GetActiveScene().name != "LoadingScreen")
        {
            SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Single).completed += InitialiseScene;
        }
        else
        {
            InitialiseScene(null);
        }
    }

    private void InitialiseScene(AsyncOperation obj)
    {
        _transition = GameObject.FindObjectOfType<SceneTransition>();
        _transition.MoveOut(LoadGameScene);
        _camera = GameObject.FindObjectOfType<Camera>().gameObject;
    }

    private void LoadGameScene()
    {
        PlayingState nextLevel = Manager.States[Manager.NextLevel] as PlayingState;

        SceneManager.LoadSceneAsync(nextLevel.LevelName, LoadSceneMode.Additive).completed += GameSceneLoaded;
    }

    private void GameSceneLoaded(AsyncOperation obj)
    {
        _camera.SetActive(false);
        _transition.gameObject.SetActive(false);
        Manager.ChangeState(Manager.NextLevel);
    }

    public override void OnExit()
    {
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }
}
