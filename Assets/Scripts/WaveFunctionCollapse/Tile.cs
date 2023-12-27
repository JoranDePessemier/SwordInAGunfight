using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private Tile[] _upNeighBours;

        public Tile[] UpNeighbours
        {
            get { return _upNeighBours; }
            set { _upNeighBours = value; }
        }

        [SerializeField]
        private Tile[] _downNeighbours;

        public Tile[] DownNeighbours
        {
            get { return _downNeighbours; }
            set { _downNeighbours = value; }
        }

        [SerializeField]
        private Tile[] _rightNeighbours;

        public Tile[] RightNeighbours
        {
            get { return _rightNeighbours; }
            set { _rightNeighbours = value; }
        }

        [SerializeField]
        private Tile[] _leftNeighbours;

        public Tile[] LeftNeighbours
        {
            get { return _leftNeighbours; }
            set { _leftNeighbours = value; }
        }

    }

}
