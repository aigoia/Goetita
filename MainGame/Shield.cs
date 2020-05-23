using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
	public class Shield : MonoBehaviour {

		public Sprite halfShield;
		public Sprite perfectShield;
		
		public bool isShield = false;

		public Dictionary<Vector3, Transform> transformDictionary;
		public Dictionary<Vector3, SpriteRenderer> spriteRendererDict;

		Transform _shieldForward;
		Transform _shieldBack;
		Transform _shieldRight;
		Transform _shieldLeft;

		SpriteRenderer _forwardSpriteRenderer;
		SpriteRenderer _backSpriteRenderer;
		SpriteRenderer _rightSpriteRenderer;
		SpriteRenderer _leftSpriteRenderer;

		public Vector3 halfShieldVector = new Vector3(0, 1f, 0);
		public Vector3 perfectShieldVector = new Vector3(0, 2f, 0);

		void Awake()
		{
			if (_shieldForward == null) _shieldForward =  transform.Find("Shield (0, 0, 1)");
			if (_shieldBack == null) _shieldBack = transform.Find("Shield (0, 0, -1)");
			if (_shieldRight == null) _shieldRight = transform.Find("Shield (1, 0, 0)");
			if (_shieldLeft == null) _shieldLeft = transform.Find("Shield (-1, 0, 0)");

			transformDictionary = new Dictionary<Vector3, Transform>()
			{
				{ Vector3.forward, _shieldForward },
				{ Vector3.back, _shieldBack  },
				{ Vector3.right, _shieldRight  },
				{ Vector3.left, _shieldLeft  },
			};

			if (_forwardSpriteRenderer == null) _forwardSpriteRenderer = _shieldForward.Find("Shield").GetComponent<SpriteRenderer>();
			if (_backSpriteRenderer == null) _backSpriteRenderer = _shieldBack.Find("Shield").GetComponent<SpriteRenderer>();
			if (_rightSpriteRenderer == null) _rightSpriteRenderer = _shieldRight.Find("Shield").GetComponent<SpriteRenderer>();
			if (_leftSpriteRenderer == null) _leftSpriteRenderer = _shieldLeft.Find("Shield").GetComponent<SpriteRenderer>();

			spriteRendererDict = new Dictionary<Vector3, SpriteRenderer>()
			{
				{ Vector3.forward, _forwardSpriteRenderer },
				{ Vector3.back, _backSpriteRenderer  },
				{ Vector3.right, _rightSpriteRenderer  },
				{ Vector3.left, _leftSpriteRenderer  },
			};
		}
	}
}