using UnityEngine;

namespace Game.Window
{
    public class SkillManager : MonoBehaviour
    {
        public InventoryManager inventoryManager;

        private void Awake()
        {
            if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
        }
        
        
    }
}