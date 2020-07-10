using UnityEngine;

namespace Game.HideOut
{
	public class Item {
		
		public readonly int ItemId;
		public readonly string ItemName;
		public readonly int ItemPrice;

		public Item(int itemId, string itemName, int itemPrice)
		{
			ItemId = itemId;
			ItemName = itemName;
			ItemPrice = itemPrice;
		}

		public Item(Item item)
		{
			ItemId = item.ItemId;
			ItemName = item.ItemName;
			ItemPrice = item.ItemPrice;
		}
	}
}
