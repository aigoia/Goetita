using System.Collections.Generic;
using Game.Data;
using Game.MainGame;
// using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PostProcessing;
using UnityEngine.UIElements;

namespace Game.Window
{
    public class MainCamera : MonoBehaviour
    {
        public bool useKeyboardInput = true;
        public bool useScreenEdgeInput = true;

        public float keyboardMovementSpeed = 160f;
        public float mouseMovementSpeed = 80f;
        public float screenEdgeMovementSpeed = 160f;

        public float screenEdgeBorder = 40f;
        public float layFloat = 50f;
        
        public KeyCode keyCodeIn = KeyCode.R;
        public KeyCode keyCodeOut = KeyCode.F;
        public int rangeLevel = 2;
        public int cameraZoomOut = 9;
        public int cameraZoomIn = 3;
        public int change = 12;
        public int minCameraSize = 18;
        public int maxCameraSize = 52;
        public Camera thisCamera;
        public Vector3 rotate = new Vector3(60f, 0, 0);

        Rect _leftRect;
        Rect _rightRect;
        Rect _upRect;
        Rect _downRect;
        Vector3 _desiredVector;

        public float xMin = 0;
        public float xMax = 0;
        public float zMin = 0;
        public float zMax = 0;
        
        float verticalWeight = 1;
        float horizontalWeight = 1;

        public DataManager dataManager;
        private readonly Vector3 _bias = new Vector3( -5, 39.5f, -25);

        public LayerMask layerMask;
        
        List<Clockwise> _clockwiseList;
        Clockwise _zeroClock;
        Clockwise _threeClock;
        Clockwise _sixClock;
        Clockwise _nineClock;
        public Clockwise currentClock;
        public bool somethingOn = false;
        // private bool initClick = false;

        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
            
            _desiredVector = new Vector3(horizontalWeight, 0f, verticalWeight);
            _leftRect = new Rect(-1, -1, screenEdgeBorder, Screen.height);
            _rightRect = new Rect(Screen.width - screenEdgeBorder, -1, screenEdgeBorder, Screen.height);
            _upRect = new Rect(-1, Screen.height - screenEdgeBorder + 1, Screen.width, screenEdgeBorder);
            _downRect = new Rect(-1, -1, Screen.width, screenEdgeBorder);
            
            // make clockwise List
            _clockwiseList = new List<Clockwise>();
            _zeroClock = MakeClockwise("zeroClock", 0, GameUtility.ZeroClockRotation + rotate);
            _threeClock = MakeClockwise("threeClock", 1, GameUtility.ThreeClockRotation + rotate);
            _sixClock = MakeClockwise("sixClock", 2, GameUtility.SixClockRotation + rotate);
            _nineClock = MakeClockwise("nineClock", 3, GameUtility.NineClockRotation + rotate);
            _clockwiseList.Add(_zeroClock);
            _clockwiseList.Add(_threeClock);
            _clockwiseList.Add(_sixClock);
            _clockwiseList.Add(_nineClock);
            currentClock = _clockwiseList[0];
        }

        // public void SomethingOn()
        // {
        //     somethingOn = true;
        // }
        //
        // public void SomethingOff()
        // {
        //     somethingOn = false;
        // }
        //
        Clockwise MakeClockwise(string clockName, int index, Vector3 rotation)
        {
        	var newClockwise = new Clockwise
        	{
        		Name = clockName, Index = index, ClockwiseRotation = Quaternion.Euler(rotation)
        	};

        	newClockwise.DesiredQuaternion = Quaternion.Euler(new Vector3(0f, newClockwise.ClockwiseRotation.eulerAngles.y + 45f, 0f));
        	return newClockwise;
        }

        private void Start()
        {
            dataManager.LoadPosition();
            transform.position = dataManager.whereData.currentPosition + _bias;
            
        }

        // void InitClick()
        // {
        //     RotateButton(1);
        //     RotateButton(1);
        //     initClick = true;
        // }

        public void Hungry(bool hungry)
        {
            GetComponent<PostProcessingBehaviour>().profile.colorGrading.enabled = hungry;
        }

        void Update()
        {
            if (somethingOn) return;
            // if (initClick == false) InitClick();
            
            MouseMovement();
            MouseController();
            KeyboardController();
            CheckPos();
            Zoom();
        }

        void MouseMovement()
        {
            if (Input.GetMouseButton(2))
            {
                CameraMovementMouse();
            }

            if (Input.GetMouseButtonDown(1))
            {
                RotateButton(1);
            }
        }

        void RotateButton(int add)
        {
            if (add == 0) return;
        
            int newClockIndex = currentClock.Index + add;
            if  (newClockIndex < 0) newClockIndex = 3;
            else if (newClockIndex > 3) newClockIndex = 0;
            currentClock = _clockwiseList[newClockIndex];
            transform.rotation = currentClock.ClockwiseRotation;
        }
        
        void Zoom()
        {
            // if (_gameManager.somethingOn) return;

            if (Input.GetKeyDown(keyCodeOut))
            {
                if (cameraZoomOut >= maxCameraSize) return;
                CameraOut();
            }
            else if (Input.GetKeyDown(keyCodeIn))
            {
                if (cameraZoomOut <= minCameraSize) return;
                CameraIn();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (cameraZoomOut >= maxCameraSize) return;
                // MovePoint();
                CameraOut();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0) 
            {
                if (cameraZoomOut <= minCameraSize) return;
                // MovePoint();
                CameraIn();
            }
        }

        void MovePoint()
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit rayHit, layFloat, layerMask))
                {
                    print("(" + rayHit.point.x + ", " + rayHit.point.z + ")");
                    
                    transform.position = new Vector3(rayHit.point.x, 40, rayHit.point.y);
                }
            }
        }

        void CameraOut()
        {
            rangeLevel = rangeLevel + 1;
            cameraZoomOut += change;
            thisCamera.orthographicSize = cameraZoomOut;
        }
		
        void CameraIn()
        {
            rangeLevel = rangeLevel - 1;
            cameraZoomOut -= change;
            thisCamera.orthographicSize = cameraZoomOut;
        }
        
        private void CheckPos()
        {
            var position = transform.position;
            position = new Vector3(
                Mathf.Clamp(position.x, xMin, xMax),
                position.y,
                Mathf.Clamp(position.z, zMin, zMax));
            transform.position = position;
        }

        void KeyboardController()
        {
            CameraMovementKeyboard();
            RotateKeyboard();
        }

        void RotateKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateButton(-1);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateButton(1);
            }
        }

        void CameraMovementKeyboard()
        {
            if (currentClock == _zeroClock)
            {
                Movement(KeyboardInputZero);
            }
            else if (currentClock == _threeClock)
            {
                Movement(KeyboardInputThree);
            }
            else if (currentClock == _sixClock)
            {
                Movement(KeyboardInputSix);
            }
            else if (currentClock == _nineClock)
            {
                Movement(KeyboardInputNine);
            }
        }
        
        void CameraMovementMouse()
        {
            if (currentClock == _zeroClock)
            {
                Movement(MouseInputMovementZero);
            }
            else if (currentClock == _threeClock)
            {
                Movement(MouseInputMovementThree);
            }
            else if (currentClock == _sixClock)
            {
                Movement(MouseInputMovementSix);
            }
            else if (currentClock == _nineClock)
            {
                Movement(MouseInputMovementNine);
            }
        }

        void Movement(Vector3 move)
        {
            Vector3 desiredMove = move;
            desiredMove *= keyboardMovementSpeed * Time.deltaTime;
            desiredMove = transform.InverseTransformDirection(desiredMove);
            transform.Translate(desiredMove, Space.Self);
        }

        void MouseController()
        {
            if (!useScreenEdgeInput) return;
            
            ScreenMovement();
        }

        void ScreenMovement()
        {
            Vector3 desiredMove = _desiredVector;
            
            if (currentClock == _zeroClock)
            {
                desiredMove.x *= _leftRect.Contains(MouseInput) ? -1 : _rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z *= _upRect.Contains(MouseInput) ? 1 : _downRect.Contains(MouseInput) ? -1 : 0;
            }
            else if (currentClock == _threeClock)
            {
                desiredMove.z *= _leftRect.Contains(MouseInput) ? 1 : _rightRect.Contains(MouseInput) ? -1 : 0;
                desiredMove.x *= _upRect.Contains(MouseInput) ? 1 : _downRect.Contains(MouseInput) ? -1 : 0;
            }
            else if (currentClock == _sixClock)
            {
                desiredMove.x *= _leftRect.Contains(MouseInput) ? 1 : _rightRect.Contains(MouseInput) ? -1 : 0;
                desiredMove.z *= _upRect.Contains(MouseInput) ? -1 : _downRect.Contains(MouseInput) ? 1 : 0;
            }
            else if (currentClock == _nineClock)
            {
                desiredMove.z *= _leftRect.Contains(MouseInput) ? -1 : _rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.x *= _upRect.Contains(MouseInput) ? -1 : _downRect.Contains(MouseInput) ? 1 : 0;
            }

            if (desiredMove == Vector3.zero) return;
		
            desiredMove *= screenEdgeMovementSpeed * Time.deltaTime;
            desiredMove = transform.InverseTransformDirection (desiredMove);
            transform.Translate(desiredMove, Space.Self);
        }
        
        void CheckPosition()
        {
            var position = transform.position;
            position  = new Vector3(
                Mathf.Clamp(position.x, xMin, xMax),
                position.y,
                Mathf.Clamp(position.z, zMin, zMax));
            transform.position = position;
        }
        
        Vector3 MouseInputMovementZero =>
            new Vector3(
                Input.GetAxis("Mouse X") * horizontalWeight,
                0f,
                Input.GetAxis("Mouse Y") * verticalWeight);
        Vector3 MouseInputMovementThree =>
            new Vector3(
                Input.GetAxis("Mouse Y") * horizontalWeight,
                0f,
                -Input.GetAxis("Mouse X") * verticalWeight);
        Vector3 MouseInputMovementSix =>
            new Vector3(
                -Input.GetAxis("Mouse X") * horizontalWeight,
                0f,
                -Input.GetAxis("Mouse Y") * verticalWeight);
        Vector3 MouseInputMovementNine =>
            new Vector3(
                -Input.GetAxis("Mouse Y") * horizontalWeight,
                0f,
                Input.GetAxis("Mouse X") * verticalWeight);
        
        Vector3 KeyboardInputZero
        {
            get{return useKeyboardInput ? 
                new Vector3(
                    Input.GetAxis("Horizontal") * horizontalWeight, 
                    0f, 
                    Input.GetAxis("Vertical") * verticalWeight) : 
                Vector3.zero;}
        }
        Vector3 KeyboardInputThree
        {
            get{return useKeyboardInput ? 
                new Vector3(
                    Input.GetAxis("Vertical") * horizontalWeight, 
                    0f, 
                    -Input.GetAxis("Horizontal") * verticalWeight) : 
                Vector3.zero;}
        }
        Vector3 KeyboardInputSix
        {
            get{return useKeyboardInput ? 
                new Vector3(
                    -Input.GetAxis("Horizontal") * horizontalWeight, 
                    0f, 
                    -Input.GetAxis("Vertical") * verticalWeight) : 
                Vector3.zero;}
        }
        Vector3 KeyboardInputNine
        {
            get{return useKeyboardInput ? 
                new Vector3(
                    -Input.GetAxis("Vertical") * horizontalWeight, 
                    0f, 
                    Input.GetAxis("Horizontal") * verticalWeight) : 
                Vector3.zero;}
        }

        Vector2 MouseInput
        {
            get{return Input.mousePosition;}
        }
    }
}
