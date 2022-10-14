using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Game.Data;
using Game.MainGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    public class DeckManager : MonoBehaviour
    {
        public InventoryManager inventoryManager;
        public List<Deck> deckList;

        private void Awake()
        {
            if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
        }

        private void Start()
        {
            MakeDeck();
        }

        public void MakeDeck()
        {
            var deckTypes = new List<DeckType>();
            
            if (inventoryManager.selectedCharacter.classType == CharacterClass.Claymore)
            {
                deckTypes.Add(DeckType.ShadowWalk);
                
                if (inventoryManager.selectedCharacter.characterName == "Tina")
                {
                    deckTypes.Add(DeckType.ShadowWalk);
                }
                
                foreach (var item in inventoryManager.selectedCharacter.itemList)
                {
                    if (item.itemType == ItemType.Weapon)
                    {
                        if (item.itemGrade == ItemGrade.One)
                        {
                            deckTypes.Add(DeckType.ShadowWalk);
                        }
                        else if (item.itemGrade == ItemGrade.Two)
                        {
                            deckTypes.Add(DeckType.ShadowWalk);
                            deckTypes.Add(DeckType.ShadowWalk);
                        }
                        else if (item.itemGrade == ItemGrade.Three)
                        {
                            deckTypes.Add(DeckType.ShadowWalk);
                            deckTypes.Add(DeckType.ShadowWalk);
                            deckTypes.Add(DeckType.ShadowWalk);
                        }
                        else if (item.itemGrade == ItemGrade.Four)
                        {
                            deckTypes.Add(DeckType.ShadowWalk);
                            deckTypes.Add(DeckType.ShadowWalk);
                            deckTypes.Add(DeckType.ShadowWalk);
                            deckTypes.Add(DeckType.ShadowWalk);
                        }
                        break;
                    }
                }
            }
            else if (inventoryManager.selectedCharacter.classType == CharacterClass.Ranger)
            {
                deckTypes.Add(DeckType.PowerFull);

                if (inventoryManager.selectedCharacter.characterName == "Tina")
                {
                    deckTypes.Add(DeckType.PowerFull);
                }
                
                foreach (var item in inventoryManager.selectedCharacter.itemList)
                {
                    if (item.itemType == ItemType.Weapon)
                    {
                        if (item.itemGrade == ItemGrade.One)
                        {
                            deckTypes.Add(DeckType.PowerFull);
                        }
                        else if (item.itemGrade == ItemGrade.Two)
                        {
                            deckTypes.Add(DeckType.PowerFull);
                            deckTypes.Add(DeckType.PowerFull);
                        }
                        else if (item.itemGrade == ItemGrade.Three)
                        {
                            deckTypes.Add(DeckType.PowerFull);
                            deckTypes.Add(DeckType.PowerFull);
                            deckTypes.Add(DeckType.PowerFull);
                        }
                        else if (item.itemGrade == ItemGrade.Four)
                        {
                            deckTypes.Add(DeckType.PowerFull);
                            deckTypes.Add(DeckType.PowerFull);
                            deckTypes.Add(DeckType.PowerFull);
                            deckTypes.Add(DeckType.PowerFull);
                        }
                        
                        break;
                    }
                }
                
            }

            foreach (var deck in deckList)
            {
                deck.gameObject.SetActive(false);
            }
            
            var count = 0;
            foreach (var deckType in deckTypes)
            {
                foreach (var image in deckList[count].deckImages)
                {
                    image.gameObject.SetActive(false);

                    if (image.gameObject.name == deckType.ToString())
                    {
                        image.gameObject.SetActive(true);
                    }
                }
                
                deckList[count].gameObject.SetActive(true);
                count += 1;

                if (count == deckList.Count) return;
            }
        }
    }
    
}
