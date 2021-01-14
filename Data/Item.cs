namespace Game.Data
{
	public enum ItemType
	{
		Non, Weapon, Armor, Underwear, Heal, Fix, Trap, Decoy, Stocking, BlueScreen, Scope, SteamPack,  
	}
	
	[System.Serializable]
	public class Item {
		
		public int itemId;
		public string itemName;
		public int itemPrice;
		public ItemType itemType;

		public Item(int itemId, string itemName, int itemPrice, ItemType itemType)
		{
			this.itemId = itemId;
			this.itemName = itemName;
			this.itemPrice = itemPrice;
			this.itemType = itemType;
		}

		public Item(Item item)
		{
			this.itemId = item.itemId;
			this.itemName = item.itemName;
			this.itemPrice = item.itemPrice;
			this.itemType = item.itemType;
		}
	}
}
