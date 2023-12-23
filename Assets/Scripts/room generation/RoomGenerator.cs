using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [SerializeField]
    private Room _baseRoom;

    [SerializeField]
    private float _moveAmount;

    [SerializeField]
    private int _amountToGenerate = 20;

    [SerializeField]
    private Tilemap _tilesCombinedMap;

    [SerializeField]
    private Tilemap _groundCombinedMap;

    [SerializeField]
    private TileBase _wallTile;

    [SerializeField]
    private int _boundaryWallBuffer;

    private Vector2 _currentRoomPosition;

    private Vector2 _previousRoomPosition;

    private Direction _lastDirection;

    private Dictionary<Vector2, Room> _generatedRooms = new Dictionary<Vector2, Room>();

    private void Awake()
    {
        _transform = transform;
        RoomCoordinateUtilities.DistanceBetweenRooms = _moveAmount;

        SpawnRoom();

        StartCoroutine(GenerateRooms());
    }

    private IEnumerator GenerateRooms()
    {
        while(_generatedRooms.Count < _amountToGenerate)
        {
            yield return new WaitForSeconds(0.1f);
            MoveRoomPosition();
            SpawnRoom();
        }
        CombineRoomTiles();
        SetBoundaryWalls();
    }

    private void SetBoundaryWalls()
    {
        foreach (Room room in _generatedRooms.Values)
        {
            Dictionary<Vector3, TileBase> tiles = room.GetGroundTiles();

            foreach (KeyValuePair<Vector3, TileBase> tile in tiles)
            {
                _groundCombinedMap.SetTile(_groundCombinedMap.WorldToCell(tile.Key), tile.Value);
            }

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
        foreach(Room room in _generatedRooms.Values)
        {
            Dictionary<Vector3,TileBase> tiles = room.GetRoomTiles();

            foreach (KeyValuePair<Vector3,TileBase> tile in tiles)
            {
                _tilesCombinedMap.SetTile(_tilesCombinedMap.WorldToCell(tile.Key), tile.Value);
            }

            Destroy(room.TileMap.gameObject);
        }
    }

    private void SpawnRoom()
    {
        if (_generatedRooms.ContainsKey(_currentRoomPosition))
        {
            return;
        }

        Vector2 spawnPosition = RoomCoordinateUtilities.RoomToWorldCoordinate(_currentRoomPosition);

        Room newGeneratedRoom = GameObject.Instantiate(_baseRoom.gameObject,spawnPosition, Quaternion.identity).GetComponent<Room>();
        _generatedRooms.Add(_currentRoomPosition, newGeneratedRoom);

        if(_generatedRooms.Count > 1 && _generatedRooms.TryGetValue(_previousRoomPosition,out Room previousRoom))
        {
            SetExits(previousRoom, newGeneratedRoom);
        }
    }

    private void SetExits(Room previousRoom, Room newRoom)
    {
        switch(_lastDirection)
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
