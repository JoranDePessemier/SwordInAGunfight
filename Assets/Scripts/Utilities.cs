using RoomGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public static List<Type> Shuffle<Type>(List<Type> collection)
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

    public static Vector2 AddAngleToVector(Vector2 vector, float angle)
    {
        float angleRadians = Mathf.Deg2Rad * angle;
        float cos = Mathf.Cos(angleRadians);
        float sin = Mathf.Sin(angleRadians);

        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }

    public static Dictionary<Vector3, TileBase> GetTilesInMap(Tilemap tileMap)
    {
        Dictionary<Vector3, TileBase> returnTiles = new Dictionary<Vector3, TileBase>();

        for (int x = tileMap.cellBounds.min.x; x < tileMap.cellBounds.max.x; x++)
        {
            for (int y = tileMap.cellBounds.min.y; y < tileMap.cellBounds.max.y; y++)
            {
                TileBase tile = tileMap.GetTile<TileBase>(new Vector3Int(x, y));

                if (tile != null)
                {
                    returnTiles.Add(tileMap.CellToWorld(new Vector3Int(x, y)), tile);
                }
            }
        }

        return returnTiles;
    }

    public static void AddTilesToTileMap(ref Tilemap tileMap, Dictionary<Vector3, TileBase> tiles)
    {
        foreach (KeyValuePair<Vector3, TileBase> tile in tiles)
        {
            tileMap.SetTile(tileMap.WorldToCell(tile.Key), tile.Value);
        }
    }
}
