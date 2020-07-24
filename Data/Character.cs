using System.Collections.Generic;
using Game.HideOut;
using UnityEngine;

namespace Game.Data
{
	public class Character
	{
		public readonly int CharacterId;
		public readonly string CharacterName;
		public readonly GameObject Profile;
		public readonly List<Item> ItemList = new List<Item>();

		public Character(int characterId, string characterName, GameObject profile)
		{
			CharacterId = characterId;
			CharacterName = characterName;
			Profile = profile;

		}
	}
}
