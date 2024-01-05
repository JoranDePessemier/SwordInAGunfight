using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CursorBehaviour : MonoBehaviour
{
    private Transform _transform;
    private GameObject _gameObject;

    private Controls _controls;

    [SerializeField]
    private UnityEvent _clicked;

    private void Awake()
    {
        _transform = this.transform;
        _gameObject = this.gameObject;
        _controls = new Controls();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Player.Attack.performed += CursorClicked;
    }

    private void OnDisable()
    {
        _controls.Disable();
        _controls.Player.Attack.performed -= CursorClicked;
    }

    private void CursorClicked(InputAction.CallbackContext obj)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _clicked.Invoke();
    }

    private void Update()
    {
        _transform.position = _controls.Player.MousePosition.ReadValue<Vector2>();
    }

    public void Spin(float time)
    {
        LeanTween.rotateAround(_gameObject, Vector3.forward, -360, time)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnComplete(() => _transform.rotation = Quaternion.identity);
    }
}
