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
		public int characterId;
		public string characterName;
		public List<Item> itemList;
		public int level = 0;
		public int exp = 0;
		public int baseHp = 0;
		public int currentHp = 0;
		public int baseDeal = 0;
		public CharacterClass type;

		public Character(int characterId, string characterName, CharacterClass type, int baseHp, int currentHp, int baseDeal, int level, int exp, List<Item> itemList)
		{
			this.characterId = characterId;
			this.characterName = characterName;
			this.type = type;
			this.baseHp = baseHp;
			this.currentHp = currentHp;
			this.baseDeal = baseDeal;
			this.level = level;
			this.exp = exp;
			this.itemList = itemList;
		}
		
	}
}
