using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Controls _controls;
    private Rigidbody2D _body;

    private Vector2 _velocity;
    private Vector2 _inputVector;

    [SerializeField]
    private float _acceleration;

    [SerializeField]
    private float _drag;

    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private GameObject _playerVisual;

    private SpriteRenderer _playerSprite;
    private Animator _playerAnimator;

    private void Awake()
    {
        _controls = new Controls();
        _body = GetComponent<Rigidbody2D>();

        if(_playerVisual != null)
        {
            _playerAnimator = _playerVisual.GetComponent<Animator>();
            _playerSprite = _playerVisual.GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();    
    }

    private void Update()
    {
        _inputVector = _controls.Player.Movement.ReadValue<Vector2>();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_inputVector.SqrMagnitude() <= 0)
        {
            _playerAnimator?.SetBool("IsRunning", false);
        }
        else
        {
            _playerAnimator?.SetBool("IsRunning", true);

            if (_inputVector.x != 0)
            {
                if (_inputVector.x < 0)
                {
                    _playerSprite.flipX = true;
                }
                else
                {
                    _playerSprite.flipX = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyDrag();
        ApplySpeedLimit();

        _body.MovePosition(_body.position + _velocity * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        _velocity += _inputVector * _acceleration;
    }

    private void ApplyDrag()
    {
        _velocity *= (1 - _drag * Time.deltaTime);
    }

    private void ApplySpeedLimit()
    {
        _velocity = Vector2.ClampMagnitude(_velocity,_maxSpeed);
    }
}
