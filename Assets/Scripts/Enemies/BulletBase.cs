using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private Rigidbody2D _body;
    private bool _hasFired;
    private float _speed;
    private Vector2 _direction;

    private Collider2D _collider;   

    [SerializeField]
    private LayerMask _destroyMask;

    [SerializeField]
    private ParticleSystem[] _particleSystems;

    [SerializeField]
    private GameObject _visual;

    private void Awake()
    {
        _body  = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
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
            _visual.SetActive(false);
            _collider.enabled = false;

            foreach(ParticleSystem particle in _particleSystems)
            {
                particle.Stop();
                StartCoroutine(DestroyOnParticleStop());
            }
        }
    }

    private IEnumerator DestroyOnParticleStop()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            int stoppedAmount = 0;

            foreach(ParticleSystem particle in _particleSystems)
            {
                if(particle.particleCount <= 0)
                {
                    stoppedAmount++;
                }
            }

            if(stoppedAmount >= _particleSystems.Length)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
