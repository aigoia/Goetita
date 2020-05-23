using UnityEngine;

namespace Game.MainGame
{
	public class TestSpeed : MonoBehaviour {

		public Vector3 destinationPos = new Vector3(14, 0, 104);
		float delayTime = 0f;
		public float moveSpeed = 3f;
		public EaseType baseEase = EaseType.linear ;

		// Use this for initialization
		void Start() {
			gameObject.MoveTo(destinationPos, moveSpeed, delayTime, baseEase);
		}
	}
}
