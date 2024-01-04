using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PulseUIVisual : MonoBehaviour
{
    [SerializeField]
    private Vector2 _maxScale;

    [SerializeField]
    private float _time;

    [SerializeField]
    private bool _autoPulse;

    private RectTransform _UIImage;
    private bool _isPulsing;

    private void Awake()
    {
        _UIImage = GetComponent<RectTransform>();
        if (_autoPulse)
            StartPulsing();
    }

    private void OnEnable()
    {
        if (_autoPulse)
            StartPulsing();
    }

    public void StartPulsing()
    {
        if (_isPulsing)
            return;

        _isPulsing = true;
        PulseUp();
    }

    public void StopPulsing()
    {
        _isPulsing=false;
    }

    private void PulseUp()
    {
        LeanTween.scale(_UIImage, _maxScale, _time)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(PulseBack);
    }

    private void PulseBack()
    {
        LeanTween.scale(_UIImage, Vector2.one, _time)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
        {
            if (_isPulsing)
                PulseUp();
        });
    }
}
