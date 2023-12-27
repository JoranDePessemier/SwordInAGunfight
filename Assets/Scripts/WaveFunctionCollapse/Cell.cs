using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class Cell : MonoBehaviour
    {
        public bool collapsed { get; set; }
        public Tile[] TileOptions;

        public void CreateCell(bool collapseState, Tile[] tiles)
        {
            collapsed = collapseState;
            TileOptions = tiles;
        }

        public void RecreateCell(Tile[] tiles)
        {
            TileOptions = tiles;
        }
    }
}


