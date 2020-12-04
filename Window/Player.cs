using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Window
{
	public class Player : MonoBehaviour
	{
		public int characterId = 0;
		public string characterName = "Default";
		public GameObject profile = null;
		public readonly List<Item> ItemList = new List<Item>();
		public int level = 0;
		public int baseHp = 0;
		public int currentHp = 0;
		public CharacterClass type = CharacterClass.Non;
	}
}
