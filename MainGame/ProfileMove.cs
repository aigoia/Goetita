using System.Collections;
using UnityEngine;

namespace Game.MainGame
{
	public class ProfileMove : MonoBehaviour {

		Vector3 _leftPosition;
		Vector3 _rightPosition;
		float outX = 400;
		float movingTime = 2f;
	
		Transform _head;
		ProfileSix _headSix;
		Transform _complete;
		ProfileSix _completeSix;
		Transform _inComplete;
		ProfileSix _inCompleteSix;
		WaitForSeconds _waitTime;
		WaitForSeconds _doubleWaitTime;

		void Awake()
		{
			if (_waitTime == null) _waitTime = new WaitForSeconds (movingTime);
			if (_doubleWaitTime == null) _doubleWaitTime = new WaitForSeconds (movingTime*2);
			if (_head == null) _head = transform.Find ("Head");
			if (_headSix == null) _headSix = _head.GetComponent<ProfileSix>();
			if (_complete == null) _complete = transform.Find ("Complete");
			if (_completeSix == null) _completeSix = _complete.GetComponent<ProfileSix>();
			if (_inComplete == null) _inComplete = transform.Find ("InComplete");
			if (_inCompleteSix == null) _inCompleteSix = _inComplete.GetComponent<ProfileSix>();

			_leftPosition = new Vector3 (-outX, this.transform.position.y, 0f);
			_rightPosition = new Vector3 (outX*3.5f, 0, 0f);
		}

		// Use this for initialization
		void Start () 
		{
			StartCoroutine (MakeComplete());
		}

		IEnumerator MakeComplete()
		{
			StartCoroutine (MoveInProfile (_inCompleteSix.blue));
			yield return _waitTime;
			StartCoroutine (MoveOutProfile (_completeSix.blue));
			yield return _waitTime;

			StartCoroutine (MoveInProfile (_inCompleteSix.green));
			yield return _waitTime;
			StartCoroutine (MoveOutProfile (_completeSix.green));
			yield return _waitTime;

			StartCoroutine (MoveInProfile (_inCompleteSix.pink));
			yield return _waitTime;
			StartCoroutine (MoveOutProfile (_completeSix.pink));
			yield return _waitTime;

			StartCoroutine (MoveInProfile (_inCompleteSix.purple));
			yield return _waitTime;
			StartCoroutine (MoveOutProfile (_completeSix.purple));
			yield return _waitTime;

			StartCoroutine (MoveInProfile (_inCompleteSix.white));
			yield return _waitTime;
			StartCoroutine (MoveOutProfile (_completeSix.white));
			yield return _waitTime;

			StartCoroutine (MoveInProfile (_inCompleteSix.red));
			yield return _waitTime;
			StartCoroutine (MoveOutProfile (_completeSix.red));
			yield return _waitTime;
		
			StartCoroutine (FinalMove());
		}

		IEnumerator FinalMove()
		{
			GameObject o;
			(o = _head.gameObject).SetActive (true);
			iTween.MoveFrom (o, _leftPosition, movingTime);
			yield return _waitTime;

			_headSix.blue.gameObject.SetActive (true);
			_headSix.pink.gameObject.SetActive (true);
			_headSix.white.gameObject.SetActive (true);
			_headSix.green.gameObject.SetActive (true);
			_headSix.red.gameObject.SetActive (true);
			_headSix.purple.gameObject.SetActive (true);

			iTween.MoveAdd (_head.gameObject, new Vector3 (0f, -200f, 0f), movingTime);
			iTween.MoveAdd (_headSix.pink.gameObject, new Vector3 (400f, 0f, 0f), movingTime);
			iTween.MoveAdd (_headSix.white.gameObject, new Vector3 (-400f, 0f, 0f), movingTime);
			iTween.MoveAdd (_headSix.blue.gameObject, new Vector3 (-400f, 300f, 0f), movingTime);
			iTween.MoveAdd (_headSix.green.gameObject, new Vector3 (400f, 300f, 0f), movingTime);
			iTween.MoveAdd (_headSix.purple.gameObject, new Vector3 (0f, 300f, 0f), movingTime);
		}

		IEnumerator MoveInProfile (Transform profileSix)
		{
			GameObject o;
			(o = profileSix.gameObject).SetActive (true);
			iTween.MoveFrom (o, _leftPosition, movingTime);
			yield return _waitTime;
			iTween.ScaleTo (profileSix.gameObject, Vector3.zero, movingTime);
			yield return _waitTime;
			_headSix.blue.gameObject.SetActive (false);
		}

		IEnumerator MoveOutProfile (Transform profileSix)
		{
			GameObject o;
			(o = profileSix.gameObject).SetActive (true);
			iTween.ScaleFrom (o, Vector3.zero, movingTime);
			yield return _waitTime;
			iTween.MoveAdd (profileSix.gameObject, _rightPosition, movingTime);
			yield return _waitTime;
			_headSix.blue.gameObject.SetActive (false);
		}
	}
}
