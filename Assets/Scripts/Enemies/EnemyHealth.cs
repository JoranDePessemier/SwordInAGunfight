using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int _startingHealth = 1;

    [SerializeField]
    private GeneratingItem[] _pickupsToDrop;

    [SerializeField]
    private float _pickupDropRadius;

    [SerializeField]
    private float _pickupDropMovementTime;

    [SerializeField]
    private LeanTweenType _pickupDropMovementType;

    [SerializeField]
    private LayerMask _terrainLayer;


    private int _health;
    private Transform _transform;

    private void Awake()
    {
        _health = _startingHealth;
        _transform = transform;
    }

    public void Hit()
    {
        _health--;

        if(_health <= 0)
        {
            Kill();
        }
    }

    private void Kill()
    {
        foreach(GeneratingItem item in _pickupsToDrop)
        {
            for (int i = 0; i < item.AmountToGenerate; i++)
            {
                PickupBase pickup = Instantiate(item.Item, _transform.position,Quaternion.identity).GetComponent<PickupBase>();
                pickup.enabled = false;

                Vector2 randomDropPoint = (UnityEngine.Random.insideUnitCircle.normalized * _pickupDropRadius) + (Vector2)_transform.position;
                
                while (Physics2D.Raycast(_transform.position,(randomDropPoint - (Vector2)_transform.position).normalized, Vector2.Distance(randomDropPoint, (Vector2)_transform.position), _terrainLayer))
                {
                    randomDropPoint = (UnityEngine.Random.insideUnitCircle.normalized * _pickupDropRadius) + (Vector2)_transform.position;
                }

                LeanTween.move(pickup.gameObject, randomDropPoint, _pickupDropMovementTime)
                    .setEase(_pickupDropMovementType)
                    .setOnComplete(() =>
                    {
                        pickup.enabled = true;
                    });
            }
        }

        Destroy(this.gameObject);
    }
}
