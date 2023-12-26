using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AstarRefresher : MonoBehaviour
{
    AstarPath _path;

    private void Awake()
    {
        _path = this.GetComponent<AstarPath>();
    }
    private void Update()
    {
        _path.Scan(_path.graphs[1]);
    }
}
