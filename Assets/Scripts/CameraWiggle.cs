using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraWiggle:MonoBehaviour
{
    private GameObject _gameObject;
    private Transform _transform;

    private Vector3 _initialPosition;

    [Header("Wingle Parameters")]
    [SerializeField] private int _amountOfWiggles = 20;
    [SerializeField] private Vector3 _wiggleCenterPositionOffset;
    [SerializeField] private float _wiggleDistanceFromOriginal = 0.1f;
    [SerializeField] private float _timePerWiggle = 0.03f;

    private int _currentAmountOfWiggles;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _wiggleCenterPositionOffset, _wiggleDistanceFromOriginal);
    }

    private void Awake()
    {
        _gameObject = this.gameObject;
        _transform = transform;

        _initialPosition = _transform.localPosition;
    }

    public void StartWiggle()
    {
        if(_currentAmountOfWiggles <= 0)
        {
            MoveToNewWigglePosition();
        }
    }

    private void MoveToNewWigglePosition()
    {
        Vector3 newPosition = _initialPosition + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * _wiggleDistanceFromOriginal;
        MoveToPosition(newPosition, MoveToWigglePositionComplete);
    }

    private void MoveToWigglePositionComplete()
    {
        _currentAmountOfWiggles++;

        if (_currentAmountOfWiggles >= _amountOfWiggles)
        {
            MoveToPosition(_initialPosition,null);
            _currentAmountOfWiggles = 0;
        }
        else
            MoveToNewWigglePosition();
    }

    private void MoveToPosition(Vector3 position, Action OnComplete)
    {
        LeanTween.moveLocal(_gameObject, position, _timePerWiggle)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() => OnComplete?.Invoke());
    }
}
