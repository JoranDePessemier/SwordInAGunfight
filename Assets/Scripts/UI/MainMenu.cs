using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayGameEventArgs : EventArgs
{
    public PlayGameEventArgs(GameStateName levelName)
    {
        LevelName = levelName;
    }

    public GameStateName LevelName { get; private set;}
}

public class MainMenu : MonoBehaviour
{
    private SceneTransition _transition;

    [SerializeField]
    private GameObject[] _menus;

    [SerializeField]
    private GameObject _initialOpenMenu;

    [SerializeField]
    private GameObject _modeSelectMenu;

    [SerializeField]
    private TextMeshProUGUI[] _highScoreTexts;

    private GameObject _openMenu;

    public event EventHandler<PlayGameEventArgs> PlayGame;

    public GameObject OpenMenu
    {
        get
        {
            return _openMenu;
        }
        set
        {
            if (_openMenu == value) return;

            _openMenu?.SetActive(false);
            value.SetActive(true);
            _openMenu = value;
        }
    }

    private void Awake()
    {
        foreach(var menu in _menus)
        {
            menu.SetActive(false);
        }

        if(OpenMenu == null)
        {
            OpenMenu = _initialOpenMenu;
        }

        _transition = FindObjectOfType<SceneTransition>();
    }

    public void OpenMenuTransition(GameObject menu)
    {
        _transition.MoveIn(() => { OpenMenu = menu; _transition.MoveOut(); });
    }

    public void PlayLevel1() => OnPlayGame(new PlayGameEventArgs(GameStateName.Level1));
    public void PlayLevel2() => OnPlayGame(new PlayGameEventArgs(GameStateName.Level2));
    public void PlayLevel3() => OnPlayGame(new PlayGameEventArgs(GameStateName.Level3));

    public void StartMusic()
    {
        MusicManager.Instance.ChangeMusic("TitleTheme");
    }

    private void OnPlayGame(PlayGameEventArgs e)
    {
        var handler = PlayGame;
        PlayGame?.Invoke(this, e);
    }

    public void SetHighScores(int[] scores)
    {
        for (int i = 0; i < _highScoreTexts.Length; i++)
        {
            _highScoreTexts[i].text = $"High Score: {scores[i]:0000}";
        }
    }

    internal void OpenModeSelect()
    {
        OpenMenu = _modeSelectMenu;
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}
