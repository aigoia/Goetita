using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
	public class Sign : MonoBehaviour {
		
		public bool isSign;
    
		public Dictionary<Vector3, Transform> transformDictionary;
		public Dictionary<Vector3, SpriteRenderer> spriteRendererDict;

		Transform _signForward;
		Transform _signBack;
		Transform _signRight;
		Transform _signLeft;
		Transform _signForwardArrow;
		Transform _signBackArrow;
		Transform _signRightArrow;
		Transform _signLeftArrow;
    
		SpriteRenderer _forwardSpriteRenderer;
		SpriteRenderer _backSpriteRenderer;
		SpriteRenderer _rightSpriteRenderer;
		SpriteRenderer _leftSpriteRenderer;
		SpriteRenderer _forwardArrowSpriteRenderer;
		SpriteRenderer _backArrowSpriteRenderer;
		SpriteRenderer _rightArrowSpriteRenderer;
		SpriteRenderer _leftArrowSpriteRenderer;
		
		public Vector3 origin = new Vector3(0, 1f, 0);
		
		void Awake()
		{
			if (_signForward == null) _signForward =  transform.Find("Sign (0, 0, 1)");
			if (_signBack == null) _signBack = transform.Find("Sign (0, 0, -1)");
			if (_signRight == null) _signRight = transform.Find("Sign (1, 0, 0)");
			if (_signLeft == null) _signLeft = transform.Find("Sign (-1, 0, 0)");
			if (_signForwardArrow == null) _signForwardArrow =  transform.Find("Sign (1, 0, 1)");
			if (_signBackArrow == null) _signBackArrow = transform.Find("Sign (1, 0, -1)");
			if (_signRightArrow == null) _signRightArrow = transform.Find("Sign (-1, 0, 1)");
			if (_signLeftArrow == null) _signLeftArrow = transform.Find("Sign (-1, 0, -1)");

			transformDictionary = new Dictionary<Vector3, Transform>()
			{
				{ Vector3.forward, _signForward },
				{ Vector3.back, _signBack },
				{ Vector3.right, _signRight },
				{ Vector3.left, _signLeft },
				{ new Vector3(1, 0, 1), _signForwardArrow },
				{ new Vector3(1, 0, -1), _signBackArrow },
				{ new Vector3(-1, 0, 1), _signRightArrow },
				{ new Vector3(-1, 0, -1), _signLeftArrow },
			};

			if (_forwardSpriteRenderer == null) _forwardSpriteRenderer = _signForward.Find("Sign").GetComponent<SpriteRenderer>();
			if (_backSpriteRenderer == null) _backSpriteRenderer = _signBack.Find("Sign").GetComponent<SpriteRenderer>();
			if (_rightSpriteRenderer == null) _rightSpriteRenderer = _signRight.Find("Sign").GetComponent<SpriteRenderer>();
			if (_leftSpriteRenderer == null) _leftSpriteRenderer = _signLeft.Find("Sign").GetComponent<SpriteRenderer>();
			if (_forwardArrowSpriteRenderer == null) _forwardArrowSpriteRenderer = _signForward.Find("Sign").GetComponent<SpriteRenderer>();
			if (_backArrowSpriteRenderer == null) _backArrowSpriteRenderer = _signBack.Find("Sign").GetComponent<SpriteRenderer>();
			if (_rightArrowSpriteRenderer == null) _rightArrowSpriteRenderer = _signRight.Find("Sign").GetComponent<SpriteRenderer>();
			if (_leftArrowSpriteRenderer == null) _leftArrowSpriteRenderer = _signLeft.Find("Sign").GetComponent<SpriteRenderer>();
    
			spriteRendererDict = new Dictionary<Vector3, SpriteRenderer>()
			{
				{ Vector3.forward, _forwardSpriteRenderer },
				{ Vector3.back, _backSpriteRenderer },
				{ Vector3.right, _rightSpriteRenderer },
				{ Vector3.left, _leftSpriteRenderer },
				{ new Vector3(1, 0, 1), _forwardArrowSpriteRenderer },
				{ new Vector3(1, 0, -1), _backArrowSpriteRenderer },
				{ new Vector3(-1, 0, 1), _rightArrowSpriteRenderer },
				{ new Vector3(-1, 0, -1), _leftArrowSpriteRenderer },
			};
		}
	}
}
