using UnityEngine;

namespace Game.MainGame
{
	public class Bar : MonoBehaviour {

		void Start ()
		{
			if (Camera.main != null) this.transform.forward = Camera.main.transform.forward;
		}
	}
}
