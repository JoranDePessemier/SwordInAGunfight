using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer;

    private bool _canExit;
    private Controls _controls;

    public event EventHandler<EventArgs> ExitLevel;

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Player.ExitGame.performed += TryToExitLevel;
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if(Utilities.IsInLayerMask(collisionObject, _playerLayer))
        {
            _canExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (Utilities.IsInLayerMask(collisionObject, _playerLayer))
        {
            _canExit = false;
        }
    }

    private void TryToExitLevel(InputAction.CallbackContext obj)
    {
        if (_canExit)
        {
            OnExitLevel(EventArgs.Empty);
        }
    }

    private void OnExitLevel(EventArgs e)
    {
        var handler = ExitLevel;
        handler?.Invoke(this, e);
    }
}
