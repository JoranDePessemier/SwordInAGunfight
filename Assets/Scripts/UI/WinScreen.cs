using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class WinScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    private int _score;

    public event EventHandler<EventArgs> Retry;
    public event EventHandler<EventArgs> Menu;

    public int Score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            _scoreText.text = $"Score: {_score:0000}";
        }
    }

    private int _highScore;

    public int HighScore
    {
        get
        {
            return _highScore;
        }
        set
        {
            _highScore = value;
            _highScoreText.text = $"High Score: {_highScore:0000}";
        }
    }

    public void OnRetry()
    {
        var handler = Retry;
        handler?.Invoke(this, EventArgs.Empty);
    }

    public void OnMenu()
    {
        var handler = Menu;
        handler?.Invoke(this, EventArgs.Empty);
    }
}
