using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class Tile : MonoBehaviour
    {

        [SerializeField]
        private Direction[] _openDirections;

        [SerializeField]
        private Tilemap _groundMap;

        public Dictionary<Vector3,TileBase> GroundTiles
        {
            get
            {
                return Utilities.GetTilesInMap(_groundMap);
            }
        }

        [SerializeField]
        private Tilemap _wallMap;

        public Dictionary<Vector3, TileBase> WallTiles
        {
            get
            {
                return Utilities.GetTilesInMap(_wallMap);
            }
        }

        [SerializeField]
        private Transform _enemySpawnParent;

        [SerializeField]
        private Transform _pickupSpawnParent;

        [SerializeField]
        private Transform _levelExitSpawn;

        public Direction[] OpenDirections => _openDirections;
        public List<Tile> UpNeighbours { get; private set; } = new List<Tile>();
        public List<Tile> DownNeighbours { get; private set; } = new List<Tile>();
        public List<Tile> RightNeighbours { get; private set; } = new List<Tile>();
        public List<Tile> LeftNeighbours { get;private set; } = new List<Tile>();


        private List<Transform> _enemySpawns = new List<Transform>();
        private List<Transform> _pickupSpawns = new List<Transform>();

        private void Awake()
        {
            _enemySpawns = _enemySpawnParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToList();
            _pickupSpawns = _pickupSpawnParent.GetComponentsInChildren<Transform>().Skip<Transform>(1).ToList();
        }

        public GameObject SpawnEnemy(GameObject enemy)
        {
            GameObject spawnedEnemy = SpawnItemOnSpawnList(enemy, _enemySpawns, out Transform usedTransform);

            if (usedTransform != null)
            {
                _enemySpawns.Remove(usedTransform);
            }
            
            return spawnedEnemy;
        }

        private GameObject SpawnItemOnSpawnList(GameObject item, List<Transform> positions, out Transform usedTransform)
        {
            if(positions.Count <= 0)
            {
                usedTransform = null;
                return null;
            }


            List<Transform> shuffledSpawns = Utilities.Shuffle<Transform>(positions);

            usedTransform = shuffledSpawns[0];

            return GameObject.Instantiate(item, usedTransform.position, usedTransform.rotation);
        }

        public void SetAllNeighBours(Tile[] options)
        {
            UpNeighbours.Clear();
            DownNeighbours.Clear();
            RightNeighbours.Clear();
            LeftNeighbours.Clear();
            SetNeighBoursforDirection(Direction.Up, Direction.Down, UpNeighbours, options);
            SetNeighBoursforDirection(Direction.Down, Direction.Up, DownNeighbours, options);
            SetNeighBoursforDirection(Direction.Right, Direction.Left, RightNeighbours, options);
            SetNeighBoursforDirection(Direction.Left, Direction.Right, LeftNeighbours, options);
        }

        private void SetNeighBoursforDirection(Direction direction, Direction oppositeDirection, List<Tile> neighbours, Tile[] options)
        {
            if (OpenDirections.Contains(direction))
            {
                foreach (Tile tile in options)
                {
                    if (tile.OpenDirections.Contains(oppositeDirection))
                    {
                        neighbours.Add(tile);
                    }
                }
            }
            else
            {
                foreach (Tile tile in options)
                {
                    if (!tile.OpenDirections.Contains(oppositeDirection))
                    {
                        neighbours.Add(tile);
                    }
                }
            }
        }

        internal object SpawnPickup(GameObject pickup)
        {
            GameObject spawnedPickup = SpawnItemOnSpawnList(pickup, _pickupSpawns, out Transform usedTransform);

            if (usedTransform != null)
            {
                _pickupSpawns.Remove(usedTransform);
            }

            return spawnedPickup;
        }

        public GameObject SpawnLevelExit(GameObject exit)
        {
            if (_levelExitSpawn == null)
                return null;

            return GameObject.Instantiate(exit, _levelExitSpawn.position, _levelExitSpawn.rotation);
        }
    }

}
