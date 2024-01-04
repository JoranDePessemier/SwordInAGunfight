using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RandomWalk
{
    public enum Direction
    {
        Up,Down, Left, Right    
    }

    public class RandomWalkGenerator : MonoBehaviour
    {
        [Header("TileMaps")]
        [SerializeField]
        private Tilemap _wallMap;

        [SerializeField]
        private Tilemap _groundMap;

        [Header("Tiles")]
        [SerializeField]
        private TileBase _wallTile;

        [SerializeField]
        private TileBase _groundTile;

        [Header("TileValues")]
        [SerializeField]
        private Vector2Int _stepAmount = new Vector2Int(2,2);

        [SerializeField]
        private int _amountOfOpenTiles;

        [SerializeField]
        private int _mapBufferAmount = 10;

        [Header("Pickups")]
        [SerializeField]
        private GeneratingItem[] _pickupsToGenerate;

        [SerializeField]
        private int _maxPickupSpawnTries;

        [Header("Enemies")]
        [SerializeField]
        private GeneratingItem[] _enemiesToGenerate;

        [SerializeField]
        private int _maxEnemySpawnTries;

        [SerializeField]
        private float _spawnDistanceFromCenter;

        [Header("exit")]
        [SerializeField]
        private GameObject _exit;

        [Header("Spikes")]
        [SerializeField]
        private TileBase _spikeTile;

        [SerializeField]
        private int _spikePathAmount;

        [SerializeField]
        private int _spikesPerPath;

        private Vector2Int _currentStep;

        private Vector2Int _bottomLeftTileCoordinate;
        private Vector2Int _topRightTileCoordinate;

        private Dictionary<Vector2Int,TileBase> _openTiles = new Dictionary<Vector2Int, TileBase> ();
        private Dictionary<Vector2Int, GameObject> _generatedItems = new Dictionary<Vector2Int, GameObject>();

        private void Awake()
        {
            while(_openTiles.Count < _amountOfOpenTiles)
            {
                SetTilesOpen(_currentStep);
                _currentStep = MoveCoordsOneStep(_currentStep);
            }

            SpawnGroundTiles();
            SpawnWallTiles();
            SpawnSpikes();
            SetAstar();
            SpawnExit();
            SpawnPickups();
            SpawnEnemies();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3.zero, _spawnDistanceFromCenter);
        }

        private void SpawnWallTiles()
        {
            for (int x = _bottomLeftTileCoordinate.x - _mapBufferAmount; x <= _topRightTileCoordinate.x + _mapBufferAmount; x++)
            {
                for (int y = _bottomLeftTileCoordinate.y - _mapBufferAmount; y <= _topRightTileCoordinate.y + _mapBufferAmount; y++)
                {
                    Vector2Int coord = new Vector2Int(x, y);

                    if (!_openTiles.ContainsKey(coord))
                    {
                        _wallMap.SetTile((Vector3Int)coord, _wallTile);
                    }
                }
            }
        }

        private void SpawnGroundTiles()
        {
            foreach(Vector2Int tileCoord in _openTiles.Keys)
            {
                _groundMap.SetTile((Vector3Int)tileCoord, _groundTile);
            }
        }

        private void SetTilesOpen(Vector2Int step)
        {
            foreach(Vector2Int tileCoord in StepCoordinateUtilities.GetTileCoordinatesInStep(step,_stepAmount))
            {
                if (!_openTiles.ContainsKey(tileCoord))
                {
                    if(tileCoord.x < _bottomLeftTileCoordinate.x)
                    {
                        _bottomLeftTileCoordinate.x = tileCoord.x;
                    }
                    else if(tileCoord.x > _topRightTileCoordinate.x)
                    {
                        _topRightTileCoordinate.x = tileCoord.x;
                    }

                    if (tileCoord.y < _bottomLeftTileCoordinate.y)
                    {
                        _bottomLeftTileCoordinate.y = tileCoord.y;
                    }
                    else if (tileCoord.y > _topRightTileCoordinate.y)
                    {
                        _topRightTileCoordinate.y = tileCoord.y;
                    }

                    _openTiles.Add(tileCoord, _wallMap.GetTile((Vector3Int)tileCoord));
                }
            }
        }

        private void SpawnPickups()
        {
            foreach(GeneratingItem item in _pickupsToGenerate)
            {
                for (int i = 0; i < item.AmountToGenerate; i++)
                {
                    SpawnItem(item,2,_maxPickupSpawnTries,0);
                }
            }
        }

        private void SpawnEnemies()
        {
            foreach (GeneratingItem item in _enemiesToGenerate)
            {
                for (int i = 0; i < item.AmountToGenerate; i++)
                {
                    SpawnItem(item, 0, _maxEnemySpawnTries, _spawnDistanceFromCenter);
                }
            }
        }

        private void SpawnItem(GeneratingItem item, int surroundingTileAmount, int spawnTries, float spawnDistanceFromCenter)
        {
            KeyValuePair<Vector2Int, TileBase> randomSpawnTile = default;
            int tries = 0;

            do
            {
                do
                {
                    randomSpawnTile = PickRandomOpenTile();
                }
                while (_generatedItems.ContainsKey(randomSpawnTile.Key) || Vector2.Distance(_wallMap.CellToWorld((Vector3Int)randomSpawnTile.Key),Vector2.zero) <= _spawnDistanceFromCenter);
                tries++;
            }
            while (StepCoordinateUtilities.GetTilesPerDirection(randomSpawnTile.Key, _wallMap).Count != surroundingTileAmount && tries < spawnTries);

            GameObject generatedItem = Instantiate(item.Item, TileToWorldCenterPosition(randomSpawnTile.Key, _wallMap), Quaternion.identity);
            _generatedItems.Add(randomSpawnTile.Key, generatedItem);
        }

        private KeyValuePair<Vector2Int, TileBase> PickRandomOpenTile()
        {
            int index = Random.Range(0, _openTiles.Count);
            return _openTiles.ElementAt<KeyValuePair<Vector2Int,TileBase>>(index);
        }

        private Vector2 TileToWorldCenterPosition(Vector2Int tileCoordinates, Tilemap map)
        {
            Vector2 worldCoordinates =  map.CellToWorld((Vector3Int)tileCoordinates);
            worldCoordinates.x += map.cellSize.x / 2;
            worldCoordinates.y += map.cellSize.y / 2;

            return worldCoordinates;
        }

        private void SetAstar()
        {
            var graph = AstarPath.active.data.gridGraph;
            graph.center = graph.center + new Vector3(_wallMap.cellBounds.center.x, _wallMap.cellBounds.center.y);
            graph.SetDimensions((int)(_wallMap.cellBounds.size.x / graph.nodeSize), (int)(_wallMap.size.y / graph.nodeSize), graph.nodeSize);

            AstarPath.active.Scan();
        }

        private void SpawnExit()
        {
            Vector2Int randomCoords = Vector2Int.zero;

            if(Mathf.Abs(_topRightTileCoordinate.x) > Mathf.Abs(_bottomLeftTileCoordinate.x))
            {
                randomCoords.x = _topRightTileCoordinate.x;
            }
            else
            {
                randomCoords.x = _bottomLeftTileCoordinate.x;
            }

            if (Mathf.Abs(_topRightTileCoordinate.y) > Mathf.Abs(_bottomLeftTileCoordinate.y))
            {
                randomCoords.y = _topRightTileCoordinate.y;
            }
            else
            {
                randomCoords.y = _bottomLeftTileCoordinate.y;
            }

            while (!_openTiles.ContainsKey(randomCoords))
            {
                randomCoords = MoveCoordsOneStep(randomCoords);
                randomCoords = new Vector2Int(Mathf.Clamp(randomCoords.x,_bottomLeftTileCoordinate.x,_topRightTileCoordinate.x),Mathf.Clamp(randomCoords.y,_bottomLeftTileCoordinate.y,_topRightTileCoordinate.y));
            }

            GameObject exit = Instantiate(_exit, TileToWorldCenterPosition(randomCoords, _wallMap), Quaternion.identity);
            _generatedItems.Add(randomCoords, exit);
        }

        private void SpawnSpikes()
        {
            Dictionary<Vector2Int, TileBase> generatedSpikes = new Dictionary<Vector2Int, TileBase>();
            
            for (int i = 0; i < _spikePathAmount; i++)
            {
                Vector2Int currentCoords = new Vector2Int(Random.Range(_bottomLeftTileCoordinate.x, _topRightTileCoordinate.x), Random.Range(_bottomLeftTileCoordinate.y, _topRightTileCoordinate.y));
                int addedSpikes = 0;

                while (addedSpikes < _spikesPerPath)
                {
                    currentCoords = MoveCoordsOneStep(currentCoords);
                    currentCoords = new Vector2Int
                        (
                        Mathf.Clamp(currentCoords.x, _bottomLeftTileCoordinate.x - _mapBufferAmount + 1, _topRightTileCoordinate.x + _mapBufferAmount - 1),
                        Mathf.Clamp(currentCoords.y, _bottomLeftTileCoordinate.y - _mapBufferAmount + 1, _topRightTileCoordinate.y + _mapBufferAmount - 1)
                        );

                    if (!generatedSpikes.ContainsKey(currentCoords) && !_openTiles.ContainsKey(currentCoords))
                    {
                        _wallMap.SetTile((Vector3Int)currentCoords, _spikeTile);
                        generatedSpikes.Add(currentCoords, _wallMap.GetTile((Vector3Int)currentCoords));
                        addedSpikes++;
                    }
                }
            }

            foreach(KeyValuePair<Vector2Int,TileBase> spike in generatedSpikes)
            {
                if (StepCoordinateUtilities.GetTilesPerDirection(spike.Key,_wallMap).Count >= 4)
                {
                    _wallMap.SetTile((Vector3Int)spike.Key, _wallTile);
                }
            }
        }

        private Vector2Int MoveCoordsOneStep(Vector2Int coords)
        {
            Direction randomDirection = (Direction)Random.Range(0, 4);
            switch (randomDirection)
            {
                case Direction.Right:
                    coords.x += 1;
                    break;

                case Direction.Left:
                    coords.x -= 1;
                    break;

                case Direction.Up:
                    coords.y += 1;
                    break;

                case Direction.Down:
                    coords.y -= 1;
                    break;
            }

            return coords;
        }
    }
}