using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class WaveFunction : MonoBehaviour
    {

        [SerializeField]
        private Vector2Int _dimensions;

        [SerializeField]
        private float _cellSize;

        [SerializeField]
        private Cell _cellObject;

        [SerializeField]
        private Tile[] _tileObjects;

        private Dictionary<Vector2Int,Cell> _gridComponents = new Dictionary<Vector2Int, Cell> ();

        private void Awake()
        {
            InitialiseGrid();
            CollapseCell(CheckEntropy());
            UpdateGeneration();
        }

        private void InitialiseGrid()
        {
            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    Cell newcell = Instantiate(_cellObject, new Vector2(x * _cellSize, y * _cellSize),Quaternion.identity);
                    newcell.CreateCell(false, _tileObjects);
                    _gridComponents.Add(new Vector2Int(x,y),newcell);
                }
            }
        }

        
        private List<Cell> CheckEntropy()
        {
            List<Cell> returnGrid = new List<Cell>(_gridComponents.Values);

            //Sort all uncollapsed cells based on the tileoptionlength
            returnGrid.RemoveAll(c => c.collapsed);
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

            cellToCollapse.collapsed = true;

            Tile selectedTile = cellToCollapse.TileOptions[UnityEngine.Random.Range(0, cellToCollapse.TileOptions.Length)];
            cellToCollapse.TileOptions = new Tile[] {selectedTile };

            Tile foundTile = cellToCollapse.TileOptions[0];
            Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);
        }

        private void UpdateGeneration()
        {
            Dictionary<Vector2Int, Cell > newGenerationCell = new Dictionary<Vector2Int, Cell>(_gridComponents);

            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    Vector2Int key = new Vector2Int(x, y);

                    if (_gridComponents[key].collapsed)
                    {
                        newGenerationCell[key] = _gridComponents[key];
                    }
                    else
                    {
                        List<Tile> options = new List<Tile>();

                        foreach(Tile tile in _tileObjects)
                        {
                            options.Add(tile);
                        }


                        //Update up
                        if (y > 0)
                        {
                            Cell up = _gridComponents[new Vector2Int(x, y - 1)];
                            List<Tile> validOptions = new List<Tile>();

                            foreach(Tile possibleOption in up.TileOptions)
                            {
                                int validOptionIndex = Array.FindIndex(_tileObjects, obj => obj == possibleOption);
                                Tile[] validOption = _tileObjects[validOptionIndex].UpNeighbours;

                                validOptions = validOptions.Concat(validOption).ToList();
                            }

                            SetValidOptions(options, validOptions);
                        }
                    }
                }
            }
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

