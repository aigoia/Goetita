using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
	public class CanvasManager : MonoBehaviour
	{
		public GameObject baseButtons;
		
		private void Awake()
		{
			baseButtons.GetComponent<VerticalLayoutGroup>().enabled = false;
		}
	}
}

