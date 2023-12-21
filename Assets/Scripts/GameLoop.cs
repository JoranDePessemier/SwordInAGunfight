using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private PlayerPickup _pickupScript;
    private PlayerHealth _healthScript;
    private GameUIController _uiController;
    private ExitBehaviour _exitBehaviour;

    private int _playerPoints;

    private int PlayerPoints { get { return _playerPoints; } set { _playerPoints = value; _uiController?.SetScore(value); } }

    [SerializeField]
    private int _startingPlayerHp;

    private int _currentPlayerHp;

    private bool _isWin;
    private bool _isGameOver;

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
            }
        } 
    }

    [SerializeField]
    private float _startingGameTime;

    private float _gameTime;
    private float GameTime { get { return _gameTime; } set { _gameTime = value; _uiController?.SetTimer(value); } }

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
    }

    private void Update()
    {
        if(GameTime < 0)
        {
            GameOver();
        }
        else
        {
            GameTime -= Time.deltaTime;
        }
    }

    private void GameOver()
    {
        if (_isGameOver || _isWin)
            return;

        _isGameOver = true;

        //TODO: switch to cool screen
        print("GAME OVER GAME OVER GAME OVER");
    }

    private void GameWin(object sender, EventArgs e)
    {
        if (_isGameOver || _isWin)
            return;

        _isWin = true;

        //TODO: switch to cool screen
        print("YOU WON YOU WON YOU WON");
    }

    private void GameWin() => GameWin(this,EventArgs.Empty);

    private void OnDestroy()
    {
        _pickupScript.PickUpPoints -= PickedUpPoints;
        _healthScript.TakeDamage -= TakenDamage;
    }


    private void PickedUpPoints(object sender, PickupAmountEventArgs e)
    {
        PlayerPoints += e.Amount;
    }

    private void TakenDamage(object sender, DamageTakenEventArgs e)
    {
        CurrentPlayerHp -= e.Damage;
    }
}
