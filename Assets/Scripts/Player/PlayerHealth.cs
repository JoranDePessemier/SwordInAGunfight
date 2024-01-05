using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [SerializeField]
    private UnityEvent _hit;

    [SerializeField]
    private UnityEvent _death;

    [SerializeField]
    private GameObject _instantiateOnDeath;

    [SerializeField]
    private Image _hurtScreen;

    [SerializeField]
    private float _hurtScreenFadeInTime;

    [SerializeField]
    private float _hurtScreenFadeOutTime;

    private float _hurtScreenAlpha;
    private GameObject _hurtScreenObject; 

    private bool _isDamageable = true;

    public event EventHandler<DamageTakenEventArgs> TakeDamage;

    private void Awake()
    {
        _hurtScreenAlpha = _hurtScreen.color.a;
        _hurtScreenObject = _hurtScreen.gameObject;
        _hurtScreenObject.SetActive(false);
    }

    public void HitPlayer(int damageToDo)
    {
        if (!_isDamageable) return;

        _isDamageable = false;
        StartCoroutine(Utilities.WaitForTime(_invincibilityTime, () => { _isDamageable = true; }));
        SetAlphaLow();
        OnTakeDamage(new DamageTakenEventArgs(damageToDo));
        _hit.Invoke();
        FadeInHurtScreen();
    }

    private void FadeInHurtScreen()
    {
        _hurtScreen.color = new Color(_hurtScreen.color.r,_hurtScreen.color.g,_hurtScreen.color.b,0);
        _hurtScreenObject.SetActive(true);
        LeanTween.alpha(_hurtScreen.rectTransform, _hurtScreenAlpha,_hurtScreenFadeInTime)
            .setEase(LeanTweenType.linear)
            .setOnComplete(FadeOutHurtScreen);
    }

    private void FadeOutHurtScreen()
    {
        LeanTween.alpha(_hurtScreen.rectTransform, 0, _hurtScreenFadeOutTime)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() => _hurtScreenObject.SetActive(false));
    }

    private void SetAlphaLow()
    {
        if (!_isDamageable)
        {
            _visual.color = new Color(_visual.color.r, _visual.color.g, _visual.color.b, _alphaFlicker);
        }
        StartCoroutine(Utilities.WaitForTime(_timeBetweenFlickers, SetAlphaHigh));

    }

    public void Die()
    {
        Instantiate(_instantiateOnDeath,transform.position,Quaternion.identity);
        _death.Invoke();
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
