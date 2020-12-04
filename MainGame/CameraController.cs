using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.MainGame
{
	public class CameraController : MonoBehaviour
	{
		public Transform mainCanvas;
		public int cameraZoomOut = 9;
		public int cameraZoomIn = 3;
		readonly int change = 6;
		readonly int minCameraSize = 3;
		readonly int maxCameraSize = 15;

		[SerializeField] float keyboardMovementSpeed = 20f;
		[SerializeField] float screenEdgeMovementSpeed = 20f;
		[SerializeField] float screenEdgeBorder = 40f;

		public bool cameraOn = true;
		public bool useKeyboardInput = true;
		public bool useRotate = true;
		public bool useMove = true;
		public bool useScreenEdgeInput = true;
		public bool cameraMoving = false;

		Transform _thisTransform;

		Rect _leftRect;
		Rect _rightRect;
		Rect _upRect;
		Rect _downRect;
		Vector3 _desiredVector;

		float verticalWeight = 2;
		float horizontalWeight = 1;
		public Camera thisCamera;
		public int area = 20;

		List<Clockwise> _clockwiseList;
		Clockwise _zeroClock;
		Clockwise _threeClock;
		Clockwise _sixClock;
		Clockwise _nineClock;
		public Clockwise currentClock;

		GameManager _gameManager;
		public Transform hp;

		public KeyCode keyCodeIn = KeyCode.R;
		public KeyCode keyCodeOut = KeyCode.F;

		private readonly Vector3 _rangeDown = new Vector3(10, -93, 0);
		private readonly Vector3 _rangeMiddle = new Vector3(10, -93, 0);
		private readonly Vector3 _rangeUp = new Vector3(10, -118, 0);
		private List<Vector3> _rangeVectorList;
		public int rangeLevel = 2;

		public Board board;
		float _xMin = 0;
		float _xMax = 0;
		float _zMin = 0;
		float _zMax = 0;
		public int limit = 4;

		void Awake()
		{
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>(); 
			if (mainCanvas == null) mainCanvas = FindObjectOfType<MainCanvas>().transform;
			if (board == null) board = FindObjectOfType<Board>();
			
			// boundary
			_xMin = GameUtility.interval * limit;
			_xMax = GameUtility.interval * (board.boardSize.x - limit);
			_zMin = GameUtility.interval * limit;
			_zMax = GameUtility.interval * (board.boardSize.y - limit);
			
			// make clockwise List
			_clockwiseList = new List<Clockwise>();
			_zeroClock = MakeClockwise("zeroClock", 0, GameUtility.ZeroClockRotation);
			_threeClock = MakeClockwise("threeClock", 1, GameUtility.ThreeClockRotation);
			_sixClock = MakeClockwise("sixClock", 2, GameUtility.SixClockRotation);
			_nineClock = MakeClockwise("nineClock", 3, GameUtility.NineClockRotation);
			_clockwiseList.Add(_zeroClock);
			_clockwiseList.Add(_threeClock);
			_clockwiseList.Add(_sixClock);
			_clockwiseList.Add(_nineClock);
			currentClock = _clockwiseList[0];

			// make screen edge
			_desiredVector = new Vector3(horizontalWeight, 0f, verticalWeight);
			_leftRect = new Rect(-1, -1, screenEdgeBorder, Screen.height);
			_rightRect = new Rect(Screen.width - screenEdgeBorder, -1, screenEdgeBorder, Screen.height);
			_upRect = new Rect(-1, Screen.height - screenEdgeBorder + 1, Screen.width, screenEdgeBorder);
			_downRect = new Rect(-1, -1, Screen.width, screenEdgeBorder);

			_thisTransform = transform;
			if (thisCamera == null) thisCamera = transform.Find("Camera").GetComponent<Camera>();

			_rangeVectorList = new List<Vector3> {_rangeDown, _rangeMiddle, _rangeUp};
		}

		private void Start()
		{
			CameraOut();
			CameraIn();
		}

		void Update()
		{
			Zoom();
			MouseController();
			KeyboardController();
			CheckPosition();
		}

		private void CheckPosition()
		{
			var position = transform.position;
			position = new Vector3(
				Mathf.Clamp(position.x, _xMin, _xMax),
				position.y,
				Mathf.Clamp(position.z, _zMin, _zMax));
			transform.position = position;
		}

		void Zoom()
		{
			if (_gameManager.somethingOn) return;

			if (Input.GetKeyDown(keyCodeOut))
			{
				if (cameraZoomOut == maxCameraSize) return;
				CameraOut();
			}
			else if (Input.GetKeyDown(keyCodeIn))
			{
				if (cameraZoomOut == minCameraSize) return;
				CameraIn();
			}

			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				if (cameraZoomOut == maxCameraSize) return;
				CameraOut();
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0) 
			{
				if (cameraZoomOut == minCameraSize) return;
				CameraIn();
			}
		}

		void CameraOut()
		{
			rangeLevel = rangeLevel + 1;
			cameraZoomOut += change;
			thisCamera.orthographicSize = cameraZoomOut;
			var localScale = hp.localScale;
			int newInt  = (int)(((localScale.x) / 3) * 2);
			localScale -= Vector3.one * newInt;
			hp.localScale = localScale;
		}
		
		void CameraIn()
		{
			rangeLevel = rangeLevel - 1;
			cameraZoomOut -= change;
			thisCamera.orthographicSize = cameraZoomOut;
			var localScale = hp.localScale;
			localScale += (localScale.x) * (localScale.x) * Vector3.one;
			hp.localScale = localScale;
		}

		Clockwise MakeClockwise(string clockName, int index, Vector3 rotation)
		{
			var newClockwise = new Clockwise
			{
				Name = clockName, Index = index, ClockwiseRotation = Quaternion.Euler(rotation)
			};

			newClockwise.DesiredQuaternion = Quaternion.Euler(new Vector3(0f, newClockwise.ClockwiseRotation.eulerAngles.y + 45f, 0f));
			return newClockwise;
		}

		void KeyboardController()
		{
			if (!useKeyboardInput) return;
			if (_gameManager.somethingOn) return;

			MoveScreen();
			RotateButton(KeyboardRotate());
		}

		void MoveScreen()
		{
			if (!useMove) return;
			if (currentClock == null) return;

			Vector3 desiredMove = KeyboardInput;
			desiredMove *= keyboardMovementSpeed * Time.deltaTime;
			desiredMove = currentClock.DesiredQuaternion * desiredMove;
			desiredMove = _thisTransform.InverseTransformDirection(desiredMove);
			_thisTransform.Translate(desiredMove, Space.Self);
		}

		public void RotateButton(int add)
		{
			if (!useRotate) return;
			if (add == 0) return;

			int newClockIndex = currentClock.Index + add;
			if  (newClockIndex < 0) newClockIndex = 3;
			else if (newClockIndex > 3) newClockIndex = 0;
			currentClock = _clockwiseList[newClockIndex];
			transform.rotation = currentClock.ClockwiseRotation;
		}

		public void CloseCamera(Vector3 objectPos, bool isClosed = false, bool close = true)
		{
			if (!isClosed) return;

			thisCamera.orthographicSize = close ? cameraZoomIn : cameraZoomOut;
			transform.position = objectPos;
		}

		public void FollowCamera(Vector3 objectPos, bool isFollow = false)
		{
			if (!isFollow) return;

			transform.position = objectPos;
		}
		
		public void CameraMoveKey(bool isKey = false, bool state = false)
		{
			if (!isKey) return;

			useMove = state;
			useScreenEdgeInput = state;
		}

		void MouseController()
		{
			if (!useScreenEdgeInput) return;
			if (!useMove) return;
			if (_gameManager.somethingOn) return;

			Vector3 desiredMove = _desiredVector;

			desiredMove.x *= _leftRect.Contains(MouseInput) ? -1 : _rightRect.Contains(MouseInput) ? 1 : 0;
			desiredMove.z *= _upRect.Contains(MouseInput) ? 1 : _downRect.Contains(MouseInput) ? -1 : 0;

			if (desiredMove == Vector3.zero) return;
			if (desiredMove == null) return;
		
			desiredMove *= screenEdgeMovementSpeed * Time.deltaTime;
			desiredMove = currentClock.DesiredQuaternion * desiredMove;
			desiredMove = _thisTransform.InverseTransformDirection (desiredMove);
			_thisTransform.Translate(desiredMove, Space.Self);
		}

		int KeyboardRotate()
		{
			if (Input.GetButtonDown("Clockwise"))
			{
				return 1;
			}
			else if (Input.GetButtonDown("AntiClockwise"))
			{
				return -1;
			}

			return 0;
		}
		
		Vector3 KeyboardInput =>
			useKeyboardInput ? 
				new Vector3(
					Input.GetAxis("Horizontal") * horizontalWeight, 
					0f, 
					Input.GetAxis("Vertical") * verticalWeight) 
				: Vector3.zero;

		Vector2 MouseInput => Input.mousePosition;
	}

	public class Clockwise 
	{
		public string Name = "default";
		public int Index  = 0;

		public Quaternion ClockwiseRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		public Quaternion DesiredQuaternion = Quaternion.Euler(new Vector3(0f, 45f, 0f));
	}
}