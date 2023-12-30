using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class Cell : MonoBehaviour
    {
        public bool Collapsed { get; set; }

        public int AmountOfOpenNeighBours { get;set; }

        public Tile[] TileOptions { get; private set; }

        [SerializeField]
        private TextMesh _optionsText;

        [SerializeField]
        private TextMesh _openNeighBoursText;

        public Tile InstantiatedTile { get; set; }

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

            _optionsText.text = TileOptions.Length.ToString();
            
            if(Collapsed)
            {
                _optionsText.color = Color.red;
            }
            else
            {
                _optionsText.color = Color.green;
            }

            _openNeighBoursText.text = AmountOfOpenNeighBours.ToString();
        }
    }
}


