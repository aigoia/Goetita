using UnityEngine;

namespace Game.HideOut
{
	public class CharacterManager : MonoBehaviour
	{
		public InventoryManager inventoryManager;
			
		public int id;
		
		public void ChangeCharacter()
		{
			inventoryManager.selectedCharacter =
				inventoryManager.characterManager.currentCharacterList.Find(i => i.CharacterId == id);
			inventoryManager.SetBicProfile(id);
			inventoryManager.ChangeSlot(id);
		}
	}
}
