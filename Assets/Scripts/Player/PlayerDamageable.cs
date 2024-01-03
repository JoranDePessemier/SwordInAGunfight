using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : MonoBehaviour
{
	[SerializeField]
	private int _damageToDo;

	public int DamageToDo
	{
		get { return _damageToDo; }
		set { _damageToDo = value; }
	}

	[SerializeField]
	private bool _shouldDestroyOnHit;

	[SerializeField]
	private LayerMask _playerMask;

	public void Hit()
	{
		if (_shouldDestroyOnHit)
		{
			Destroy(this.gameObject);
		}
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
		GameObject collisionObject = collision.gameObject;

		if(Utilities.IsInLayerMask(collisionObject,_playerMask))
		{
			Hit();
			collisionObject.GetComponent<PlayerHealth>().HitPlayer(_damageToDo);
		}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (Utilities.IsInLayerMask(collisionObject, _playerMask))
        {
            Hit();
            collisionObject.GetComponent<PlayerHealth>().HitPlayer(_damageToDo);
        }
    }

}
