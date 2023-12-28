using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class Cell : MonoBehaviour
    {
        public bool Collapsed { get; set; }
        public Tile[] TileOptions { get; private set; } 

        public void CreateCell(bool collapseState, Tile[] tiles)
        {
            Collapsed = collapseState;
            TileOptions = tiles;
        }

        public void RecreateCell(Tile[] tiles)
        {
            TileOptions = tiles;
        }

        private void Update()
        {
            TextMesh text = this.GetComponentInChildren<TextMesh>();

            text.text = TileOptions.Length.ToString();
            
            if(Collapsed)
            {
                text.color = Color.red;
            }
            else
            {
                text.color = Color.green;
            }
        }
    }
}


