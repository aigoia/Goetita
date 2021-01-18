using System;
using Game.Data;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace Game.Window
{
    public class MainCamera : MonoBehaviour
    {
        public bool useKeyboardInput = true;
        public bool useScreenEdgeInput = true;
        
        public float keyboardMovementSpeed = 160f;
        public float screenEdgeMovementSpeed = 160f;
        
        public float screenEdgeBorder = 40f;

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

        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
            
            _desiredVector = new Vector3(horizontalWeight, 0f, verticalWeight);
            _leftRect = new Rect(-1, -1, screenEdgeBorder, Screen.height);
            _rightRect = new Rect(Screen.width - screenEdgeBorder, -1, screenEdgeBorder, Screen.height);
            _upRect = new Rect(-1, Screen.height - screenEdgeBorder + 1, Screen.width, screenEdgeBorder);
            _downRect = new Rect(-1, -1, Screen.width, screenEdgeBorder);
        }

        private void Start()
        {
            dataManager.LoadPosition();
            transform.position = dataManager.whereData.currentPosition + _bias;
        }

        public void Hungry(bool hungry)
        {
            GetComponent<PostProcessingBehaviour>().profile.colorGrading.enabled = hungry;
        }

        void Update()
        {
            MouseController();
            KeyboardController();
            CheckPos();
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
            Vector3 desiredMove = KeyboardInput;
            desiredMove *= keyboardMovementSpeed * Time.deltaTime;
            desiredMove = transform.InverseTransformDirection(desiredMove);
            transform.Translate(desiredMove, Space.Self);
        }

        void MouseController()
        {
            if (!useScreenEdgeInput) return;

            Vector3 desiredMove = _desiredVector;
            
            desiredMove.x *= _leftRect.Contains(MouseInput) ? -1 : _rightRect.Contains(MouseInput) ? 1 : 0;
            desiredMove.z *= _upRect.Contains(MouseInput) ? 1 : _downRect.Contains(MouseInput) ? -1 : 0;

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
        
        Vector3 KeyboardInput
        {
            get{return useKeyboardInput ? 
                new Vector3(
                    Input.GetAxis("Horizontal") * horizontalWeight, 
                    0f, 
                    Input.GetAxis("Vertical") * verticalWeight) : 
                Vector3.zero;}
        }
        
        Vector2 MouseInput
        {
            get{return Input.mousePosition;}
        }
    }
}
