using RandomWalk;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RandomWalk
{
    public static class StepCoordinateUtilities
    {
        public static List<Vector2Int> GetTileCoordinatesInStep(Vector2Int step, Vector2Int stepAmount)
        {
            List<Vector2Int> tileCoords = new List<Vector2Int>();

            Vector2Int bottomLeftTileCoords = step * stepAmount;

            for (int x = bottomLeftTileCoords.x; x < bottomLeftTileCoords.x + stepAmount.x; x++)
            {
                for (int y = bottomLeftTileCoords.y; y < bottomLeftTileCoords.y + stepAmount.y; y++)
                {
                    tileCoords.Add(new Vector2Int(x, y));
                }
            }

            return tileCoords;
        }

        public static Dictionary<Direction,TileBase> GetTilesPerDirection(Vector2Int tileCoordinates, Tilemap tileMap)
        {
            Dictionary<Direction, TileBase> returnDictionary = new Dictionary<Direction, TileBase>();
            TileBase addingTile = null;

            addingTile = tileMap.GetTile(new Vector3Int(tileCoordinates.x, tileCoordinates.y + 1));
            if(addingTile != null)
            {
                returnDictionary.Add(Direction.Up, addingTile);
            }

            addingTile = tileMap.GetTile(new Vector3Int(tileCoordinates.x, tileCoordinates.y - 1));
            if (addingTile != null)
            {
                returnDictionary.Add(Direction.Down, addingTile);
            }
            
            addingTile = tileMap.GetTile(new Vector3Int(tileCoordinates.x + 1, tileCoordinates.y));
            if (addingTile != null)
            {
                returnDictionary.Add(Direction.Right, addingTile);
            }

            addingTile = tileMap.GetTile(new Vector3Int(tileCoordinates.x - 1, tileCoordinates.y));
            if (addingTile != null)
            {
                returnDictionary.Add(Direction.Left, addingTile);
            }

            return returnDictionary;
        }
    }
}


