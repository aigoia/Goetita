using UnityEngine;

namespace Game.MainGame
{
	public class ProfileSix : MonoBehaviour {

		public Transform blue;
		public Transform green;
		public Transform pink;
		public Transform purple;
		public Transform white;
		public Transform red;

		void Awake()
		{
			blue = transform.Find("Blue");
			green = transform.Find("Green");
			pink = transform.Find("Pink");
			purple = transform.Find("Purple");
			white = transform.Find("White");
			blue = transform.Find("Blue");
			red = transform.Find("Red");
		}
	}
}
