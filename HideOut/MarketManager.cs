using System;
using System.Collections.Generic;
using Game.Data;
using TMPro;
using UnityEngine;

namespace Game.HideOut
{
	public class MarketManager : MonoBehaviour
	{
		public InventoryManager inventoryManager;
		public ItemDataManager itemDataManager;
		public DataManager dataManager;
		
		public int discount = 2;
		public MarketItem buy;
		public MarketItem sell;
		public List<Item> ownedItems = new List<Item>();
		
		public List<Item> testItemList;
		public List<Item> supplyList;

		public int testGold = 1000;
		public Transform inventoryFull;


		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
		}

		private void Start()
		{
			SettingGold(testGold); // Test
			supplyList = new List<Item>();
			TestSetting(buy);
		}

		public void ShowItem(MarketItem type, List<Item> list)
		{
			int count = 0;
			count = type.marketItemList.Count < list.Count ? type.marketItemList.Count : list.Count;
			
			for (int i = 0; i < count; i++)
			{
				type.marketItemList[i].gameObject.SetActive(true);
				GetMarketText(type, i).text = list[i].ItemName;
				GetPriceText(type, i).text = list[i].ItemPrice.ToString();
				type.marketItemList[i].GetComponent<ItemButtonManager>().itemId = (int)list[i].ItemId;
				type.marketItemList[i].GetComponent<ItemButtonManager>().priceInt = (int)list[i].ItemPrice;
			}
		}

		private void TestSetting(MarketItem type)
		{
			testItemList = new List<Item>()
			{
				new Item (1, "Heavy",100),
				new Item (2, "Light",120),
				new Item (3, "Heavy",100),
				new Item (4, "Light",120),
				new Item (5, "Heavy",100),
				new Item (6, "Light",120),
				new Item (7, "Heavy",100),
				new Item (8, "Light",120),
				new Item (9, "Heavy",100),
				new Item (10, "Light",120),
				new Item (11, "Heavy",100),
				new Item (12, "Light",120),
				new Item (13, "Heavy",100),
				new Item (14, "Light",120),
				new Item (15, "Heavy",100),
				new Item (16, "Light",120),
				new Item (17, "Heavy",100),
				new Item (18, "Light",120),
				new Item (19, "Heavy",100),
				new Item (20, "Light",120),
				new Item (21, "Heavy",100),
				new Item (22, "Light",120),
				new Item (23, "Heavy",100),
				new Item (24, "Light",120),
				new Item (25, "Heavy",100),
				new Item (26, "Light",120),
				new Item (27, "Heavy",100),
				new Item (28, "Light",120),
			};
			
			ShowItem(type, testItemList);
		}

		private void SettingGold(int money)
		{
			dataManager.gold = money;
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
