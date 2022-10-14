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
			var newCharacters = inventoryManager.dataManager.CurrentCharacterList;
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
					inventoryManager.dataManager.gameManager.Insert(slotButton, newItem);
					continue;
				}
			}
			
			// Disappear
			print("this : " + currentItem.itemId);
			foreach (var character in newCharacters)
			{
				if (character.characterName == itemHaveCharacter.characterName)
				{
					character.itemList.Remove(character.itemList.Find(i => i.itemId == currentItem.itemId));
				}
			}
			
			inventoryManager.dataManager.SaveCharacter(newCharacters);
		}
		

	}
}
