using System;
using UnityEngine;

namespace Game.HideOut
{
	public class CharacterButton : MonoBehaviour
	{
		InventoryManager _inventoryManager;
			
		public int id;

		private void Awake()
		{
			if (_inventoryManager == null) _inventoryManager = FindObjectOfType<InventoryManager>();
		}

		public void ChangeCharacter()
		{
			_inventoryManager.selectedCharacter =
				_inventoryManager.dataManager.currentCharacterList.Find(i => i.CharacterId == id);
			_inventoryManager.SetBicProfile(id);
			_inventoryManager.ChangeSlot(id);
		}
	}
}
