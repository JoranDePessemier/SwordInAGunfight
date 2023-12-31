using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DamageTakenEventArgs : EventArgs
{
    public DamageTakenEventArgs(int damage)
    {
        Damage = damage;
    }

    public int Damage { get; private set; }
}

public class PlayerHealth : MonoBehaviour
{
    [Header("Values")]
    [SerializeField]
    private float _invincibilityTime;

    [Header("Invincibilityanimation")]
    [SerializeField]
    private SpriteRenderer _visual;

    [SerializeField]
    private float _timeBetweenFlickers;

    [Range(0f,1f)]
    [SerializeField]
    private float _alphaFlicker;

    private bool _isDamageable = true;

    public event EventHandler<DamageTakenEventArgs> TakeDamage;

    public void HitPlayer(int damageToDo)
    {
        if (!_isDamageable) return;

        _isDamageable = false;
        StartCoroutine(Utilities.WaitForTime(_invincibilityTime, () => { _isDamageable = true; }));
        SetAlphaLow();
        OnTakeDamage(new DamageTakenEventArgs(damageToDo));
    }

    private void SetAlphaLow()
    {
        if (!_isDamageable)
        {
            _visual.color = new Color(_visual.color.r, _visual.color.g, _visual.color.b, _alphaFlicker);
        }
        StartCoroutine(Utilities.WaitForTime(_timeBetweenFlickers, SetAlphaHigh));

    }

    private void SetAlphaHigh()
    {
        _visual.color = new Color(_visual.color.r, _visual.color.g, _visual.color.b, 1);
        StartCoroutine(Utilities.WaitForTime(_timeBetweenFlickers, SetAlphaLow));
    }

    private void OnTakeDamage(DamageTakenEventArgs e)
    {
        var handler = TakeDamage;
        handler?.Invoke(this, e);
    }
}
