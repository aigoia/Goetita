using UnityEngine;

namespace Game.MainGame
{
	public class IconManager : MonoBehaviour
	{
		float delay = 0.2f;
		public GameObject highlighted;

		private void Start()
		{
			SettingIcon();
		}

		public void SettingIcon()
		{
			Invoke(nameof(MakeTrue), delay);
		}

		private void MakeTrue()
		{
			highlighted.SetActive(true);
		}
	}
}
