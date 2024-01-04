using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _loseText;

    [SerializeField]
    private string _loseTextTime;

    [SerializeField]
    private string _loseTextHealth;

    public event EventHandler<EventArgs> Retry;
    public event EventHandler<EventArgs> Menu;

    public void SetLoseText(float timeRemaining)
    {
        if(timeRemaining > 0f)
        {
            _loseText.text = _loseTextHealth;
        }
        else
        {
            _loseText.text = _loseTextTime;
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
