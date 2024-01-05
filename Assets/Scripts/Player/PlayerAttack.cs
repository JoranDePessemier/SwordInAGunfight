using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField]
    private LayerMask _bulletMask;

    [SerializeField]
    private LayerMask _enemyMask;

    [SerializeField]
    private BulletBase _playerBullet;

    [SerializeField]
    private float _playerBulletSpeed;

    [SerializeField]
    private Slider _attackSlider;

    private Controls _controls;

    private bool _canAttack = true;

    private Vector2 _mouseWorldPosition;
    [SerializeField]
    private CursorBehaviour _cursorController;

    private void Awake()
    {
        _controls = new Controls();
        _controls.Player.Attack.performed += (InputAction.CallbackContext obj) => { if (_canAttack) { StartAttack(); } };
        _attackCollider = GetComponent<Collider2D>();

        _attackCollider.enabled = false;
        _transform = GetComponent<Transform>();

        _attackSlider.gameObject.SetActive(false);
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
        _mouseWorldPosition = _mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10));

        if(_canAttack )
        {
            _transform.right = (_mouseWorldPosition - (Vector2)_transform.position).normalized;
        }
    }

    private void StartAttack()
    {
        _canAttack = false;
        _attackCollider.enabled = true;

        _attack.Invoke();

        StartCoroutine(WaitOutAttackTime());
        _cursorController?.Spin(_attackTime + _timeBetweenAttacks);

    }

    private IEnumerator WaitOutAttackTime()
    {
        float timer = 0f;

        while(timer <= _attackTime)
        {
            timer+= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _attackCollider.enabled = false;
        _attackSlider.gameObject.SetActive(true);

        timer = 0f;

        while (timer <= _timeBetweenAttacks)
        {
            _attackSlider.value = timer/_timeBetweenAttacks;
            timer+= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _attackSlider.gameObject.SetActive(false);
        _canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (Utilities.IsInLayerMask(collisionObject,_enemyMask))
        {
            collisionObject.GetComponent<EnemyHealth>().Hit();
        }

        if(Utilities.IsInLayerMask(collisionObject,_bulletMask))
        {
            Transform bulletTransform = collisionObject.transform;
            BulletBase newBullet = Instantiate(_playerBullet, bulletTransform.position, Quaternion.identity);

            Vector2 direction = (_mouseWorldPosition - (Vector2)bulletTransform.position).normalized;

            newBullet.Fire(direction, _playerBulletSpeed);

            Destroy(collisionObject.gameObject);

        }

    }
}
