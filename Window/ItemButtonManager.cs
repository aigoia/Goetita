using System;
using Dark_UI.Scripts;
using Game.Data;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;

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
        
        public MarketManager marketManager;
        public InventoryManager inventoryManager;
        DataManager _dataManager;
        
        public Transform background;
        public Transform icon;
        public Transform highlighted;
        public Transform itemName;
        public TextMeshProUGUI itemNameText;
        public Transform price;
        public TextMeshProUGUI priceText;
        public int priceInt;
        public Transform button;
        public int itemId;
        public ItemType itemType;
        public String hasIt = " has it";

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

        private void ReItem(ItemButtonManager itemButton)
        {
            itemButton.itemId = 0;
            itemButton.itemNameText.text = "Default";
            itemButton.priceInt = 0;
            itemButton.priceText.text = "0";
        }

        public void Equip()
        {
            if (buttonType == ItemButtonType.Slot)
            {
                _dataManager.currentCharacterList.Find
                        (character => character.characterName == inventoryManager.selectedCharacter.characterName)
                    .itemList
                    .RemoveAll(item => item.itemId == itemId);
                inventoryManager.equipManager.ResetSlot(itemId);
                ReItem(this);
                if (sound != null) sound.Play();
                gameObject.SetActive(false);
            }
            else if (buttonType == ItemButtonType.Inventory)
            {
                Enroll();
            }
            
            _dataManager.SaveCharacter(_dataManager.currentCharacterList);
        }

        private void Take(Data.Character itemHaveCharacter, Item currentItem)
        {
            // Appear
            var currentItems = _dataManager.currentCharacterList.Find
                    (character => character.characterName == inventoryManager.selectedCharacter.characterName)
                .itemList;
            if (currentItems.Count >= inventoryManager.limitSlot) return;
                    
            currentItems.Add(new Item(_dataManager.ownedItems.Find(i => i.itemId == currentItem.itemId)));
        
            foreach (var slot in inventoryManager.slotList)
            {
                if (slot.gameObject.activeSelf == false)
                {
                    inventoryManager.equipItem.Add(new Item(_dataManager.ownedItems.Find(i => i.itemId == currentItem.itemId)));
                    slot.gameObject.SetActive(true);
                    var slotButton = slot.GetComponent<ItemButtonManager>();
                    Insert(slotButton, currentItem);
                    break;
                }
            }
                    
            // Disappear
            _dataManager.currentCharacterList.Find
                    (character => character.characterName == itemHaveCharacter.characterName)
                .itemList
                .RemoveAll(item => item.itemId == itemId);
            
            _dataManager.SaveCharacter(_dataManager.currentCharacterList);
        }

        private void Insert(ItemButtonManager itemButton, Item item)
        {
            itemButton.itemId = item.itemId;
            itemButton.itemNameText.text = item.itemName;
            itemButton.priceText.text = item.itemPrice.ToString();
            itemButton.priceInt = item.itemPrice;
            itemButton.itemType = item.itemType;
        }
        
        private void Enroll()
        {
            foreach (var equipItem in inventoryManager.selectedCharacter.itemList)
            {
                if (equipItem.itemType == itemType)
                {
                    return;
                }
            }

            foreach (var character in inventoryManager.dataManager.currentCharacterList)
            {
                foreach (var item in character.itemList)
                {
                    if (item.itemId == itemId)
                    {
                        if (inventoryManager.selectedCharacter.characterId == character.characterId) return;
                        
                        Take(character, item);
                        return;
                    }
                }
            }
            
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
                    Insert(slotButton, newItem);
                    return;
                }
            }
        }

        public void BuyOrSell()
        {
            if (_dataManager.baseData.currentGold <= 0) return;
            
            if (buttonType == ItemButtonType.Buy)
            {
                if (_dataManager.ownedItems.Count >= inventoryManager.itemFull)
                {
                    marketManager.inventoryFull.GetComponent<ModalWindowManager>().ModalWindowIn();
                    print("Inventory is full");
                    return;
                }

                if (_dataManager.baseData.currentGold >= priceInt)
                {
                    ChangeGold(ItemButtonType.Buy);
                    _dataManager.baseData.itemCount = _dataManager.baseData.itemCount + 1;
                    _dataManager.SaveBaseData();
                    var newItem = new Item(_dataManager.itemList.Find(i => i.itemName == itemNameText.text))
                    {
                        itemId = _dataManager.baseData.itemCount
                    };
                    _dataManager.ownedItems.Add(newItem);
                    CloseOrOpen(false);
                }
                else
                {
                    sound.Play();
                }
            }
            
            else if (buttonType == ItemButtonType.Sell)
            {
                ChangeGold(ItemButtonType.Sell);
                _dataManager.ownedItems.RemoveAll(i => i.itemId == itemId);
                foreach (var character in inventoryManager.dataManager.currentCharacterList)
                {
                    character.itemList?.RemoveAll(i => i.itemId == itemId);
                }

                foreach (var slot in inventoryManager.slotList)
                {
                    ReItem(slot.GetComponent<ItemButtonManager>());
                    slot.gameObject.SetActive(false);
                }
                
                CloseOrOpen(false);
            }
            
            _dataManager.SaveBaseData();
            _dataManager.SaveOwnedItems();
        }
        
        void CloseOrOpen(bool active)
        {
            sound.Play();
            gameObject.SetActive(active != false); 
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
