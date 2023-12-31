using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    [SerializeField]
    private bool _shouldDestroyOnHit;

    [SerializeField]
    private LayerMask _enemyMask;

    public void Hit()
    {
        if (_shouldDestroyOnHit)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (Utilities.IsInLayerMask(collisionObject, _enemyMask))
        {
            Hit();
            collisionObject.GetComponent<EnemyHealth>().Hit();
        }
    }
}
