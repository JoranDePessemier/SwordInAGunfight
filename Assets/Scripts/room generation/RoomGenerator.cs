using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RoomGeneration
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public class RoomGenerator : MonoBehaviour
    {
        private Transform _transform;

        [Header("Room settings")]
        [SerializeField]
        private Room _startingRoom;

        [SerializeField]
        private Room[] _roomPrefabs;

        [SerializeField]
        private float _moveAmount;

        [SerializeField]
        private int _amountToGenerate = 20;

        [Header("TileMaps")]
        [SerializeField]
        private Tilemap _tilesCombinedMap;

        [SerializeField]
        private Tilemap _groundCombinedMap;

        [Header("Boundary values")]
        [SerializeField]
        private TileBase _wallTile;

        [SerializeField]
        private int _boundaryWallBuffer;

        [Header("Item prefabs")]
        [SerializeField]
        private GameObject _exitPrefab;

        [SerializeField]
        private GeneratingItem[] _enemyPrefabs;

        [SerializeField]
        private GeneratingItem[] _pickupPrefabs;

        private Vector2Int _currentRoomPosition;

        private Vector2Int _previousRoomPosition;

        private Direction _lastDirection;

        private Dictionary<Vector2Int, Room> _generatedRooms = new Dictionary<Vector2Int, Room>();

        private void Awake()
        {
            _transform = transform;
            RoomCoordinateUtilities.DistanceBetweenRooms = _moveAmount;

            SpawnRoom(_startingRoom);

            while (_generatedRooms.Count < _amountToGenerate)
            {
                MoveRoomPosition();
                SpawnRoom(_roomPrefabs[Random.Range(0, _roomPrefabs.Length)]);
            }
            CombineRoomTiles();
            SetBoundaryWalls();
            SetAstar();

            GenerateLevelExit();
            SpawnEnemies();
            SpawnPickups();
            DeleteRoomTemplates();
        }

        private void DeleteRoomTemplates()
        {
            foreach(KeyValuePair<Vector2Int,Room> room in _generatedRooms)
            {
                GameObject.Destroy(room.Value.gameObject);
            }
        }

        private void SpawnEnemies()
        {
            List<Room> availableEnemyRooms = _generatedRooms.Values.ToList<Room>();
            availableEnemyRooms.RemoveAt(0);

            foreach(GeneratingItem item in _enemyPrefabs)
            {
                int amountGenerated = 0;

                while(amountGenerated <= item.AmountToGenerate && availableEnemyRooms.Count > 0)
                {
                    Room spawnRoom = availableEnemyRooms[Random.Range(0, availableEnemyRooms.Count)];
                    
                    if(spawnRoom.SpawnEnemy(item.Item) != null)
                    {
                        amountGenerated++;
                    }
                    else
                    {
                        availableEnemyRooms.Remove(spawnRoom);
                    }
                }
            }
        }

        private void SpawnPickups()
        {
            List<Room> availablePickupRooms = _generatedRooms.Values.ToList<Room>();
            availablePickupRooms.RemoveAt(0);

            foreach (GeneratingItem item in _pickupPrefabs)
            {
                int amountGenerated = 0;

                while (amountGenerated <= item.AmountToGenerate && availablePickupRooms.Count > 0)
                {
                    Room spawnRoom = availablePickupRooms[Random.Range(0, availablePickupRooms.Count)];

                    if (spawnRoom.SpawnPickup(item.Item) != null)
                    {
                        amountGenerated++;
                    }
                    else
                    {
                        availablePickupRooms.Remove(spawnRoom);
                    }
                }
            }
        }

        private void GenerateLevelExit()
        {
            Room furthestRoom = new Room();
            int furthestRoomCoordinate = 0;

            foreach(KeyValuePair<Vector2Int,Room> room in _generatedRooms)
            {
                if(Mathf.Abs(room.Key.x) >= furthestRoomCoordinate)
                {
                    furthestRoomCoordinate = Mathf.Abs(room.Key.x);
                    furthestRoom = room.Value;
                }
                if(Mathf.Abs(room.Key.y) >= furthestRoomCoordinate)
                {
                    furthestRoomCoordinate = Mathf.Abs(room.Key.y);
                    furthestRoom = room.Value;
                }
            }

            furthestRoom.SpawnLevelExit(_exitPrefab);

        }

        private void SetAstar()
        {
            var graph = AstarPath.active.data.gridGraph;
            graph.center = new Vector3(_groundCombinedMap.cellBounds.center.x + _boundaryWallBuffer / 2, _groundCombinedMap.cellBounds.center.y + _boundaryWallBuffer / 2);
            graph.SetDimensions((int)(_groundCombinedMap.cellBounds.size.x / graph.nodeSize) + _boundaryWallBuffer, (int)(_groundCombinedMap.size.y / graph.nodeSize) + _boundaryWallBuffer, graph.nodeSize);

            AstarPath.active.Scan();
        }

        private void SetBoundaryWalls()
        {
            foreach (Room room in _generatedRooms.Values)
            {
                Dictionary<Vector3, TileBase> tiles = room.GetGroundTiles();

                Utilities.AddTilesToTileMap(ref _groundCombinedMap, tiles);

                Destroy(room.GroundMap.gameObject);
            }

            for (int x = _groundCombinedMap.cellBounds.min.x - _boundaryWallBuffer; x < _groundCombinedMap.cellBounds.max.x + _boundaryWallBuffer; x++)
            {
                for (int y = _groundCombinedMap.cellBounds.min.y - _boundaryWallBuffer; y < _groundCombinedMap.cellBounds.max.y + _boundaryWallBuffer; y++)
                {
                    TileBase tile = _groundCombinedMap.GetTile<TileBase>(new Vector3Int(x, y));

                    if (tile == null)
                    {
                        _tilesCombinedMap.SetTile(new Vector3Int(x, y), _wallTile);
                        Destroy(_tilesCombinedMap.GetInstantiatedObject(new Vector3Int(x, y)));
                    }
                }
            }
        }

        private void CombineRoomTiles()
        {
            foreach (Room room in _generatedRooms.Values)
            {
                Dictionary<Vector3, TileBase> tiles = room.GetRoomTiles();

                Utilities.AddTilesToTileMap(ref _tilesCombinedMap, tiles);

                Destroy(room.TileMap.gameObject);
            }
        }

        private void SpawnRoom(Room roomToSpawn)
        {
            if (_generatedRooms.ContainsKey(_currentRoomPosition))
            {
                return;
            }

            Vector2 spawnPosition = RoomCoordinateUtilities.RoomToWorldCoordinate(_currentRoomPosition);

            Room newGeneratedRoom = GameObject.Instantiate(roomToSpawn.gameObject, spawnPosition, Quaternion.identity).GetComponent<Room>();
            _generatedRooms.Add(_currentRoomPosition, newGeneratedRoom);

            if (_generatedRooms.Count > 1 && _generatedRooms.TryGetValue(_previousRoomPosition, out Room previousRoom))
            {
                SetExits(previousRoom, newGeneratedRoom);
            }
        }

        private void SetExits(Room previousRoom, Room newRoom)
        {
            switch (_lastDirection)
            {
                case Direction.Right:

                    previousRoom.RemoveRightTiles();
                    newRoom.RemoveLeftTiles();

                    break;

                case Direction.Left:

                    previousRoom.RemoveLeftTiles();
                    newRoom.RemoveRightTiles();

                    break;

                case Direction.Up:

                    previousRoom.RemoveTopTiles();
                    newRoom.RemoveBottomTiles();

                    break;

                case Direction.Down:

                    previousRoom.RemoveBottomTiles();
                    newRoom.RemoveTopTiles();

                    break;
            }
        }

        private void MoveRoomPosition()
        {
            _previousRoomPosition = _currentRoomPosition;
            _lastDirection = (Direction)Random.Range(0, 4);
            switch (_lastDirection)
            {
                case Direction.Right:

                    _currentRoomPosition.x += 1;

                    break;

                case Direction.Left:

                    _currentRoomPosition.x -= 1;

                    break;

                case Direction.Up:

                    _currentRoomPosition.y += 1;

                    break;


                case Direction.Down:

                    _currentRoomPosition.y -= 1;

                    break;
            }

        }
    }

}

