using System.Collections.Generic;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
	public class MarketManager : MonoBehaviour
	{
		public InventoryManager inventoryManager;
		public ItemDataManager itemDataManager;
		public DataManager dataManager;
		
		public int discount = 2;
		public MarketItem buy;
		public MarketItem sell;
		public List<Item> supplyList;

		public int testGold = 1000;
		public Transform inventoryFull;

		public Button defaultButton;

		private void Awake()
		{
			if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
		}

		private void Start()
		{
			// SettingGold(testGold); // Test
			supplyList = new List<Item>();
			dataManager.MakeItemList();
			TestSetting(buy, dataManager.itemList);
		}

		public void DefaultOn()
		{
			if (defaultButton == null) return;
			
			defaultButton.animator.Play("Hover to Pressed");
		}

		public void ShowOwnedItem()
		{
			int count = 0;
            count = sell.marketItemList.Count < dataManager.ownedItems.Count ? sell.marketItemList.Count : dataManager.ownedItems.Count;
            
            for (int i = 0; i < count; i++)
            {
            	sell.marketItemList[i].gameObject.SetActive(true);
            	GetMarketText(sell, i).text = dataManager.ownedItems[i].itemName;
            	GetPriceText(sell, i).text = dataManager.ownedItems[i].itemPrice.ToString();
            	sell.marketItemList[i].GetComponent<ItemButtonManager>().itemId = (int)dataManager.ownedItems[i].itemId;
            	sell.marketItemList[i].GetComponent<ItemButtonManager>().priceInt = (int)dataManager.ownedItems[i].itemPrice;
            }
		}

		void ShowItem(MarketItem type, List<Item> list)
		{
			int count = 0;
			count = type.marketItemList.Count < list.Count ? type.marketItemList.Count : list.Count;
			
			for (int i = 0; i < count; i++)
			{
				type.marketItemList[i].gameObject.SetActive(true);
				GetMarketText(type, i).text = list[i].itemName;
				GetPriceText(type, i).text = list[i].itemPrice.ToString();
				type.marketItemList[i].GetComponent<ItemButtonManager>().itemId = (int)list[i].itemId;
				type.marketItemList[i].GetComponent<ItemButtonManager>().priceInt = (int)list[i].itemPrice;
			}
		}

		private void TestSetting(MarketItem type, List<Item> testItemList)
		{
			ShowItem(type, testItemList);
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
