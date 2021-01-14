using System;
using System.Collections.Generic;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ES3Types;

namespace Game.Window
{
	public class InventoryManager : MonoBehaviour
	{
		public MarketManager marketManager;
		public List<Transform> inventoryList;
		public List<Item> equipItem = new List<Item>();
		public List<Transform> slotList;
		public Transform itemStock;
		public int itemFull = 24;
		public Image bicProfile;
		public DataManager dataManager;
		public EquipManager equipManager;
		public CharacterSelect characterSelect;
		public Data.Character selectedCharacter;
		public int limitSlot = 2;
		public Button defaultButton;

		public TextMeshProUGUI characterName;
		public TextMeshProUGUI characterClass;
		public TextMeshProUGUI level;
		
		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			if (equipManager == null) equipManager = FindObjectOfType<EquipManager>();
		}

		private void Start()
		{
			selectedCharacter = dataManager.currentCharacterList[0];
			EnrollItems();
		}

		private void EnrollItems()
		{
			foreach (var character in dataManager.currentCharacterList)
			{
				foreach (var item in character.itemList)
				{
					print("Item");

					print(item.itemName);
					foreach (var slot in slotList)
					{
						if (slot.gameObject.activeSelf == false)
						{
							var newItem = new Item(dataManager.ownedItems.Find(i => i.itemId == item.itemId));
							equipItem.Add(newItem);
							
							slot.gameObject.SetActive(true);
							var slotButton = slot.GetComponent<ItemButtonManager>();
							Insert(slotButton, newItem);
							break;
						}
					}
				}
			}
		}

		[ContextMenu("MakeInventoryList")]
		public void MakeInventoryList()
		{
			inventoryList = new List<Transform>();
			for (int i = 0; i < itemStock.childCount; i++)
			{
				inventoryList.Add(itemStock.GetChild(i));
			}
		}

		public void ShowOwnedItem()
		{
			ShowItem(dataManager.ownedItems);
		}

		public void DefaultOn()
		{
			if (defaultButton == null) return;
			
			defaultButton.animator.Play("Hover to Pressed");

			selectedCharacter = dataManager.currentCharacterList[0];
			SetInformation();
		}
		
		public void SetProfile()
		{
			// SetBicProfile(0);
			characterSelect.ReSetting();
			equipManager.ResetAllSlot();

			for (int i = 0; i < dataManager.currentCharacterList.Count; i++)
			{
				characterSelect.buttonList[i].SetActive(true);
				var characterId = dataManager.currentCharacterList[i].characterId;
				characterSelect.buttonList[i].GetComponent<CharacterButton>().characterId = characterId;
				
				var profileImage = dataManager.imageList[characterId - 1].transform.Find("100x100")
						.GetComponent<Image>().sprite;
				characterSelect.buttonList[i].transform.Find("ProfileImage").GetComponent<Image>().sprite =
					profileImage;
			}

			selectedCharacter = dataManager.currentCharacterList[0];
			characterSelect.activeList[0].SetActive(true);
			ChangeSlot(selectedCharacter.characterId);
		}

		public void SetBicProfile()
		{
			if (dataManager.gameObject.activeSelf == false) dataManager.gameObject.SetActive(true);
		}

		public void ChangeSlot(int i)
		{
			foreach (var slot in slotList)
			{
				slot.gameObject.SetActive(false);
			}
			
			var n = 0;
			
			foreach (var item in selectedCharacter.itemList)
			{
				slotList[n].gameObject.SetActive(true);
				var slotButton = slotList[n].GetComponent<ItemButtonManager>();
				Insert(slotButton, item);
				n = n + 1;
			}
		}
		
		private void Insert(ItemButtonManager itemButton, Item item)
		{
			itemButton.itemId = item.itemId;
			itemButton.itemNameText.text = item.itemName;
			itemButton.priceText.text = item.itemPrice.ToString();
			itemButton.priceInt = item.itemPrice;
			itemButton.itemType = item.itemType;
		}

		void ShowItem(List<Item> list)
		{
			foreach (var item in inventoryList)
			{
				item.gameObject.SetActive(false);
			}
			
			for (int i = 0; i < list.Count; i++)
			{
				inventoryList[i].gameObject.SetActive(true);
				GetMarketText(i).text = list[i].itemName;
				GetPriceText(i).text = list[i].itemPrice.ToString();
				inventoryList[i].GetComponent<ItemButtonManager>().priceInt = (int)list[i].itemPrice;
				inventoryList[i].GetComponent<ItemButtonManager>().itemId = (int)list[i].itemId;
				inventoryList[i].GetComponent<ItemButtonManager>().itemType = list[i].itemType;
			}
		}
		
		TextMeshProUGUI GetPriceText(int index)
		{
			return inventoryList[index].GetComponent<ItemButtonManager>().priceText;
		}

		TextMeshProUGUI GetMarketText(int index)
		{
			return inventoryList[index].GetComponent<ItemButtonManager>().itemNameText;
		}

		public void SetInformation()
		{
			if (selectedCharacter == null) return;

			characterName.text = selectedCharacter.characterName;
			level.text = selectedCharacter.level.ToString();

			if (selectedCharacter.type == CharacterClass.Ranger)
			{
				characterClass.text = "Ranger";
			}
			if (selectedCharacter.type == CharacterClass.Claymore)
			{
				characterClass.text = "Claymore";
			}
		}
	}
}