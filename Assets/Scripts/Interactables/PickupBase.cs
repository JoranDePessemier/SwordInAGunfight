using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBase : MonoBehaviour
{
    private Transform _transform;
    private Collider2D _collider;

    public bool StartedPickup { get; private set; }

    [SerializeField]
    private float _startingMovementSpeed;

    [SerializeField]
    private float _movementAcceleration;

    private float _movementSpeed;

    private void Awake()
    {
        _transform = this.GetComponent<Transform>();
        _collider = this.GetComponent<Collider2D>();
        _movementSpeed = _startingMovementSpeed;
    }


    internal void StartPickingUp(Transform moveTransform,Action onComplete)
    {
        _collider.enabled = false;
        StartedPickup = true;
        StartCoroutine(MoveToPickupSpot(moveTransform, onComplete));
    }

    private IEnumerator MoveToPickupSpot(Transform moveTransform, Action onComplete)
    {
        while(Vector3.Distance(_transform.position,moveTransform.position) >= 0.15f)
        {
            _movementSpeed += _movementAcceleration * Time.fixedDeltaTime * 0.5f;
            _transform.position = Vector3.MoveTowards(_transform.position, moveTransform.position, _movementSpeed * Time.deltaTime);
            _movementSpeed += _movementAcceleration * Time.fixedDeltaTime * 0.5f;
            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
        onComplete?.Invoke();  
    }
}
