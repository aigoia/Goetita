using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Window
{
	public class EquipManager : MonoBehaviour {

		public List<ItemButtonManager> itemSlot = new List<ItemButtonManager>();

		private void Awake()
		{
			itemSlot = transform.GetComponentsInChildren<ItemButtonManager>().ToList();
			itemSlot.ForEach(i => i.gameObject.SetActive(false));
		}
	}
}
