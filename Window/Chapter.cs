using Game.Data;
using UnityEngine;

namespace Game.Window
{
    public class Chapter : MonoBehaviour
    {
        public CardManager cardManager;
        public DataManager dataManager;
        
        private void Awake()
        {
            if (dataManager == null) FindObjectOfType<DataManager>();
            if (cardManager == null) FindObjectOfType<CardManager>();
        }
    }
}
