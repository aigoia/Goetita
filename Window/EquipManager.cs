using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;

namespace Game.Window
{
	public class EquipManager : MonoBehaviour
	{

		public List<ItemButtonManager> itemSlot = new List<ItemButtonManager>();

		private void Awake()
		{
			itemSlot = transform.GetComponentsInChildren<ItemButtonManager>().ToList();
			itemSlot.ForEach(i => i.gameObject.SetActive(false));
		}

		public void ResetAllSlot()
		{
			foreach (var button in itemSlot)
			{
				button.itemId = 0; 
				button.itemNameText.text = "";
				button.priceText.text = "";
				button.priceInt = 0;
				button.itemType = ItemType.Non;
			}
			itemSlot.ForEach(i => i.gameObject.SetActive(false));
		}

		public void ResetSlot(int id)
		{
			var slot = itemSlot.Find(i => i.itemId == id);
			if (slot == null) return;
			slot.itemId = 0;
		}
	}
}
