using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
    public class EnemyMove : MonoBehaviour {
        
        // private const EaseType BaseEase = EaseType.linear;
        public float moveNormalSpeed = 0.28f;
        public float moveSlowSpeed = 0.30f;
        public float turnSpeed = 0.0f;

        static readonly float waitTime = 2f;
        readonly WaitForSeconds _waiting = new WaitForSeconds(waitTime);

        GameManager _gameManager;
        Enemy _enemy;
        Board _board;
        Animator _animator;
        CameraController _cameraController;
        static readonly int Running = Animator.StringToHash("Running");

        void Awake()
        {
            if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
            if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
            if (_board == null) _board = FindObjectOfType<Board>();
            if (_enemy == null) _enemy = GetComponent<Enemy>();
            if (_animator == null) _animator = GetComponent<Animator>();
        }

        // entrance
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

            var wayCount = 0;
            if (way != null)
            {
                var nCount = way.Count - 1;
                // do
                foreach (var destination in way)
                {
                    if (wayCount == 0)
                    {
                        MoveUnit(destination, moveSlowSpeed,0, EaseType.easeInSine);    
                    }
                    else if (wayCount == nCount)
                    {
                        MoveUnit(destination, moveSlowSpeed,0, EaseType.easeOutSine);
                    }
                    else
                    {
                        MoveUnit(destination, moveNormalSpeed,0, EaseType.linear);
                    }
                    
                    while (_enemy.activeState == ActiveState.Moving)
                    {
                        yield return null;
                    }

                    wayCount += 1;
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

        IEnumerator MoveRoutine(Vector3 destinationPos, float moveSpeed, float delayTime, EaseType moveType = EaseType.linear) 
        {
            _enemy.activeState = ActiveState.Moving;

            gameObject.MoveTo(destinationPos, moveSpeed, delayTime, moveType);
            gameObject.LookTo(destinationPos, turnSpeed, delayTime, moveType);

            while (Vector3.Distance(destinationPos, transform.position) > 0.01f) 
            {
                yield return null;
            }

            iTween.Stop(gameObject);
            transform.position = destinationPos;
        
            // _gameManager.SingleMarked(_enemy);
            
            _enemy.activeState = ActiveState.NotAnything;
        }
        
        void MoveUnit(Vector3 destinationPos, float moveSpeed, float delayTime, EaseType moveType)
        {
            StartCoroutine(MoveRoutine (destinationPos, moveSpeed, delayTime, moveType));
        }

        List<Vector3> MakeTransform(List<TileNode> way){

            var transformList = new List<Vector3>();

            foreach (var node in way)
            {
                transformList.Add(GameUtility.CoordinateToTransform (node.Coordinate));
            }

            return transformList;
        }
    }
}