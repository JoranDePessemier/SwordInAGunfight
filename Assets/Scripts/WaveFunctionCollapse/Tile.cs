using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class Tile : MonoBehaviour
    {

        [SerializeField]
        private Direction[] _openDirections;

        public Direction[] OpenDirections => _openDirections;

        public void SetAllNeighBours(Tile[] options)
        {
            SetNeighBoursforDirection(Direction.Up,Direction.Down,UpNeighbours,options);
            SetNeighBoursforDirection(Direction.Down, Direction.Up, DownNeighbours, options);
            SetNeighBoursforDirection(Direction.Right, Direction.Left, RightNeighbours, options);
            SetNeighBoursforDirection(Direction.Left, Direction.Right, LeftNeighbours, options);
        }

        private void SetNeighBoursforDirection(Direction direction,Direction oppositeDirection,List<Tile> neighbours,Tile[] options)
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

        public List<Tile> UpNeighbours { get; private set; } = new List<Tile>();

        public List<Tile> DownNeighbours { get; private set; } = new List<Tile>();


        public List<Tile> RightNeighbours { get; private set; } = new List<Tile>();

        public List<Tile> LeftNeighbours { get;private set; } = new List<Tile>();

    }

}
