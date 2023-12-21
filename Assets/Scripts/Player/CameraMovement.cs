using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform _transform;

    [SerializeField]
    private Transform _followTransform;

    [SerializeField]
    private float _followSpeed;

    private void Awake()
    {
        _transform = transform;
    }

    private void FixedUpdate()
    {
        Vector3 followPosition = _followTransform.localPosition;
        followPosition.z = _transform.localPosition.z;

        _transform.localPosition = Vector3.Lerp(_transform.localPosition, followPosition, _followSpeed * Time.deltaTime);
    }
}
