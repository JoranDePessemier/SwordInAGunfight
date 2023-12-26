using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static List<Type> Shuffle<Type>( List<Type> collection)
    {
        List<Type> collectionToBeShuffled = new List<Type>(collection);
        List<Type> returnCollection = new List<Type>();

        while (collectionToBeShuffled.Count > 0)
        {
            Type item = collection[UnityEngine.Random.Range(0, collection.Count)];
            returnCollection.Add(item);
            collectionToBeShuffled.Remove(item);
        }

        return returnCollection;    
    }
}
