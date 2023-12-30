using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private Rigidbody2D _body;
    private bool _hasFired;
    private float _speed;
    private Vector2 _direction;

    [SerializeField]
    private LayerMask _destroyMask;

    private void Awake()
    {
        _body  = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 direction, float speed)
    {
        _hasFired = true;
        _speed = speed;
        _direction = direction;
    }

    private void FixedUpdate()
    {
        if(_hasFired)
        {
            _body.MovePosition(_body.position + _speed * _direction * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject _collisionObject = collision.gameObject;



        if (Utilities.IsInLayerMask(_collisionObject, _destroyMask))
        {
            Destroy(this.gameObject);
        }
    }

}
