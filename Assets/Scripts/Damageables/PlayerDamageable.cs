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

	public void Hit()
	{
		if (_shouldDestroyOnHit)
		{
			Destroy(this.gameObject);
		}
	}

}
