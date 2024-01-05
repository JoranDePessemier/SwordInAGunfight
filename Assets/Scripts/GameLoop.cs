using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class GameStatsEventArgs : EventArgs
{
    public GameStatsEventArgs(bool wonGame, int points, float remainingTime)
    {
        WonGame = wonGame;
        Points = points;
        RemainingTime = remainingTime;
    }

    public bool WonGame { get; private set; }
    public int Points { get; private set; }
    public float RemainingTime { get;private set; }
}

public class GameLoop : MonoBehaviour
{
    private PlayerPickup _pickupScript;
    private PlayerHealth _healthScript;
    private GameUIController _uiController;
    private ExitBehaviour _exitBehaviour;
    private EnemyBaseBehaviour[] _enemies;

    private int _playerPoints;

    private int PlayerPoints { get { return _playerPoints; } set { _playerPoints = value; _uiController?.SetScore(value); } }

    [SerializeField]
    private int _startingPlayerHp;

    private int _currentPlayerHp;

    private bool _isGameOver;
    private bool _timeWarningFired;

    private int CurrentPlayerHp { 
        get 
        {
            return _currentPlayerHp; 
        } 
        set 
        {
            _currentPlayerHp = value; 
            _uiController?.SetHp(value); 

            if(_currentPlayerHp <= 0)
            {
                GameOver();
                _healthScript.Die();
            }
        } 
    }

    [SerializeField]
    private float _startingGameTime;

    [SerializeField]
    private float _timeRemainingWarning;

    [SerializeField]
    private UnityEvent _startWarning;

    [SerializeField]
    private UnityEvent _gameOver;

    [SerializeField]
    private UnityEvent _winGame;

    private float _gameTime;
    private float GameTime { get { return _gameTime; } set { _gameTime = value; _uiController?.SetTimer(value); } }

    public event EventHandler<GameStatsEventArgs> GameEnded;

    private void Start()
    {
        _pickupScript = FindObjectOfType<PlayerPickup>();

        if( _pickupScript != null )
            _pickupScript.PickUpPoints += PickedUpPoints;

        _healthScript = FindObjectOfType<PlayerHealth>();

        if( _healthScript != null )
            _healthScript.TakeDamage += TakenDamage;

        _uiController = FindObjectOfType<GameUIController>();

        _exitBehaviour = FindObjectOfType<ExitBehaviour>();

        if (_exitBehaviour != null)
            _exitBehaviour.ExitLevel += GameWin;

        CurrentPlayerHp = _startingPlayerHp;
        GameTime = _startingGameTime;

        _enemies = FindObjectsOfType<EnemyBaseBehaviour>();
        foreach(EnemyBaseBehaviour enemy in _enemies)
        {
            enemy.Target = _healthScript.GetComponent<Transform>();
        }
    }

    private void Update()
    {
        if(_isGameOver) return;

        if(GameTime < 0)
        {
            GameOver();
        }
        else
        {
            GameTime -= Time.deltaTime;
        }

        if(GameTime <= _timeRemainingWarning && !_timeWarningFired)
        {
            TriggerTimeWarning();
            _timeWarningFired = true;
        }
    }

    private void TriggerTimeWarning()
    {
        MusicManager.Instance.ChangeMusic("GameThemeFast");
        _uiController.StartPulsingTimer();
        _startWarning.Invoke();
        _exitBehaviour.AppearPointer();
    }

    private void GameOver()
    {
        if (_isGameOver)
            return;
        
        _gameOver.Invoke();

        _isGameOver = true;

        OnGameEnded(new GameStatsEventArgs(false, _playerPoints, _gameTime));
    }

    private void GameWin(object sender, EventArgs e)
    {
        if (_isGameOver)
            return;

        _winGame.Invoke();

        _isGameOver = true;

        OnGameEnded(new GameStatsEventArgs(true, _playerPoints, _gameTime));
    }

    private void GameWin() => GameWin(this,EventArgs.Empty);

    private void OnDestroy()
    {
        _pickupScript.PickUpPoints -= PickedUpPoints;
        _healthScript.TakeDamage -= TakenDamage;
    }


    private void PickedUpPoints(object sender, PickupAmountEventArgs e)
    {
        if (_isGameOver) return;
        PlayerPoints += e.Amount;
    }

    private void TakenDamage(object sender, DamageTakenEventArgs e)
    {
        if(_isGameOver) return; 
        CurrentPlayerHp -= e.Damage;
    }

    private void OnGameEnded(GameStatsEventArgs e)
    {
        var handler = GameEnded;
        handler?.Invoke(this, e);
    }
}
