using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer;

    [SerializeField]
    private RectTransform _exitMenuTransform;

    private bool _canExit;
    private Controls _controls;

    public event EventHandler<EventArgs> ExitLevel;

    private bool _menuMoving;

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
            MenuUtilities.MoveIn(_exitMenuTransform,1f,LeanTweenType.easeOutQuint,null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (Utilities.IsInLayerMask(collisionObject, _playerLayer))
        {
            _canExit = false;
            MenuUtilities.MoveOutDown(_exitMenuTransform,1f,LeanTweenType.easeInQuint ,null);
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
