using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.MainGame;
using UnityEngine;

namespace Game.Window
{
	public class ButtonManager : MonoBehaviour
	{
		public List<ItemButtonManager> itemSlot = new List<ItemButtonManager>();
		
		private void Awake()
		{
			itemSlot.ForEach(i => i.gameObject.SetActive(false));
		}
		
		public void Start()
		{
			
		}

		[ContextMenu("Setting")]
		public void Setting()
		{
			itemSlot = transform.GetComponentsInChildren<ItemButtonManager>().ToList();
			foreach (var item in itemSlot)
			{
				item.SetAct();
			}
		}

		public void ResetAllSlot()
		{
			// print(itemSlot);
			foreach (var button in itemSlot)
			{
				button.itemId = 0; 
				button.itemNameText.text = "";
				button.priceText.text = "";
				button.priceInt = 0;
				button.itemType = ItemType.Non;
				button.detailType = CharacterClass.Non;
				button.itemConsumable = ItemConsumable.Non;
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
