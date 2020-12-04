using System.Collections.Generic;
using Game.Window;
using UnityEngine;

namespace Game.Data
{
	public enum CharacterClass
	{
		Non, Claymore, Ranger
	}

	public class Character
	{
		public int CharacterId;
		public string CharacterName;
		public readonly List<Item> ItemList = new List<Item>();
		public int Level = 0;
		public int Exp = 0;
		public int BaseHp = 0;
		public int CurrentHp = 0;
		public int BaseDeal = 0;
		public CharacterClass Type;

		public Character(int characterId, string characterName, CharacterClass type, int baseHp, int currentHp, int baseDeal, int level, int exp)
		{
			CharacterId = characterId;
			CharacterName = characterName;
			Type = type;
			BaseHp = baseHp;
			CurrentHp = currentHp;
			BaseDeal = baseDeal;
			Level = level;
			Exp = exp;
		}
	}
}
