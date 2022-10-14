using System;
using Dark_UI.Scripts;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Game.MainGame;
using UnityEngine.EventSystems;

namespace Game.Window
{
    public enum ItemButtonType
    {
        Buy, Sell, Inventory, Slot,
    }
    
    public class ItemButtonManager : MonoBehaviour
    {
        public AudioSource sound;
        public ItemButtonType buttonType = ItemButtonType.Buy;
        public float invokeTime = 0.3f;
        [SerializeField]
        [Range(0.4f, 10f)]
        public float fadeTime = 1f;

        public MarketManager marketManager;
        public InventoryManager inventoryManager;
        DataManager _dataManager;

        public Transform background;
        public Transform icon;
        public Transform highlighted;
        public Transform itemNameTransform;
        public TextMeshProUGUI itemNameText;
        public Transform price;
        public TextMeshProUGUI priceText;
        public int priceInt;
        public Transform button;
        public int itemId;
        public ItemType itemType;
        public CharacterClass detailType;
        public ItemConsumable itemConsumable;
        public String hasIt = " has it";
        public TextMeshProUGUI actTextNormal;
        public TextMeshProUGUI actTextHighlighted;
        public TextMeshProUGUI quantity;
        public bool unEquip = false;

        [ContextMenu("SetAct")]
        public void SetAct()
        {
           actTextNormal = transform.Find("Buttons").Find("Active").Find("Normal").Find("Text").GetComponent<TextMeshProUGUI>();
           actTextHighlighted = transform.Find("Buttons").Find("Active").Find("Highlighted").Find("Text").GetComponent<TextMeshProUGUI>();
        }
        
        private void Awake()
        {
            if (_dataManager == null) _dataManager = FindObjectOfType<DataManager>();
            
            if (marketManager == null)
            {
                if (buttonType == ItemButtonType.Buy || buttonType == ItemButtonType.Sell)
                {
                    marketManager = transform.parent.parent.parent.parent.GetComponent<MarketManager>();
                }
            }
        }

        public void Test()
        {
            print("Test");
        }
        
        public void Use()
        {
            var currentCharacters = _dataManager.CurrentCharacterList;
            
            print("Click");
            if (itemConsumable == ItemConsumable.Equip)
            {
                if (buttonType == ItemButtonType.Slot)
                {
                    // currentCharacters.Find
                    //         (character => character.characterName == inventoryManager.selectedCharacter.characterName)
                    //     .itemList
                    //     .RemoveAll(item => item.itemId == itemId);
                    inventoryManager.selectedCharacter.itemList.RemoveAll(item => item.itemId == itemId);
                    SaveCharacter(currentCharacters);
                    
                    inventoryManager.buttonManager.ResetSlot(itemId);
                    _dataManager.gameManager.ReItem(this);
                    // if (sound != null) sound.Play();
                    inventoryManager.ShowStats();
                    gameObject.SetActive(false);
                    
                    // _dataManager.SaveCharacter(currentCharacters);
                    
                }
                else if (buttonType == ItemButtonType.Inventory)
                {
                    Enroll(currentCharacters);
                }
            }
            else if (itemConsumable == ItemConsumable.Consume)
            {
                print("Consume!");
                if (itemNameText.text == "Memory")
                {
                    inventoryManager.selectedCharacter.exp += _dataManager.ownedItems.Find(
                        i => i.itemName == "Memory").baseInt;
                    // if (sound != null) sound.Play(); 
                    SaveCharacter(currentCharacters);
                    // consume memory
                    var newItems = new List<Item>();
                    
                    int countInit = GameUtility.CountItem(_dataManager.ownedItems, _dataManager.ownedItems.Find(
                        i => i.itemName == "Memory"));
                    countInit = countInit - 1;
                    int count = countInit;
                    foreach (var item in _dataManager.ownedItems)
                    {
                        if (item.itemName != "Memory")
                        {
                            newItems.Add(item);
                        }
                        else if (item.itemName == "Memory")
                        {
                            if (count != 0)
                            {
                                newItems.Add(item);
                                count -= 1;
                            }
                        }
                    }

                    _dataManager.ownedItems = newItems;
                    _dataManager.SaveOwnedItems();
                    
                    if (countInit <= 0)
                    {
                        inventoryManager.ShowOwnedItem();
                    }
                    else
                    {
                        quantity.text = countInit.ToString();
                    }

                    inventoryManager.InitLevelExp();
                    _dataManager.LevelCheck(true);
                    print(inventoryManager.selectedCharacter.characterName +" use memory and exp is " + inventoryManager.selectedCharacter.exp);
                }
            }
            else
            {
                print("Item use style is Non!");    
            }
            
            inventoryManager.ShowOwnedItem();
            // _dataManager.SaveCharacter(currentCharacters);
        }

        void SaveCharacter(List<Character> currentCharacters)
        {
            var newCharacters = new List<Character>(); 
            foreach (var character in currentCharacters)
            {
                newCharacters.Add(character.characterName == inventoryManager.selectedCharacter.characterName
                    ? inventoryManager.selectedCharacter
                    : character);
            }
            _dataManager.SaveCharacter(newCharacters);
        }

        // private void Take(Data.Character itemHaveCharacter, Item currentItem, List<Character> currentCharacterList)
        // {
        //     // Appear
        //     var currentItems = currentCharacterList.Find
        //             (character => character.characterName == inventoryManager.selectedCharacter.characterName)
        //         .itemList;
        //     if (currentItems.Count >= inventoryManager.limitSlot) return;
        //             
        //     currentItems.Add(new Item(_dataManager.ownedItems.Find(i => i.itemId == currentItem.itemId)));
        //
        //     foreach (var slot in inventoryManager.slotList)
        //     {
        //         if (slot.gameObject.activeSelf == false)
        //         {
        //             inventoryManager.equipItem.Add(new Item(_dataManager.ownedItems.Find(i => i.itemId == currentItem.itemId)));
        //             slot.gameObject.SetActive(true);
        //             var slotButton = slot.GetComponent<ItemButtonManager>();
        //             _dataManager.gameManager.Insert(slotButton, currentItem);
        //             break;
        //         }
        //     }
        //             
        //     // Disappear
        //     currentCharacterList.Find
        //             (character => character.characterName == itemHaveCharacter.characterName)
        //         .itemList
        //         .RemoveAll(item => item.itemId == itemId);
        //     
        //     _dataManager.SaveCharacter(currentCharacterList);
        //     inventoryManager.ShowOwnedItem();
        // }
        
        private void Enroll(List<Character> characters)
        {
            foreach (var equipItem in inventoryManager.selectedCharacter.itemList)
            {
                if (equipItem.itemType == itemType)
                {
                    return;
                }
            }
            
            if (detailType != CharacterClass.Non)
            {
                if (detailType != inventoryManager.selectedCharacter.classType)
                {
                    return;
                }
            }

            // foreach (var character in characters)
            // {
            //     foreach (var item in character.itemList)
            //     {
            //         if (item.itemId == itemId)
            //         {
            //             if (inventoryManager.selectedCharacter.characterName == character.characterName) return;
            //             
            //             Take(character, item, characters);
            //             return;
            //         }
            //     }
            // }
            
            var currentItems = inventoryManager.selectedCharacter.itemList;
            if (currentItems.Count >= inventoryManager.limitSlot) return;
            
            currentItems.Add(new Item(_dataManager.ownedItems.Find(i => i.itemId == itemId)));

            foreach (var slot in inventoryManager.slotList)
            {
                if (slot.gameObject.activeSelf == false)
                {
                    var newItem = new Item(_dataManager.ownedItems.Find(i => i.itemId == itemId));
                    inventoryManager.equipItem.Add(newItem);
                    slot.gameObject.SetActive(true);
                    var slotButton = slot.GetComponent<ItemButtonManager>();
                    _dataManager.gameManager.Insert(slotButton, newItem);

                    SaveCharacter(characters);
                    return;
                }
            }
            
            inventoryManager.ShowOwnedItem();
        }

        public void BuyOrSell()
        {
            if (_dataManager.baseData.currentGold <= 0) return;
            print("Click");
            
            if (buttonType == ItemButtonType.Buy)
            {
                var consumableCount = 0;
                foreach (var item in _dataManager.ownedItems)
                {
                    if (item.itemConsumable == ItemConsumable.Consume)
                    {
                        consumableCount = consumableCount + 1;

                    }
                }
                // print(consumableCount);
                var ownedItemCount = _dataManager.ownedItems.Count - consumableCount + 1;
                print(ownedItemCount);
                if (ownedItemCount >= inventoryManager.inventoryList.Count)
                {
                    // marketManager.inventoryFull.GetComponent<ModalWindowManager>().ModalWindowIn();
                    marketManager.inventoryFull.gameObject.SetActive(true);
                    print("Inventory is full");
                    return;
                }
                
                print(_dataManager.baseData.currentGold);
                print(priceInt);
                if (_dataManager.baseData.currentGold >= priceInt)
                {
                    ChangeGold(ItemButtonType.Buy);
                    
                    // input index
                    while (_dataManager.ownedItems.Exists(i => i.itemId == _dataManager.baseData.itemCount))
                    {
                        _dataManager.baseData.itemCount = _dataManager.baseData.itemCount + 1;    
                    }
                    _dataManager.SaveBaseData();
                    
                    var newItem = new Item(_dataManager.baseItemList.Find(i => i.itemName == itemNameText.text))
                    {
                        itemId = _dataManager.baseData.itemCount
                    };
                    print(newItem.itemName + "(" + newItem.itemConsumable + ")");
                    _dataManager.ownedItems.Add(newItem);

                    var cityArea = _dataManager.marketManager.characterManager.WhereIam();
                    print(cityArea.name);

                    // remove one item
                    var oldItemList = cityArea.assignedAccident.marketItem;
                    cityArea.assignedAccident.marketItem = new List<Item>();
                    var itemNumber = 0;
                    foreach (var item in oldItemList)
                    {
                        if (item.itemName != newItem.itemName)
                        {
                            cityArea.assignedAccident.marketItem.Add(item);    
                        }
                        else
                        {
                            if (itemNumber == 0)
                            {
                                itemNumber = 1;
                                continue;
                            }
                            else
                            {
                                _dataManager.ownedItems.Add(item);
                            }
                        }
                    }

                    CloseOrOpen(false);
                }
                else
                {
                    marketManager.noMoney.gameObject.SetActive(true);
                    // sound.Play();
                }
            }
            
            else if (buttonType == ItemButtonType.Sell)
            {
                ChangeGold(ItemButtonType.Sell);
                
                // remove one item
                // var oldItemList = _dataManager.ownedItems;

                _dataManager.ownedItems.RemoveAll(i => i.itemId == itemId);
                
                
                // _dataManager.ownedItems = oldItemList;
                
                // _dataManager.ownedItems = new List<Item>();
                // var itemNumber = 0;
                // foreach (var item in oldItemList)
                // {
                //     if (item.itemName != itemNameText.text)
                //     {
                //         _dataManager.ownedItems.Add(item);   
                //     }
                //     else
                //     {
                //         if (itemNumber == 0)
                //         {
                //             itemNumber = 1;
                //             continue;
                //         }
                //         else
                //         {
                //             _dataManager.ownedItems.Add(item);
                //         }
                //     }
                // }
                //
                // // _dataManager.ownedItems.RemoveAll(i => i.itemId == itemId);
                // bool isSell= false;
                // var oldOwnedItems = _dataManager.ownedItems; 
                // _dataManager.ownedItems = new List<Item>();
                // foreach (var item in oldItemList)
                // {
                //     if (item.itemName != itemNameText.text)
                //     {
                //         _dataManager.ownedItems.Add(item);
                //     }
                //     else if (item.itemName == itemNameText.text)
                //     {
                //         if (isSell == false)
                //         {
                //             isSell = true;    
                //             continue;
                //         }
                //         _dataManager.ownedItems.Add(item);
                //     }
                // }
                
                // foreach (var character in inventoryManager.dataManager.currentCharacterList)
                // {
                //     character.itemList?.RemoveAll(i => i.itemId == itemId);
                // }

                foreach (var slot in inventoryManager.slotList)
                {
                    _dataManager.gameManager.ReItem(slot.GetComponent<ItemButtonManager>());
                    slot.gameObject.SetActive(false);
                }
                
                CloseOrOpen(false);
            }
            
            _dataManager.SaveBaseData();
            _dataManager.SaveOwnedItems();
            marketManager.ShowOwnedItem();
        }
        
        void RemoveItem(List<Item> itemList)
        {}
        
        void CloseOrOpen(bool active)
        {
            sound.Play();
            gameObject.SetActive(active); 
        }

        void ChangeGold(ItemButtonType type)
        {
            int price = priceInt;

            if (price == 0)
            {
                print("Price is missing");
                return;
            }
            if (type == ItemButtonType.Buy)
            {
                _dataManager.baseData.currentGold = _dataManager.baseData.currentGold - price; 
                _dataManager.RenewalGold();
            }
            else if (type == ItemButtonType.Sell)
            {
                price = price / marketManager.discount;
                _dataManager.baseData.currentGold = _dataManager.baseData.currentGold + price; 
               _dataManager.RenewalGold();
            }
        }
    }
}
