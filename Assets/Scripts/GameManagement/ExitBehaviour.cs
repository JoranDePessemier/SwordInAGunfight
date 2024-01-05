using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExitBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer;

    [SerializeField]
    private RectTransform _exitMenuTransform;

    [SerializeField]
    private RectTransform _exitPointerRotator;

    [SerializeField]
    private RectTransform _exitPointerImage;

    private bool _canExit;
    private Controls _controls;

    public event EventHandler<EventArgs> ExitLevel;

    private Transform _player;

    private bool _pointerShouldAppear;
    private Transform _transform;
    private float _exitPointerAlpha;

    private void Awake()
    {
        _controls = new Controls();
        _exitPointerAlpha = _exitPointerImage.GetComponent<Image>().color.a;
        LeanTween.alpha(_exitPointerImage, 0, 0f);
        _transform = this.transform;
    }

    private void Start()
    {
        _player = FindObjectOfType<PlayerMovement>().transform;
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

            if (_pointerShouldAppear)
            {
                LeanTween.alpha(_exitPointerImage, 0, 0.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (Utilities.IsInLayerMask(collisionObject, _playerLayer))
        {
            _canExit = false;
            MenuUtilities.MoveOutDown(_exitMenuTransform,1f,LeanTweenType.easeInQuint ,null);
            if (_pointerShouldAppear)
            {
                LeanTween.alpha(_exitPointerImage, _exitPointerAlpha, 0.5f);
            }

        }
    }

    public void AppearPointer()
    {
        _pointerShouldAppear = true;
        if (!_canExit)
        {
            LeanTween.alpha(_exitPointerImage, _exitPointerAlpha, 1f);
        }

    }

    private void TryToExitLevel(InputAction.CallbackContext obj)
    {
        if (_canExit)
        {
            OnExitLevel(EventArgs.Empty);
        }
    }

    private void Update()
    {
        _exitPointerRotator.up = _transform.position - _player.position;
    }

    private void OnExitLevel(EventArgs e)
    {
        var handler = ExitLevel;
        handler?.Invoke(this, e);
    }
}
