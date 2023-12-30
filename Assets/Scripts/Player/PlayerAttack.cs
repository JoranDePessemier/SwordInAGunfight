using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Collider2D _attackCollider;
    private Camera _mainCam;
    private Transform _transform;

    [SerializeField]
    private float _timeBetweenAttacks;

    [SerializeField]
    private float _attackTime;

    [SerializeField]
    private UnityEvent _attack;

    private Controls _controls;

    private bool _canAttack = true;

    private void Awake()
    {
        _controls = new Controls();
        _controls.Player.Attack.performed += (InputAction.CallbackContext obj) => { if (_canAttack) { StartAttack(); } };
        _attackCollider = GetComponent<Collider2D>();

        _attackCollider.enabled = false;
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _mainCam = Camera.main;
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
        Vector2 mouseScreenPosition = _controls.Player.MousePosition.ReadValue<Vector2>();
        Vector2 mouseWorldPosition = _mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10));

        _transform.right = (mouseWorldPosition - (Vector2) _transform.position).normalized;
        
    }

    private void StartAttack()
    {
        _canAttack = false;
        _attackCollider.enabled = true;

        _attack.Invoke();

        StartCoroutine(Utilities.WaitForTime(_timeBetweenAttacks,() => _canAttack = true));
        StartCoroutine(Utilities.WaitForTime(_attackTime, () => _attackCollider.enabled = false));

    }
}
