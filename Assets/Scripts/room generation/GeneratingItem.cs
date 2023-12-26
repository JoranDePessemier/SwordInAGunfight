using UnityEngine;

namespace RoomGeneration
{
    [System.Serializable ]
    public struct GeneratingItem
    {
        [SerializeField]
        private GameObject _item;

        public GameObject Item
        {
            get { return _item; }
            private set { _item = value; }
        }

        [SerializeField]
        private int _amountToGenerate;

        public int AmountToGenerate
        {
            get { return _amountToGenerate; }
            private set { _amountToGenerate = value; }
        }

    }
}


