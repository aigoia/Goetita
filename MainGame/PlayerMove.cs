﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
    public class PlayerMove : MonoBehaviour {

        public bool isClosed = false;
        public bool isFollow = false;
        public GameObject destinationPoint;
        private readonly EaseType _baseMoveType = EaseType.linear ;

        public float moveSpeed = 0.279f;
        public float turnSpeed = 0.0279f;

        Player _player;
        Animator _animator;
        GameManager _gameManager;
        Board _board;
        CameraController _cameraController;
        private Sign _sword;
        
        MainCanvas _canvas;
        private static readonly int Running = Animator.StringToHash("Running");

        void Awake()
        {
            if (_board == null) _board = FindObjectOfType<Board>();
            if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
            if (_canvas == null) _canvas = _gameManager.GetComponent<UiManager>().canvas.GetComponent<MainCanvas>();
            if (_player == null) _player = GetComponent<Player>();
            if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
            if (_animator == null) _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (_sword == null) _sword = _gameManager.transform.Find("Sword").GetComponent<Sign>();
        }

        void MoveUnit (Vector3 destinationPos, float delayTime = 0f)
        {
            StartCoroutine(MoveRoutine(destinationPos, delayTime));
        }

        private List<Vector3> MakeTransform (List<TileNode> way){

            var transformList = new List<Vector3>();

            foreach (var node in way)
            {
                transformList.Add(GameUtility.CoordinateToTransform(node.Coordinate));
            }

            return transformList;
        }

        // start
        public void IndicateUnit(List<TileNode> way, bool skill = false)
        {
            way.Reverse();
            if (_gameManager.currentPlayer == null) return;

            StartCoroutine(Indicator(MakeTransform(way), skill));
        }

        void StartIndicator(bool skill)
        {
            _board.line.positionCount = 0;
            _gameManager.round.position = _gameManager.roundVector;
            _gameManager.ResetClose();
            _player.activeState = ActiveState.Moving;
            _player.PlayerOff();
            
            foreach (var item in _gameManager.activePlayerList)
            {
                item.canSelect = false;
            }

            if (isClosed) _canvas.CanvasOff();

            _cameraController.CloseCamera(transform.position, isClosed, true);
            _cameraController.CameraMoveKey(isClosed, false);
            _cameraController.CameraMoveKey(isFollow, false);

            _gameManager.somethingOn = true;
        }

        void EndIndicator(bool skill)
        {
            _player.activeState = ActiveState.NotAnything;

            foreach (var item in _gameManager.activePlayerList)
            {
                item.canSelect = true;
            }

            _cameraController.CloseCamera(transform.position, isClosed, false);
            _cameraController.CameraMoveKey(isClosed, true);
            _cameraController.CameraMoveKey(isFollow, true);
            _board.ResetBoard();
            
            if (skill == false)
            {
                if (isClosed) _canvas.CanvasOn();
                _gameManager.somethingOn = false;
            }
        }

        IEnumerator Indicator(List<Vector3> way, bool skill)
        {
            // start
            StartIndicator(skill);
            var nCount = way.Count - 1;
            // GameObject destinationFlag = new GameObject();

            // if (nCount >= 1)
            // {
            //     destinationFlag = Instantiate(destinationPoint, way[nCount], Quaternion.identity);    
            // }
            

            yield return null;

            _animator.SetBool(Running, true);
            var waysCount = way.Count;

            // do
            _board.ResetBoundary();
            foreach (var destination in way)
            {
                MoveUnit(destination);

                // camera move
                while (_player.activeState == ActiveState.Moving) 
                {                
                    yield return null;

                    var position = transform.position;
                    _cameraController.CloseCamera(position, isClosed, true);
                    _cameraController.FollowCamera(position, isFollow);
                }
            }

            _animator.SetBool(Running, false);

            yield return null;

            // end
            EndIndicator(skill);
            // if (nCount >= 1)
            // {
            //      DestroyImmediate(destinationFlag);
            // }
            
            if (skill == false) 
            {
                if ( _player.currentVigor > 0 )
                {
                    _gameManager.SelectPlayer (_player);
                }
                
                _gameManager.PlayerTurnEnding();
                if (_gameManager.currentPlayer != null) _gameManager.currentPlayer.CloseAttackUiCheck();
            }
        }

        IEnumerator MoveRoutine (Vector3 destinationPos, float delayTime) 
        {   
            _player.activeState = ActiveState.Moving;

            yield return new WaitForSeconds (delayTime);

            gameObject.MoveTo (destinationPos, moveSpeed, delayTime, _baseMoveType);
            gameObject.LookTo (destinationPos, turnSpeed, delayTime, _baseMoveType);
            
            while (Vector3.Distance(destinationPos, transform.position) > 0.01f) 
            {
                yield return null;
            }

            iTween.Stop(gameObject);
            transform.position = destinationPos;
            _player.activeState = ActiveState.NotAnything;
        }
    }
}
