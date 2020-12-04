using Game.Data;
using UnityEngine;

namespace Game.Window
{
	public class CharacterButton : MonoBehaviour
	{
		public InventoryManager inventoryManager;
		public int characterId;
		public Character character;

		private void Awake()
		{
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
		}

		public void ChangeCharacter()
		{
			inventoryManager.selectedCharacter =
				inventoryManager.dataManager.currentCharacterList.Find(i => i.CharacterId == characterId);
			// _inventoryManager.SetBicProfile(id);
			inventoryManager.ChangeSlot(characterId);
			// print(inventoryManager.selectedCharacter.CharacterName);
			
			inventoryManager.SetInformation();
		}
	}
}
