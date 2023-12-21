using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PickupAmountEventArgs : EventArgs
{
    public PickupAmountEventArgs(int amount)
    {
        Amount = amount;
    }

    public int Amount { get; set; }
}

public class PlayerPickup : MonoBehaviour
{
    private Transform _transform;

    [SerializeField]
    private float _pickupRadius;

    [SerializeField]
    private LayerMask _pickupLayer;

    public event EventHandler<PickupAmountEventArgs> PickUpPoints;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }

    private void Update()
    {
        Collider2D pickupCollider = Physics2D.OverlapCircle(_transform.position, _pickupRadius,_pickupLayer);

        if (pickupCollider == null)
            return;

        if(!pickupCollider.TryGetComponent<PickupBase>(out PickupBase pickup))
        {
            Debug.LogWarning($"{pickupCollider.gameObject} is on the PickupLayer but does not have a pickup script attached");
            return;
        }

        if (pickup.StartedPickup)
            return;


        pickup.StartPickingUp(_transform,() => OnPickedUp(pickup));
    }

    private void OnPickedUp(PickupBase pickup)
    {
        Type pickupType = pickup.GetType();

        if(pickupType == typeof(PickupPoints))
        {
            PickupPoints points = pickup as PickupPoints;
            OnPickupPoints(new PickupAmountEventArgs(points.PickupAmount));
        }
    }

    private void OnPickupPoints(PickupAmountEventArgs e)
    {
        var handler = PickUpPoints;
        handler?.Invoke(this, e);   
    }
}
