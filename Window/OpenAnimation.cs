using UnityEngine;

namespace Game.Window
{
	public class OpenAnimation : MonoBehaviour
	{

		public GameObject open;
			
		// Use this for initialization
		void Start () {
			open.SetActive(false);
			GetComponent<Animator>().Play(0);
		}

	}
}
