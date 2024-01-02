
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public class WaveFunction : MonoBehaviour
    {
        [Header("CellValues")]
        [SerializeField]
        private Vector2Int _dimensions;

        [SerializeField]
        private float _cellSize;

        [SerializeField]
        private Cell _cellObject;

        [Header("TileObjects")]
        [SerializeField]
        private Tile[] _randomTileOptions;

        [SerializeField]
        private Tile[] _oneOpeningTileOptions;

        [SerializeField]
        private Tile _spawnTile;

        [SerializeField]
        private Tile _borderTile;

        [Header("TileMaps")]
        [SerializeField]
        private Tilemap _wallMap;

        [SerializeField]
        private Tilemap _groundMap;

        [Header("Enemies")]
        [SerializeField]
        private GeneratingItem[] _enemies;

        [Header("Pickups")]
        [SerializeField]
        private GeneratingItem[] _pickups;

        [Header("Exit")]
        [SerializeField]
        private GameObject _levelExit;

        private Dictionary<Vector2Int,Cell> _gridComponents = new Dictionary<Vector2Int, Cell> ();

        private List<Tile> _allTileObjects = new List<Tile>();

        private bool AllCellsCollapsed
        {
            get
            {
                foreach(Cell cell in _gridComponents.Values)
                {
                    if(!cell.Collapsed)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void Awake()
        {
            InitialiseGrid();
            StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            PlaceBorderRooms();
            PlaceSpawnRoom();
            _gridComponents = UpdateGeneration();

            while (!AllCellsCollapsed)
            {
                yield return new WaitForSeconds(0.1f);
                CollapseCell(CheckEntropy());
                _gridComponents = UpdateGeneration();
            }

            CombineTiles();
            SpawnEnemies();
            SpawnPickups();
            SpawnExit();
            DestroyGeneratedObjects();
        }

        private void PlaceBorderRooms()
        {
            _borderTile.SetAllNeighBours(_randomTileOptions);
            _allTileObjects.Add(_borderTile);
            
            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    if(x == 0 || y == 0 || x == _dimensions.x - 1 || y == _dimensions.y - 1)
                    {
                        CollapseSpecificCell(_gridComponents[new Vector2Int(x, y)],_borderTile);
                    }
                }
            }
        }

        private void PlaceSpawnRoom()
        {
            _spawnTile.SetAllNeighBours(_randomTileOptions);
            _allTileObjects.Add(_spawnTile);
            Vector2Int spawnCoords = _dimensions / 2;
            CollapseSpecificCell(_gridComponents[spawnCoords],_spawnTile);
        }

        private void InitialiseGrid()
        {
            foreach(Tile tile in _randomTileOptions)
            {
                _allTileObjects.Add(tile);
            }

            foreach(Tile tile in _oneOpeningTileOptions)
            {
                _allTileObjects.Add(tile);
            }

            for (int i = 0; i < _allTileObjects.Count; i++)
            {
                Tile tile = _allTileObjects[i];
                tile.SetAllNeighBours(_randomTileOptions);
                _allTileObjects[i] = tile;
            }

            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    Cell newcell = Instantiate(_cellObject, new Vector2(x * _cellSize, y * _cellSize),Quaternion.identity);
                    newcell.CreateCell(false, _allTileObjects.ToArray());
                    _gridComponents.Add(new Vector2Int(x,y),newcell);
                }
            }
        }


        private List<Cell> CheckEntropy()
        {
            List<Cell> returnList = new List<Cell>(_gridComponents.Values);

            //Sort all uncollapsed cells based on the open neighbours
            returnList.RemoveAll(c => c.Collapsed);
            returnList.Sort((a, b) => { return b.AmountOfOpenNeighBours - a.AmountOfOpenNeighBours; });

            int maxOpenNeighBours = returnList[0].AmountOfOpenNeighBours;

            if(maxOpenNeighBours <= 0)
            {
                return null;
            }

            int openNeighBourStopIndex = default;

            for (int i = 1; i < returnList.Count; i++)
            {
                if (returnList[i].AmountOfOpenNeighBours < maxOpenNeighBours)
                {
                    openNeighBourStopIndex = i;
                    break;
                }
            }

            if (openNeighBourStopIndex > 0)
            {
                returnList.RemoveRange(openNeighBourStopIndex, returnList.Count - openNeighBourStopIndex);
            }


            returnList.Sort((a, b) => { return a.TileOptions.Length - b.TileOptions.Length; });

            //keep only cells with a short tileoptionlength
            int arrayLength = returnList[0].TileOptions.Length;
            int stopIndex = default;

            for (int i = 1; i < returnList.Count; i++)
            {
                if (returnList[i].TileOptions.Length > arrayLength)
                {
                    stopIndex = i;
                    break;
                }
            }

            if(stopIndex > 0)
            {
                returnList.RemoveRange(stopIndex, returnList.Count - stopIndex);
            }

            return returnList;
        }

        private int GetOpenNeighBours(Vector2Int gridCoordinates, out List<Direction> openDirections)
        {
            int openCount = 0;

            openDirections = new List<Direction>();

            if (gridCoordinates.y > 0 && CheckOpenDirection(new Vector2Int(gridCoordinates.x, gridCoordinates.y - 1), Direction.Up))
            {
                openCount++;
                openDirections.Add(Direction.Down);
            }
            if (gridCoordinates.y < _dimensions.y - 1 &&  CheckOpenDirection(new Vector2Int(gridCoordinates.x, gridCoordinates.y + 1), Direction.Down))
            {
                openCount++;
                openDirections.Add(Direction.Up);
            }
            if (gridCoordinates.x > 0 && CheckOpenDirection(new Vector2Int(gridCoordinates.x - 1, gridCoordinates.y), Direction.Right))
            {
                openCount++;
                openDirections.Add(Direction.Left);
            }
            if (gridCoordinates.x < _dimensions.x -1  && CheckOpenDirection(new Vector2Int(gridCoordinates.x + 1, gridCoordinates.y), Direction.Left))
            {
                openCount++;
                openDirections.Add(Direction.Right);
            }

            return openCount;
        }

        private bool CheckOpenDirection( Vector2Int gridCoordinates, Direction oppositeDirection)
        {
            Cell cell = _gridComponents[gridCoordinates];
            return cell.Collapsed && cell.TileOptions[0].OpenDirections.Contains(oppositeDirection);
        }

        private void CollapseCell(List<Cell> grid)
        {
            if (grid == null)
            {
                CollapseAllRemaining();
                return;
            }


            Cell cellToCollapse = grid[UnityEngine.Random.Range(0, grid.Count)];

            SetNewOptionsOnZero(cellToCollapse);

            Tile selectedTile = cellToCollapse.TileOptions[UnityEngine.Random.Range(0, cellToCollapse.TileOptions.Length)];
            cellToCollapse.RecreateCell(new Tile[] { selectedTile });

            CollapseSpecificCell(cellToCollapse, selectedTile);
        }

        private void SetNewOptionsOnZero(Cell cellToCollapse)
        {
            if (cellToCollapse.TileOptions.Length <= 0)
            {
                List<Tile> newTileOptions = new List<Tile>();
                Vector2Int key = Vector2Int.zero;

                foreach (KeyValuePair<Vector2Int, Cell> cell in _gridComponents)
                {
                    if (cell.Value == cellToCollapse)
                    {
                        key = cell.Key;
                    }
                }

                if (GetOpenNeighBours(key, out List<Direction> directions) == 1)
                {
                    Direction direction = directions[0];

                    foreach (Tile tile in _allTileObjects)
                    {
                        if (tile.OpenDirections.Length == 1 && tile.OpenDirections[0] == direction)
                        {
                            newTileOptions.Add(tile);
                        }
                    }
                }
                cellToCollapse.RecreateCell(newTileOptions.ToArray());
            }
        }

        private void CollapseAllRemaining()
        {
            foreach(Cell cell in _gridComponents.Values)
            {
                if (!cell.Collapsed)
                {
                    CollapseSpecificCell(cell, _borderTile);
                }
            }
        }

        private void CollapseSpecificCell(Cell cellToCollapse, Tile collapsingTile)
        {
            cellToCollapse.Collapsed = true;

            cellToCollapse.RecreateCell(new Tile[] { collapsingTile});

            Tile foundTile = cellToCollapse.TileOptions[0];
            Tile instance = Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

            cellToCollapse.InstantiatedTile = instance;
        }

        private Dictionary<Vector2Int,Cell> UpdateGeneration()
        {
            Dictionary<Vector2Int, Cell > newGenerationCell = new Dictionary<Vector2Int, Cell>(_gridComponents);

            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    Vector2Int key = new Vector2Int(x, y);

                    if (_gridComponents[key].Collapsed)
                    {
                        newGenerationCell[key] = _gridComponents[key];
                    }
                    else
                    {
                        List<Tile> options = new List<Tile>();

                        foreach (Tile tile in _allTileObjects)
                        {
                            options.Add(tile);
                        }


                        //Update down
                        if (y > 0)
                        {
                            ChangeCellOptions(new Vector2Int(x, y - 1), Direction.Up, options);
                        }

                        //Update right
                        if (x < _dimensions.x - 1)
                        {
                            ChangeCellOptions(new Vector2Int(x + 1, y), Direction.Right, options);
                        }

                        //Update up
                        if (y < _dimensions.y - 1)
                        {
                            ChangeCellOptions(new Vector2Int(x, y + 1), Direction.Down, options);
                        }

                        //Update left
                        if (x > 0)
                        {
                            ChangeCellOptions(new Vector2Int(x - 1, y), Direction.Left, options);
                        }

                        Tile[] newTileArray = new Tile[options.Count];

                        for (int i = 0; i < options.Count; i++)
                        {
                            newTileArray[i] = options[i];
                        }

                        newGenerationCell[key].RecreateCell(newTileArray);
                        newGenerationCell[key].AmountOfOpenNeighBours = GetOpenNeighBours(key, out List<Direction> directionList);
                    }
                }
            }

            return newGenerationCell;
        }

        private void ChangeCellOptions(Vector2Int coordinates,Direction direction ,List<Tile> options)
        {
            Cell cell = _gridComponents[coordinates];

            if (!cell.Collapsed)
                return;

            List<Tile> validOptions = new List<Tile>();

            foreach (Tile possibleOption in cell.TileOptions)
            {
                int validOptionIndex = Array.FindIndex(_allTileObjects.ToArray(), obj => obj == possibleOption);

                switch (direction)
                {
                    case Direction.Up:
                        validOptions.AddRange(_allTileObjects[validOptionIndex].UpNeighbours);
                        break;
                    case Direction.Down:
                        validOptions.AddRange(_allTileObjects[validOptionIndex].DownNeighbours);
                        break;
                    case Direction.Left:
                        validOptions.AddRange(_allTileObjects[validOptionIndex].RightNeighbours);
                        break;
                    case Direction.Right:
                        validOptions.AddRange(_allTileObjects[validOptionIndex].LeftNeighbours);
                        break;
                }

            }

            SetValidOptions(options, validOptions);
        }

        private void SetValidOptions(List<Tile> optionList, List<Tile> validOptions)
        {
            for (int i = optionList.Count - 1; i >= 0; i--)
            {
                Tile tile = optionList[i];

                if (!validOptions.Contains(tile))
                {
                    optionList.RemoveAt(i);
                }
            }
        }

        private void CombineTiles()
        {
            foreach(Cell cell in _gridComponents.Values)
            {
                Utilities.AddTilesToTileMap(ref _groundMap, cell.InstantiatedTile.GroundTiles);
                Utilities.AddTilesToTileMap(ref _wallMap, cell.InstantiatedTile.WallTiles);
            }
        }

        private void DestroyGeneratedObjects()
        {
            foreach(Cell cell in _gridComponents.Values)
            {
                Destroy(cell.InstantiatedTile.gameObject);
                Destroy(cell.gameObject);
            }
        }

        private void SpawnEnemies()
        {
            List<Tile> availableEnemyTiles = new List<Tile>();

            foreach(Cell cell in _gridComponents.Values)
            {
                availableEnemyTiles.Add(cell.InstantiatedTile);
            }

            foreach (GeneratingItem item in _enemies)
            {
                int amountGenerated = 0;

                while (amountGenerated <= item.AmountToGenerate && availableEnemyTiles.Count > 0)
                {
                    Tile spawnTile = availableEnemyTiles[UnityEngine.Random.Range(0, availableEnemyTiles.Count)];

                    if (spawnTile.SpawnEnemy(item.Item) != null)
                    {
                        amountGenerated++;
                    }
                    else
                    {
                        availableEnemyTiles.Remove(spawnTile);
                    }
                }
            }
        }

        private void SpawnPickups()
        {
            List<Tile> availablePickupTiles = new List<Tile>();

            foreach (Cell cell in _gridComponents.Values)
            {
                availablePickupTiles.Add(cell.InstantiatedTile);
            }

            foreach (GeneratingItem item in _pickups)
            {
                int amountGenerated = 0;

                while (amountGenerated <= item.AmountToGenerate && availablePickupTiles.Count > 0)
                {
                    Tile spawnTile = availablePickupTiles[UnityEngine.Random.Range(0, availablePickupTiles.Count)];

                    if (spawnTile.SpawnPickup(item.Item) != null)
                    {
                        amountGenerated++;
                    }
                    else
                    {
                        availablePickupTiles.Remove(spawnTile);
                    }
                }
            }

        }

        private void SpawnExit()
        {
            Vector2Int randomCoords = Vector2Int.zero;
            int borderIndex = UnityEngine.Random.Range(0, 3);

            switch (borderIndex)
            {
                case 0:
                    randomCoords = new Vector2Int(UnityEngine.Random.Range(1,_dimensions.x - 1), _dimensions.y - 2);
                    break;
                
                case 1:
                    randomCoords = new Vector2Int(UnityEngine.Random.Range(1, _dimensions.x - 1), 1);
                    break;

                case 2:
                    randomCoords = new Vector2Int(_dimensions.x - 2, UnityEngine.Random.Range(1, _dimensions.y - 1));
                    break;

                case 3:
                    randomCoords = new Vector2Int(1, UnityEngine.Random.Range(1, _dimensions.y - 1));
                    break;
            }

            while (_gridComponents[randomCoords].InstantiatedTile.SpawnLevelExit(_levelExit) == null)
            {
                borderIndex = UnityEngine.Random.Range(0, 3);

                switch (borderIndex)
                {
                    case 0:
                        randomCoords.x -= 1;
                        break;

                    case 1:
                        randomCoords.x += 1;
                        break;

                    case 2:
                        randomCoords.y -= 1;
                        break;

                    case 3:
                        randomCoords.y += 1;
                        break;
                }

                randomCoords = new Vector2Int(Mathf.Clamp(randomCoords.x, 1, _dimensions.x - 2), Mathf.Clamp(randomCoords.y, 1, _dimensions.y - 2));
            }
        }
    }
}

