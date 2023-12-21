using System;
using System.Collections;
using UnityEngine;

public static class Utilities 
{
    public static bool IsInLayerMask(GameObject gameObject, LayerMask mask)
    {
        return ((mask & (1 << gameObject.layer)) != 0);
    }

    public static IEnumerator WaitForTime(float time, Action onComplete)
    {
        yield return new WaitForSeconds(time);
        onComplete?.Invoke();
    }
}
