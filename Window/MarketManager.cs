using System.Collections.Generic;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace Game.Window
{
	public class MarketManager : MonoBehaviour
	{
		public InventoryManager inventoryManager;
		public ItemDataManager itemDataManager;
		public DataManager dataManager;
		public CharacterManager characterManager;
		
		public int discount = 2;
		public MarketItem buy;
		public MarketItem sell;
		public List<Item> supplyList;

		public int testGold = 1000;
		public Transform inventoryFull;
		public Transform noMoney;
		public CityArea currentCityArea;

		public Button defaultButton;
		public List<Item> defaultBuyList = new List<Item>();

		public int gradeOnePercent = 75;
		public int gradeTwoPercent = 55;
		public int gradeThreePercent = 35;
		public int gradeFourPercent = 15;

		public Scrollbar scrollbar;

		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
		}

		private void Start()
		{
			// SettingGold(testGold); // Test
			// supplyList = new List<Item>();
			// dataManager.MakeItemList();
			// InitSetting(buy, null);
			
			DefaultButtonOnclick();
		}

		public void ScrollbarInit()
		{
			scrollbar.value = 1;
		}

		public List<Item> MakeBuyList()
		{
			List<Item> newList = new List<Item>();

			newList.Add(FindBaseItem("Memory"));
			
			AddItem(newList, FindBaseItem("Sword"));
			AddItem(newList, FindBaseItem("Katana"));
			AddItem(newList, FindBaseItem("Bic One"));
			AddItem(newList, FindBaseItem("Hammer"));
			AddItem(newList, FindBaseItem("Axe"));
			AddItem(newList, FindBaseItem("Alane"));

			AddItem(newList, FindBaseItem("Flak Jacket"));
			AddItem(newList, FindBaseItem("Armor"));
			AddItem(newList, FindBaseItem("Barrier"));
			AddItem(newList, FindBaseItem("Tanker"));
			AddItem(newList, FindBaseItem("Shield"));

			AddItem(newList, FindBaseItem("Aes Gun"));
			AddItem(newList, FindBaseItem("Laser Gun"));
			AddItem(newList, FindBaseItem("Pistol Gun"));
			AddItem(newList, FindBaseItem("High Gun"));
			AddItem(newList, FindBaseItem("Dubble Gun"));
			AddItem(newList, FindBaseItem("Pulse Gun"));
			AddItem(newList, FindBaseItem("Rail Gun"));
			AddItem(newList, FindBaseItem("Plasma Gun"));
				
			return newList;
		}
		
		Item FindBaseItem(string itemName)
		{
			return dataManager.baseItemList.Find(i => i.itemName == itemName);
		}

		public void AddItem(List<Item> items, Item thisItem)
		{
			var number = Random.Range(0, 100);

			if (thisItem.itemGrade == ItemGrade.One)
			{
				if (number < gradeOnePercent)
				{
					items.Add(thisItem);
				}
			}
			else if (thisItem.itemGrade == ItemGrade.Two)
			{
				if (number < gradeTwoPercent)
				{
					items.Add(thisItem);
				}
			}
			else if (thisItem.itemGrade == ItemGrade.Three)
			{
				if (number < gradeThreePercent)
				{
					items.Add(thisItem);
				}
			}
			else if (thisItem.itemGrade == ItemGrade.Four)
			{
				if (number < gradeFourPercent)
				{
					items.Add(thisItem);
				}
			}
		}
		
		public void DefaultButtonOnclick()
		{
			defaultButton.onClick.Invoke();
		}

		public void CheckInitSetting()
		{
			currentCityArea = characterManager.WhereIam();
			InitSetting(buy, currentCityArea.assignedAccident.marketItem);
		}
		
		void SetIconImage(string itemName, ItemButtonManager itemButton)
		{
			var iconImage = dataManager.FindItemImage(itemName);
			if (iconImage == null) return;
			
			itemButton.icon.GetComponent<Image>().sprite = iconImage.sprite;
		}

		public void DefaultOn()
		{
			if (defaultButton == null) return;
			defaultButton.animator.Play("Hover to Pressed");
		}
		
		public void ShowOwnedItem()
		{
			var stockCount = sell.marketItemList.Count;
			var newList = new List<Item>();
			if (dataManager.ownedItems.Count > stockCount)
			{
				for (int l = 0; l < stockCount; l++)
				{
					newList.Add(dataManager.ownedItems[l]);
				}
			}
			else
			{
				newList = dataManager.ownedItems;
			}
			
			sell.SetBaseIcon();
			sell.OffAll();
			
			// except memory
			var newOwnedItems = new List<Item>();
			var equipItems = new List<Item>();
			foreach (var item in newList)
			{
				foreach (var character in dataManager.CurrentCharacterList)
				{
					foreach (var equipItem in character.itemList)
					{
						if (item.itemId == equipItem.itemId)
						{
							equipItems.Add(item);
						}
					}
				}
			}
			
			foreach (var item in newList)
			{
				if (item.itemName != "Memory")
				{
					if (!equipItems.Exists(equip => equip.itemId == item.itemId))
					{	
						newOwnedItems.Add(item);
					}
				}
			}
			
			int count = 0;
            count = sell.marketItemList.Count < newOwnedItems.Count ? sell.marketItemList.Count : newOwnedItems.Count;

            int i = 0;
            foreach (var item in newOwnedItems)
            {
	            sell.marketItemList[i].gameObject.SetActive(true);
	            GetMarketText(sell, i).text = newOwnedItems[i].itemName;

	            SetIconImage(item.itemName, sell.marketItemList[i].GetComponent<ItemButtonManager>());
                
	            dataManager.gameManager.Insert(sell.marketItemList[i].GetComponent<ItemButtonManager>(), newOwnedItems[i]);
	            
	            var itemPrice = newOwnedItems[i].itemPrice / discount;
	            GetPriceText(sell, i).text = itemPrice.ToString();
	            i = i + 1;
            }
		}

		void ShowItem(MarketItem type, List<Item> list)
		{
			if (list == null) return;

			buy.SetBaseIcon();
			buy.OffAll();

			int count = 0;
			count = type.marketItemList.Count < list.Count ? type.marketItemList.Count : list.Count;

			int i = 0;
			foreach (var item in list)
			{
				type.marketItemList[i].gameObject.SetActive(true);
				GetMarketText(type, i).text = list[i].itemName;
				GetPriceText(type, i).text = list[i].itemPrice.ToString();
				SetIconImage(item.itemName, type.marketItemList[i].GetComponent<ItemButtonManager>());		
                
				dataManager.gameManager.Insert(type.marketItemList[i].GetComponent<ItemButtonManager>(), list[i]);
				i = i + 1;
			}
		}

		private void InitSetting(MarketItem type, List<Item> testItemList)
		{
			var stockCount = sell.marketItemList.Count;
			var newList = new List<Item>();
			if (currentCityArea.assignedAccident.marketItem.Count > stockCount)
			{
				for (int l = 0; l < stockCount; l++)
				{
					newList.Add(currentCityArea.assignedAccident.marketItem[l]);
				}
			}
			else
			{
				newList = testItemList;
			}
			
			ShowItem(type, newList);
		}

		private void SettingGold(int money)
		{
			dataManager.baseData.currentGold = money;
			dataManager.RenewalGold();
		}
		
		private TextMeshProUGUI GetPriceText(MarketItem market, int index)
		{
			return market.marketItemList[index].GetComponent<ItemButtonManager>().priceText;
		}

		private TextMeshProUGUI GetMarketText(MarketItem market, int index)
		{
			return market.marketItemList[index].GetComponent<ItemButtonManager>().itemNameText;
		}
	}
}
