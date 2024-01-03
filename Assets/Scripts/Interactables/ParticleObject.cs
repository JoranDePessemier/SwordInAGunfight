using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    private ParticleSystem _system;
    private void Awake()
    {
        _system = this.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!_system.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
