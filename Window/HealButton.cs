using System;
using Game.Data;
using UnityEngine;

namespace Game.Window
{
    [SerializeField]
    public class HealButton : MonoBehaviour
    {
        public HealManager healManager;
        public bool isSelected = false;
        public GameObject active;
        public Character character;
        public int characterId;

        private void Awake()
        {
            if (healManager == null) FindObjectOfType<HealManager>();
            if (active == null) transform.Find("Active");
        }
        
        public void OnClick()
        {
            isSelected = !isSelected;
            active.SetActive(isSelected);
            
            healManager.SetInformation(characterId);
        }
    }
}
