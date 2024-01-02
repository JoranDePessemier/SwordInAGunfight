using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

namespace RoomGeneration
{
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
        private Transform[] _randomTileParents;

        private Dictionary<Vector3Int, TileBase> _tilesToKeep = new Dictionary<Vector3Int, TileBase>();

        private Dictionary<Vector3Int, TileBase> _tilesToRemove = new Dictionary<Vector3Int, TileBase>();

        [Range(0f, 1f)]
        [SerializeField]
        private float _randomTileChance;

        [Header("Level Exit")]
        [SerializeField]
        private Transform _levelExitSpawn;
        
        [SerializeField]
        private Transform _enemySpawnParent;

        private List<Transform> _enemySpawns = new List<Transform>();

        [SerializeField]
        private Transform _pickupSpawnParent;

        private List<Transform> _pickupSpawns = new List<Transform>();


        private void Awake()
        {
            _topTiles = _topExitParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();
            _bottomTiles = _bottomExitparent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();
            _rightTiles = _rightExitParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();
            _leftTiles = _leftExitparent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToArray();

            _enemySpawns = _enemySpawnParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToList();
            _pickupSpawns = _pickupSpawnParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToList();


            SetRandomTiles();
            SetTileTemplates();
            UpdateTiles();
        }

        private void UpdateTiles()
        {
            foreach (KeyValuePair<Vector3Int, TileBase> tile in _tilesToRemove)
            {
                TileMap.SetTile(tile.Key, null);
            }
            _tilesToRemove.Clear();
        }

        private void SetRandomTiles()
        {
            foreach (Transform tileparent in _randomTileParents)
            {
                if (UnityEngine.Random.Range(0f, 1f) >= _randomTileChance)
                {
                    SetTilesToBeKept(tileparent.GetComponentsInChildren<Transform>().Skip<Transform>(1));
                }
                else
                {
                    SetTilesToBeRemoved(tileparent.GetComponentsInChildren<Transform>().Skip<Transform>(1));
                }
            }
        }

        private void SetTileTemplates()
        {
            int chosenTemplateIndex = UnityEngine.Random.Range(0, _tileTemplateParents.Length);

            for (int i = 0; i < _tileTemplateParents.Length; i++)
            {
                if (i != chosenTemplateIndex)
                {
                    SetTilesToBeRemoved(_tileTemplateParents[i].GetComponentsInChildren<Transform>().Skip<Transform>(1));
                }
                else
                {
                    SetTilesToBeKept(_tileTemplateParents[i].GetComponentsInChildren<Transform>().Skip<Transform>(1));
                }
            }
        }

        private void SetTilesToBeKept(IEnumerable<Transform> tiles)
        {
            foreach (Transform transform in tiles)
            {
                Vector3Int cellPosition = TileMap.WorldToCell(transform.position);

                if (!_tilesToKeep.ContainsKey(cellPosition))
                {
                    _tilesToKeep.Add(cellPosition, TileMap.GetTile(cellPosition));

                    if (_tilesToRemove.ContainsKey(cellPosition))
                    {
                        _tilesToRemove.Remove(cellPosition);
                    }
                }
            }
        }

        private void SetTilesToBeRemoved(IEnumerable<Transform> tiles)
        {
            foreach (Transform transform in tiles)
            {
                Vector3Int cellPosition = TileMap.WorldToCell(transform.position);

                if (!_tilesToRemove.ContainsKey(cellPosition) && !_tilesToKeep.ContainsKey(cellPosition))
                {
                    _tilesToRemove.Add(cellPosition, TileMap.GetTile(cellPosition));
                }
            }
        }

        private void RemoveTiles(IEnumerable<Transform> tiles)
        {
            foreach (Transform transform in tiles)
            {
                Vector3Int cellPosition = TileMap.WorldToCell(transform.position);
                TileMap.SetTile(cellPosition, null);
            }
        }

        public void RemoveBottomTiles() => RemoveTiles(_bottomTiles);

        public void RemoveTopTiles() => RemoveTiles(_topTiles);

        public void RemoveRightTiles() => RemoveTiles(_rightTiles);

        public void RemoveLeftTiles() => RemoveTiles(_leftTiles);

        public Dictionary<Vector3, TileBase> GetRoomTiles() => Utilities.GetTilesInMap(TileMap);

        public Dictionary<Vector3, TileBase> GetGroundTiles() => Utilities.GetTilesInMap(GroundMap);

        public void SpawnLevelExit(GameObject exit)
        {
            GameObject.Instantiate(exit,_levelExitSpawn.position,_levelExitSpawn.rotation);
        }

        public GameObject SpawnEnemy(GameObject enemy)
        {
            GameObject spawnedEnemy = SpawnItemOnSpawnList(enemy, _enemySpawns, out Transform usedTransform);

            if(usedTransform != null)
            {
                _enemySpawns.Remove(usedTransform);
            }


            return spawnedEnemy;

        }

        public GameObject SpawnPickup(GameObject pickup)
        {
            GameObject spawnedPickup = SpawnItemOnSpawnList(pickup, _pickupSpawns, out Transform usedTransform);

            if (usedTransform != null)
            {
                _pickupSpawns.Remove(usedTransform);
            }

            return spawnedPickup;
        }

        private GameObject SpawnItemOnSpawnList(GameObject item, List<Transform> positions,out Transform usedTransform)
        {
            List<Transform> shuffledSpawns = Utilities.Shuffle<Transform>(positions);

            foreach(Transform t in shuffledSpawns)
            {
                if(TileMap.GetTile<TileBase>(TileMap.WorldToCell(t.position)) == null)
                {
                    usedTransform = t;
                    return GameObject.Instantiate(item,t.position,t.rotation);
                }
            }
            
            usedTransform = null;
            return null;
        }

    }

}
