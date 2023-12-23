using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{

    [Header("TileMaps")]
    [SerializeField]
    private Tilemap _tileMap;

    public Tilemap TileMap
    {
        get { return _tileMap; }
        set { _tileMap = value; }
    }

    [SerializeField]
    private Tilemap _groundMap;

    public Tilemap GroundMap
    {
        get { return _groundMap; }
        set { _groundMap = value; }
    }

    [Header("RoomExits")]
    [SerializeField]
    private Transform _leftExitparent;

    [SerializeField]
    private Transform _rightExitParent;

    [SerializeField]
    private Transform _bottomExitparent;

    [SerializeField]
    private Transform _topExitParent;

    private Transform[] _bottomTiles, _topTiles, _leftTiles, _rightTiles;

    [Header("Extra tiles")]
    [SerializeField]
    private Transform[] _tileTemplateParents;

    [SerializeField]
    private Transform _randomTileParent;

    [Range(0f, 1f)] 
    [SerializeField]
    private float _randomTileChance;

    private void Awake()
    {
        _topTiles = _topExitParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();
        _bottomTiles = _bottomExitparent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();
        _rightTiles = _rightExitParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();
        _leftTiles = _leftExitparent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();

        SetTileTemplates();
        SetRandomTiles();
    }

    private void SetRandomTiles()
    {
        List<Transform> removeTiles = new List<Transform>();

        foreach (Transform tile in _randomTileParent.GetComponentsInChildren<Transform>().Skip<Transform>(1))
        {
            if(UnityEngine.Random.Range(0f,1f) >= _randomTileChance)
            {
                removeTiles.Add(tile);  
            }
        }

        RemoveTiles(removeTiles);
    }

    private void SetTileTemplates()
    {
        int chosenTemplateIndex = UnityEngine.Random.Range(0,_tileTemplateParents.Length);

        for (int i = 0; i < _tileTemplateParents.Length; i++)
        {
            if (i != chosenTemplateIndex)
            {
                RemoveTiles(_tileTemplateParents[i].GetComponentsInChildren<Transform>().Skip<Transform>(1));
            }
        }
    }

    private void RemoveTiles(IEnumerable tiles)
    {
        foreach (Transform transform in tiles)
        {
            Vector3Int cellPosition = TileMap.WorldToCell(transform.position);

            TileMap.SetTile(cellPosition, null);
            Destroy(transform.gameObject);
        }
    }

    public void RemoveBottomTiles() => RemoveTiles(_bottomTiles);

    public void RemoveTopTiles() => RemoveTiles(_topTiles);

    public void RemoveRightTiles() => RemoveTiles(_rightTiles);

    public void RemoveLeftTiles() => RemoveTiles(_leftTiles);

    public Dictionary<Vector3, TileBase> GetRoomTiles() => GetTiles(TileMap);

    public Dictionary<Vector3, TileBase> GetGroundTiles() => GetTiles(GroundMap);


    private Dictionary<Vector3, TileBase> GetTiles(Tilemap tileMap)
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

}
