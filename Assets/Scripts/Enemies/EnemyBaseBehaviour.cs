using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EnemyState
{
    Standing,
    RunTowards,
    KnockBack,
    RunAway
}

public class EnemyBaseBehaviour : MonoBehaviour
{
    private Transform _transform;

    public Transform Target { get; set; }

    private AIPath _path;

    [Header("Target Transforms")]
    [SerializeField]
    private Transform _inverseTarget;

    [SerializeField]
    private Transform _inverseCloseTarget;

    [SerializeField]
    private Transform _inverseTargetRotator;

    [Header("Run Away")]
    [SerializeField]
    private Vector2 _minMaxRunAwayDistance;

    [SerializeField]
    private Vector2 _minMaxRunAwaySpeed;

    [SerializeField]
    private Vector2 _minMaxRunAwayTime;

    [Header("Run Towards")]
    [SerializeField]
    private float _runTowardsDistance;

    [SerializeField]
    private Vector2 _minMaxRunTowardsSpeed;

    [Header("KnockBack")]
    [SerializeField]
    private float _knockBackSpeed;

    [SerializeField]
    private float _knockBackTime;

    private Transform _destination;

    private float _runAwayDistance;

    private EnemyState _state;

    private EnemyState State 
    { 
        get 
        {
            return _state; 
        }
        set
        {
            if (_state == value)
                return;

            _state = value;

            switch (value)
            {
                case EnemyState.Standing:
                    StartStanding();
                    break;
                case EnemyState.RunTowards:
                    StartRunTowards(); 
                    break;
                case EnemyState.KnockBack:
                    StartKnockBack();
                    break;
                case EnemyState.RunAway:
                    StartRunAway();
                    break;
            }
        }
    }

    private void Awake()
    {
        _transform = this.GetComponent<Transform>();
        _path = this.GetComponent<AIPath>();
        _runAwayDistance = Random.Range(_minMaxRunAwayDistance.x, _minMaxRunAwayDistance.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _minMaxRunAwayDistance.x);
        Gizmos.DrawWireSphere(transform.position, _minMaxRunAwayDistance.y);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _runTowardsDistance);
    }

    private void Update()
    {
        _inverseTargetRotator.LookAt(Target);

        if(_destination != null)
        {
            _path.destination = _destination.position;
        }


        switch (State)
        {
            case EnemyState.Standing:
                State = GetStateBasedOnDistance();
                break;

            case EnemyState.RunTowards:
                State = GetStateBasedOnDistance();
                break;
        }
    }

    private EnemyState GetStateBasedOnDistance()
    {
        float distanceFromTarget = Vector2.Distance(Target.position, _transform.position);

        if(distanceFromTarget >= _runTowardsDistance)
        {
            return EnemyState.Standing;
        }
        else if(distanceFromTarget >= _runAwayDistance)
        {
            return EnemyState.RunTowards;
        }
        else
        {
            return EnemyState.RunAway;
        }
    }

    private void StartStanding()
    {
        _path.maxSpeed = 0;
    }

    private void StartRunTowards()
    {
        _destination = Target;
        _path.maxSpeed = Random.Range(_minMaxRunTowardsSpeed.x, _minMaxRunTowardsSpeed.y);
    }

    private void StartRunAway()
    {
        _destination = _inverseTarget;
        _path.maxSpeed = Random.Range(_minMaxRunAwaySpeed.x,_minMaxRunAwaySpeed.y);
        StartCoroutine(Utilities.WaitForTime(Random.Range(_minMaxRunAwayTime.x,_minMaxRunAwayTime.y), () => { _state = EnemyState.Standing; }));
    }

    private void StartKnockBack()
    {
        _destination = _inverseCloseTarget;
        _path.maxSpeed = _knockBackSpeed;
        StartCoroutine(Utilities.WaitForTime(_knockBackTime, () => { _state = EnemyState.Standing; }));
    }


}
