using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]    
    private RectTransform _movingImage;

    [SerializeField]
    private Vector2 _topPosition;

    [SerializeField]
    private Vector2 _bottomPosition;

    [SerializeField]
    private float _moveInTime;

    [SerializeField]
    private float _moveOutTime;

    [SerializeField]
    private LeanTweenType _moveInType;

    [SerializeField]
    private LeanTweenType _moveOutType;

    private GameObject _movingImageGameObject;

    private void Awake()
    {
        _movingImageGameObject = _movingImage.gameObject;
        _movingImageGameObject.SetActive(true);
    }

    public void MoveIn(Action onComplete)
    {
        _movingImageGameObject.SetActive(true);
        _movingImage.localPosition = _topPosition;
        LeanTween.move(_movingImage, Vector3.zero, _moveInTime)
            .setEase(_moveInType)
            .setOnComplete(() => onComplete?.Invoke());
    }

    public void MoveIn() => MoveIn(null);

    public void MoveOut(Action onComplete)
    {
        _movingImageGameObject.SetActive(true);
        _movingImage.localPosition = Vector3.zero;
        LeanTween.move(_movingImage, _bottomPosition, _moveOutTime)
            .setEase(_moveOutType)
            .setOnComplete(() => { onComplete?.Invoke(); _movingImage.gameObject.SetActive(false); });
    }

    public void MoveOut() => MoveOut(null);
}
