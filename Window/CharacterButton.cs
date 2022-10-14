using Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
	public class CharacterButton : MonoBehaviour
	{
		public InventoryManager inventoryManager;
		public int index;
		
		private void Awake()
		{
			if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();
		}

		public void ChangeCharacter()
		{
			var characters = inventoryManager.dataManager.CurrentCharacterList;

			inventoryManager.selectedCharacter = characters.Find(i =>
				i.characterName == transform.Find("ProfileImage").GetComponent<Image>().sprite.name);
		}
	}
}
