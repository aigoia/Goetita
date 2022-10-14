using UnityEngine;
using UnityEngine.UI;

namespace Game.Data
{
	public enum ItemType
	{
		Non, Weapon, Armor, Underwear, BlueScreen, SteamPack, Memory, 
	}

	public enum ItemConsumable
	{
		Non, Equip, Consume,
	}

	public enum Trait
	{
		Non, ArmorPiercing, Psionic,
	}
	
	[System.Serializable]
	public class Item {
		
		public int itemId;
		public string itemName;
		public int itemPrice;
		public ItemType itemType;
		public CharacterClass detailType;
		public ItemConsumable itemConsumable;
		public int baseInt;
		public int plusInt;
		public Trait trait;
		public ItemGrade itemGrade;
		

		public Item(int itemId, string itemName, int itemPrice, ItemType itemType, CharacterClass detailType, ItemConsumable itemConsumable, int baseInt, int plusInt, Trait trait, ItemGrade itemGrade)
		{
			this.itemId = itemId;
			this.itemName = itemName;
			this.itemPrice = itemPrice;
			this.itemType = itemType;
			this.detailType = detailType;
			this.itemConsumable = itemConsumable;
			this.baseInt = baseInt;
			this.plusInt = plusInt;
			this.trait = trait;
			this.itemGrade = itemGrade;
		}

		public Item(Item item)
		{
			this.itemId = item.itemId;
			this.itemName = item.itemName;
			this.itemPrice = item.itemPrice;
			this.itemType = item.itemType;
			this.detailType = item.detailType;
			this.itemConsumable = item.itemConsumable;
			this.baseInt = item.baseInt;
			this.plusInt = item.plusInt;
			this.trait = item.trait;
			this.itemGrade = item.itemGrade;

		}
	}
	
}
