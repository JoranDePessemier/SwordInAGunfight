using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public class WaveFunction : MonoBehaviour
    {

        [SerializeField]
        private Vector2Int _dimensions;

        [SerializeField]
        private float _cellSize;

        [SerializeField]
        private Cell _cellObject;

        [SerializeField]
        private Tile[] _tilePrefabs;


        private List<Tile> _tileObjects = new List<Tile>();

        private Dictionary<Vector2Int,Cell> _gridComponents = new Dictionary<Vector2Int, Cell> ();

        private void Awake()
        {
            _tileObjects = new List<Tile>(_tilePrefabs);

            InitialiseGrid();
            StartCoroutine(Generate());

        }

        private IEnumerator Generate()
        {
            for (int i = 0; i < _dimensions.x * _dimensions.y; i++)
            {
                yield return new WaitForSeconds(0.05f);
                CollapseCell(CheckEntropy());
                _gridComponents = UpdateGeneration();
            }
        }

        private void InitialiseGrid()
        {
            for (int i = 0; i < _tileObjects.Count; i++)
            {
                Tile tile = _tileObjects[i];
                tile.SetAllNeighBours(_tileObjects.ToArray());
                _tileObjects[i] = tile;
            }

            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    Cell newcell = Instantiate(_cellObject, new Vector2(x * _cellSize, y * _cellSize),Quaternion.identity);
                    newcell.CreateCell(false, _tileObjects.ToArray());
                    _gridComponents.Add(new Vector2Int(x,y),newcell);
                }
            }
        }


        private List<Cell> CheckEntropy()
        {
            List<Cell> returnGrid = new List<Cell>(_gridComponents.Values);

            //Sort all uncollapsed cells based on the tileoptionlength
            returnGrid.RemoveAll(c => c.Collapsed);
            returnGrid.Sort((a, b) => { return a.TileOptions.Length - b.TileOptions.Length; });

            //keep only cells with a long tileoptionlength
            int arrayLength = returnGrid[0].TileOptions.Length;
            int stopIndex = default;

            for (int i = 1; i < returnGrid.Count; i++)
            {
                if (returnGrid[i].TileOptions.Length > arrayLength)
                {
                    stopIndex = i;
                    break;
                }
            }

            if(stopIndex > 0)
            {
                returnGrid.RemoveRange(stopIndex, returnGrid.Count - stopIndex);
            }

            return returnGrid;
        }

        private void CollapseCell(List<Cell> grid)
        {
            Cell cellToCollapse = grid[UnityEngine.Random.Range(0,grid.Count)];

            cellToCollapse.Collapsed = true;

            Tile selectedTile = cellToCollapse.TileOptions[UnityEngine.Random.Range(0, cellToCollapse.TileOptions.Length)];
            cellToCollapse.RecreateCell(new Tile[] { selectedTile });

            Tile foundTile = cellToCollapse.TileOptions[0];
            Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);
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

                        foreach (Tile tile in _tileObjects)
                        {
                            options.Add(tile);
                        }


                        //Update up
                        if (y > 0)
                        {
                            ChangeCellOptions(new Vector2Int(x, y - 1), Direction.Up, options);
                        }

                        //Update right
                        if (x < _dimensions.x - 1)
                        {
                            ChangeCellOptions(new Vector2Int(x + 1, y), Direction.Right, options);
                        }

                        //Update down
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
                    }
                }
            }

            return newGenerationCell;
        }

        private void ChangeCellOptions(Vector2Int coordinates,Direction direction ,List<Tile> options)
        {
            Cell cell = _gridComponents[coordinates];
            List<Tile> validOptions = new List<Tile>();

            foreach (Tile possibleOption in cell.TileOptions)
            {
                int validOptionIndex = Array.FindIndex(_tileObjects.ToArray(), obj => obj == possibleOption);

                switch (direction)
                {
                    case Direction.Up:
                        validOptions.AddRange(_tileObjects[validOptionIndex].UpNeighbours);
                        break;
                    case Direction.Down:
                        validOptions.AddRange(_tileObjects[validOptionIndex].DownNeighbours);
                        break;
                    case Direction.Left:
                        validOptions.AddRange(_tileObjects[validOptionIndex].RightNeighbours);
                        break;
                    case Direction.Right:
                        validOptions.AddRange(_tileObjects[validOptionIndex].LeftNeighbours);
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
    }
}

