using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;

    [SerializeField]
    private TextMeshProUGUI _hpText;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    public void SetHp(int hp)
    {
        _hpText.text = $"Health {hp:0}";
    }

    public void SetScore(int score)
    {
        _scoreText.text = $"Score {score:000}";
    }

    public void SetTimer(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        _timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
