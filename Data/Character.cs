using System.Collections.Generic;
using Game.Window;
using UnityEngine;

namespace Game.Data
{
	public enum CharacterClass
	{
		Non, Claymore, Ranger
	}

	[System.Serializable]
	public class Character
	{
		
		public string characterName;
		public List<Item> itemList;
		public int level = 0;
		public int exp = 0;
		public int baseHp = 0;
		public int currentHp = 0;
		public int baseDeal = 0;
		public int plusDeal = 0;
		public int baseArmor = 0;
		public CharacterClass classType;
		public UnitClass unitClass;
		public Trait trait;

		public Character(string characterName, CharacterClass classType, int baseHp, int currentHp, int baseDeal, int plusDeal, int baseArmor, int level, int exp, List<Item> itemList, UnitClass unitClass = UnitClass.One, Trait trait = Trait.Non)
		{
			this.characterName = characterName;
			this.classType = classType;
			this.baseHp = baseHp;
			this.currentHp = currentHp;
			this.baseDeal = baseDeal;
			this.plusDeal = plusDeal;
			this.baseArmor = baseArmor;
			this.level = level;
			this.exp = exp;
			this.itemList = itemList;
			this.unitClass = unitClass;
			this.trait = trait;
		}
	}
}
