using System.Collections.Generic;
using UnityEngine;

namespace Game.Window
{
    public class TraitManager : MonoBehaviour
    {
        public InventoryManager inventoryManager;
        public List<Deck> deckList;

        private void Awake()
        {
            if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
        }

        public void TraitOn(string trait, int count)
        {
            deckList[count].gameObject.SetActive(true);
            foreach (var image in deckList[count].deckImages)
            {
                image.gameObject.SetActive(false);

                if (image.gameObject.name == trait)
                {
                    image.gameObject.SetActive(true);
                }
            }
        }
        
        public void TraitOffAll()
        {

            foreach (var deck in deckList)
            {
                deck.gameObject.SetActive(false);
            }
        }
    }
}
