using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
	public class ImageSetting : MonoBehaviour
	{
		readonly Color32 _baseColor = new Color32(250, 245, 245, 15);
		readonly Color32 _blackColor = new Color32(15, 10, 15, 255);
	
		public void SettingOn()
		{
			transform.GetComponent<Image>().color = _blackColor;
		}

		public void SettingOff()
		{
			transform.GetComponent<Image>().color = _baseColor;
		}
	}
}
