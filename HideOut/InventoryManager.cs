using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Profile;

namespace Game.HideOut
{
	public class InventoryManager : MonoBehaviour
	{
		public MarketManager marketManager;
		public List<Transform> inventoryList;
		public List<Item> equipItem = new List<Item>();
		public List<Transform> slotList;
		public Transform itemStock;
		public Image bicProfile;
		public Profile.CharacterManager characterManager;
		public CharacterSelect characterSelect;
		public Profile.Character selectedCharacter;
		public Transform itemAsk;
		public int limitSlot = 2;

		private void Start()
		{
			inventoryList = new List<Transform>();
			for (int i = 0; i < itemStock.childCount; i++)
			{
				inventoryList.Add(itemStock.GetChild(i));
			}
			ShowItem(marketManager.ownedItems);
		}

		public void SetProfile()
		{
			SetBicProfile(0);
			characterSelect.ReSetting();

			for (int i = 0; i < characterManager.currentCharacterList.Count; i++)
			{
				characterSelect.buttonList[i].SetActive(true);
				characterSelect.buttonList[i].GetComponent<CharacterManager>().id =
					characterManager.currentCharacterList[i].CharacterId;
				characterSelect.buttonList[i].transform.Find("ProfileImage").GetComponent<Image>().sprite =
					characterManager.currentCharacterList[i].Profile.transform.Find("100x100").GetComponent<Image>().sprite;
			}

			selectedCharacter = characterManager.currentCharacterList[0];
			characterSelect.activeList[0].SetActive(true);
		}

		public void SetBicProfile(int i)
		{
			bicProfile.sprite = characterManager.currentCharacterList[i].Profile.transform.Find("360x360")
				.GetComponent<Image>().sprite;
		}

		public void ChangeSlot(int i)
		{
			foreach (var slot in slotList)
			{
				slot.gameObject.SetActive(false);
			}
			
			var n = 0;
			
			foreach (var item in selectedCharacter.ItemList)
			{
				slotList[n].gameObject.SetActive(true);
				var slotButton = slotList[n].GetComponent<ItemButtonManager>();
				Insert(slotButton, item);
				n = n + 1;
			}
		}
		
		private void Insert(ItemButtonManager itemButton, Item item)
		{
			itemButton.itemId = item.ItemId;
			itemButton.itemNameText.text = item.ItemName;
			itemButton.priceText.text = item.ItemPrice.ToString();
			itemButton.priceInt = item.ItemPrice;
		}

		public void ShowItem(List<Item> list)
		{
			foreach (var item in inventoryList)
			{
				item.gameObject.SetActive(false);
			}
			
			for (int i = 0; i < list.Count; i++)
			{
				inventoryList[i].gameObject.SetActive(true);
				GetMarketText(i).text = list[i].ItemName;
				GetPriceText(i).text = list[i].ItemPrice.ToString();
				inventoryList[i].GetComponent<ItemButtonManager>().priceInt = (int)list[i].ItemPrice;
				inventoryList[i].GetComponent<ItemButtonManager>().itemId = (int)list[i].ItemId;
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
	}
}
