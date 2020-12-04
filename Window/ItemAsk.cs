﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
	public class ItemAsk : MonoBehaviour
	{
		public TextMeshProUGUI testMesh;
		public Button take;
		public Button keep;
		public Data.Character itemHaveCharacter;
		public Item currentItem;
		public InventoryManager inventoryManager;
		public MarketManager marketManager;
		public Data.Character selectedCharacter;

		public void Take()
		{
			// Appear
			var currentItems = selectedCharacter.ItemList;
			if (currentItems.Count >= inventoryManager.limitSlot) return;
            
			currentItems.Add(new Item(marketManager.ownedItems.Find(i => i.ItemId == currentItem.ItemId)));

			foreach (var slot in inventoryManager.slotList)
			{
				if (slot.gameObject.activeSelf == false)
				{
					var newItem = new Item(marketManager.ownedItems.Find(i => i.ItemId == currentItem.ItemId));
					inventoryManager.equipItem.Add(newItem);
					slot.gameObject.SetActive(true);
					var slotButton = slot.GetComponent<ItemButtonManager>();
					Insert(slotButton, newItem);
					continue;
				}
			}
			
			// Disappear
			print("this : " + currentItem.ItemId);
			foreach (var character in inventoryManager.dataManager.currentCharacterList)
			{
				if (character.CharacterId == itemHaveCharacter.CharacterId)
				{
					character.ItemList.Remove(character.ItemList.Find(i => i.ItemId == currentItem.ItemId));
				}
			}
		}
		
		private void Insert(ItemButtonManager itemButton, Item item)
		{
			itemButton.itemId = item.ItemId;
			itemButton.itemNameText.text = item.ItemName;
			itemButton.priceText.text = item.ItemPrice.ToString();
			itemButton.priceInt = item.ItemPrice;
		}
	}
}