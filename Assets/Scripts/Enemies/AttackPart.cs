using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackPart
{
	[SerializeField]
	private BulletBase _projectile;
	public BulletBase Projectile => _projectile;

	[SerializeField]
	private float _timeAfterLastPart;
	public float TimeAfterLastPart => _timeAfterLastPart;

	[SerializeField]
	private float _projectileSpeed;
	public float ProjectileSpeed => _projectileSpeed;

	[Range(0, 359)]
	[SerializeField]
	private int _minAngle;

	[Range(0, 359)]
	[SerializeField]
	private int _maxAngle;
	public Vector2Int MinMaxAngle { get { return new Vector2Int(_minAngle, _maxAngle); } }

	[SerializeField]
	private bool _setBaseAngleTowardsPlayer;
	public bool SetBaseAngleTowardsPlayer => _setBaseAngleTowardsPlayer;

}
