using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuState : GameState
{
    private SceneTransition _transition;
    private MainMenu _mainMenu;

    public MenuState(GameManager manager) : base(manager)
    {
    }

    public override void OnEnter()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single).completed += InitialiseScene;
            MusicManager.Instance.ChangeMusic("TitleTheme");
        }
        else
        {
            InitialiseScene(null);
        }

    }

    private void InitialiseScene(AsyncOperation obj)
    {
        _transition = GameObject.FindObjectOfType<SceneTransition>();
        _transition.MoveOut();

        _mainMenu = GameObject.FindObjectOfType<MainMenu>();
        if (Manager.MenuOpened)
        {
            _mainMenu.OpenModeSelect();
        }
        _mainMenu.PlayGame += PlayGame;
        _mainMenu.SetHighScores(Manager.HighScores.Values.ToArray());
    }

    private void PlayGame(object sender, PlayGameEventArgs e)
    {
        Manager.NextLevel = e.LevelName;
        _transition.MoveIn(() => Manager.ChangeState(GameStateName.Loading));
    }

    public override void OnExit()
    {
        Manager.MenuOpened = true;
        _mainMenu.PlayGame -= PlayGame; 
    }
}
