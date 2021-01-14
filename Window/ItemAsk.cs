using Game.Data;
using TMPro;
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
			var currentItems = selectedCharacter.itemList;
			if (currentItems.Count >= inventoryManager.limitSlot) return;
            
			currentItems.Add(new Item(inventoryManager.dataManager.ownedItems.Find(i => i.itemId == currentItem.itemId)));

			foreach (var slot in inventoryManager.slotList)
			{
				if (slot.gameObject.activeSelf == false)
				{
					var newItem = new Item(inventoryManager.dataManager.ownedItems.Find(i => i.itemId == currentItem.itemId));
					inventoryManager.equipItem.Add(newItem);
					slot.gameObject.SetActive(true);
					var slotButton = slot.GetComponent<ItemButtonManager>();
					Insert(slotButton, newItem);
					continue;
				}
			}
			
			// Disappear
			print("this : " + currentItem.itemId);
			foreach (var character in inventoryManager.dataManager.currentCharacterList)
			{
				if (character.characterId == itemHaveCharacter.characterId)
				{
					character.itemList.Remove(character.itemList.Find(i => i.itemId == currentItem.itemId));
				}
			}
			
			inventoryManager.dataManager.SaveCharacter(inventoryManager.dataManager.currentCharacterList);
		}
		
		private void Insert(ItemButtonManager itemButton, Item item)
		{
			itemButton.itemId = item.itemId;
			itemButton.itemNameText.text = item.itemName;
			itemButton.priceText.text = item.itemPrice.ToString();
			itemButton.priceInt = item.itemPrice;
		}
	}
}
