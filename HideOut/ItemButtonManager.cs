using System;
using Dark_UI.Scripts;
using Game.Data;
using TMPro;
using UnityEngine;

namespace Game.HideOut
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
                inventoryManager.selectedCharacter.ItemList.RemoveAll(i => i.ItemId == itemId);
                ReItem(this);
                if (sound != null) sound.Play();
                gameObject.SetActive(false);
            }
            else if (buttonType == ItemButtonType.Inventory)
            {
                Enroll();
            }
        }

        private void Take(Data.Character itemHaveCharacter, Item currentItem)
        {
            // Appear
            var currentItems = inventoryManager.selectedCharacter.ItemList;
            if (currentItems.Count >= inventoryManager.limitSlot) return;
                    
            currentItems.Add(new Item(marketManager.ownedItems.Find(i => i.ItemId == currentItem.ItemId)));
        
            foreach (var slot in inventoryManager.slotList)
            {
                if (slot.gameObject.activeSelf == false)
                {
                    inventoryManager.equipItem.Add(new Item(marketManager.ownedItems.Find(i => i.ItemId == currentItem.ItemId)));
                    slot.gameObject.SetActive(true);
                    var slotButton = slot.GetComponent<ItemButtonManager>();
                    Insert(slotButton, currentItem);
                    break;
                }
            }
                    
            // Disappear
            itemHaveCharacter.ItemList.RemoveAll(i => i.ItemId == itemId);
        }

        private void Insert(ItemButtonManager itemButton, Item item)
        {
            itemButton.itemId = item.ItemId;
            itemButton.itemNameText.text = item.ItemName;
            itemButton.priceText.text = item.ItemPrice.ToString();
            itemButton.priceInt = item.ItemPrice;
        }
        
        private void Enroll()
        {
            foreach (var character in inventoryManager.dataManager.currentCharacterList)
            {
                foreach (var item in character.ItemList)
                {
                    if (item.ItemId == itemId)
                    {
                        if (inventoryManager.selectedCharacter.CharacterId == character.CharacterId) return;
                        
                        Take(character, item);
                        return;
                    }
                }
            }
            
            var currentItems = inventoryManager.selectedCharacter.ItemList;
            if (currentItems.Count >= inventoryManager.limitSlot) return;
            
            currentItems.Add(new Item(marketManager.ownedItems.Find(i => i.ItemId == itemId)));

            foreach (var slot in inventoryManager.slotList)
            {
                if (slot.gameObject.activeSelf == false)
                {
                    var newItem = new Item(marketManager.ownedItems.Find(i => i.ItemId == itemId));
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
            if (buttonType == ItemButtonType.Buy)
            {
                if (marketManager.ownedItems.Count >= inventoryManager.itemStock.GetComponent<MarketItem>().marketItemList.Count)
                {
                    marketManager.inventoryFull.GetComponent<ModalWindowManager>().ModalWindowIn();
                    print("Inventory is full");
                    return;
                }
                
                ChangeGold(ItemButtonType.Buy);
                marketManager.ownedItems.Add(new Item(marketManager.testItemList.Find(i => i.ItemId == itemId)));
                marketManager.ShowItem(marketManager.sell, marketManager.ownedItems);
            }
            
            else if (buttonType == ItemButtonType.Sell)
            {
                ChangeGold(ItemButtonType.Sell);
                marketManager.ownedItems.RemoveAll(i => i.ItemId == itemId);
                foreach (var character in inventoryManager.dataManager.currentCharacterList)
                {
                    character.ItemList.RemoveAll(i => i.ItemId == itemId);
                }

                foreach (var slot in inventoryManager.slotList)
                {
                    ReItem(slot.GetComponent<ItemButtonManager>());
                    slot.gameObject.SetActive(false);
                }
            }
            
            CloseOrOpen(false);
            marketManager.inventoryManager.ShowItem(marketManager.ownedItems);
        }
        
        void CloseOrOpen(bool active)
        {
            sound.Play();
            gameObject.SetActive(active != false); 
        }

        void ChangeGold(ItemButtonType type)
        {
            int money = priceInt;
            if (money == 0)
            {
                print("Price is missing");
                return;
            }

            if (type == ItemButtonType.Buy)
            {
                _dataManager.gold = _dataManager.gold - money; 
                _dataManager.RenewalGold();
            }
            else if (type == ItemButtonType.Sell)
            {
                money = money / marketManager.discount;
                _dataManager.gold = _dataManager.gold + money; 
               _dataManager.RenewalGold();
            }
        }
    }
}
