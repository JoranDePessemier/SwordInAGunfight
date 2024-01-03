using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPoints : PickupBase
{
	[SerializeField]
	private int _pickupAmount;

	public int PickupAmount
	{
		get { return _pickupAmount; }
		private set { _pickupAmount = value; }
	}

}
