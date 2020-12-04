using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
    public class EnemyMove : MonoBehaviour {
        private const EaseType BaseEase = EaseType.linear;
        public float moveSpeed = 0.279f;
        public float turnSpeed = 0.0279f;

        private static float waitTime = 2f;
        readonly WaitForSeconds _waiting = new WaitForSeconds(waitTime);

        private GameManager _gameManager;
        Enemy _enemy;
        Board _board;
        Animator _animator;
        CameraController _cameraController;
        private static readonly int Running = Animator.StringToHash("Running");

        void Awake()
        {
            if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
            if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
            if (_board == null) _board = FindObjectOfType<Board>();
            if (_enemy == null) _enemy = GetComponent<Enemy>();
            if (_animator == null) _animator = GetComponent<Animator>();
        }

        void MoveUnit(Vector3 destinationPos, float delayTime = 0f)
        {
            StartCoroutine(MoveRoutine (destinationPos, delayTime));
        }

        List<Vector3> MakeTransform(List<TileNode> way){

            var transformList = new List<Vector3>();

            foreach (var node in way)
            {
                transformList.Add(GameUtility.CoordinateToTransform (node.Coordinate));
            }

            return transformList;
        }
        
        // start
        public void IndicateUnit(List<TileNode> way, AreaOrder areaOrder, WalkOrder walkOrder)
        {
            int spendVigor;
            
            if (areaOrder == AreaOrder.Base)
            {
                spendVigor = _enemy.enemyAi.halfVigor;
            }
            else if (areaOrder == AreaOrder.Double)
            {
                spendVigor = _enemy.enemyAi.fullVigor;
            }
            else 
            {
                spendVigor = _enemy.enemyAi.halfVigor;
            }

            // if (_enemy.cube != null)
            // {
            //     _enemy.cube.transform.position = GameUtility.CoordinateToTransform(way[0].Coordinate); 
            // }
            //
            way.Reverse();
            StartCoroutine(Indicator(MakeTransform (way), spendVigor, walkOrder));
        }

        IEnumerator Indicator(List<Vector3> way, int spendVigor, WalkOrder walkOrder)
        {
            // start
            _enemy.activeState = ActiveState.Moving;
            _cameraController.transform.rotation = _cameraController.currentClock.ClockwiseRotation;
            yield return null;

            if (way == null)
            {
                print("No Way");
                _enemy.activeState = ActiveState.NotAnything;
                _cameraController.useKeyboardInput = true;
                _cameraController.useScreenEdgeInput = true;
            }

            _animator.SetBool(Running, true);

            // do
            if (way != null)
                foreach (var destination in way)
                {
                    MoveUnit(destination);
                    while (_enemy.activeState == ActiveState.Moving)
                    {
                        yield return null;
                    }
                }

            if (walkOrder == WalkOrder.Rush)
            {
                yield return _waiting;
            }
            
            _animator.SetBool(Running, false);

            // end
            _board.ResetBoard();
            _enemy.activeState = ActiveState.NotAnything;
            _gameManager.Marked();
            _enemy.currentVigor -= spendVigor;
        }

        IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime) 
        {
            _enemy.activeState = ActiveState.Moving;

            gameObject.MoveTo(destinationPos, moveSpeed, delayTime, BaseEase);
            gameObject.LookTo(destinationPos, turnSpeed, delayTime, BaseEase);

            while (Vector3.Distance(destinationPos, transform.position) > 0.01f) 
            {
                yield return null;
            }

            iTween.Stop(gameObject);
            transform.position = destinationPos;
        
            _enemy.activeState = ActiveState.NotAnything;
        }
    }
}