using System;
using UnityEngine;

namespace Game.Menu
{
	public class Exit : MonoBehaviour {

		public void Quit()
		{
			Application.Quit();
				
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#endif
			
		}
	}
}
