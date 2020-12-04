using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.MainGame
{
	public enum SkillState
	{
		Non, BaseCloseAttack,
	}

	public enum BoundaryDirection
	{
		Default, Zero, Three, Six, Nine, 
	}

	public enum BoundaryArea
	{
		Default, OneBlock, TowBlock, NonWalkable,
	}

	public enum TileStyle
	{
		Normal, OneArea, TwoArea, NonWalkable, Test, BaseRange, Range, HideOneArea, HideTowArea,
	}


	public enum ShieldStyle
	{
		Default, HalfShield, PerfectShield, Block,
	}

	public class TileNode : MonoBehaviour {

		Action _useSkill;
		public float distance;
		// public Player targetPlayer;

		public float farFormTarget;
		public BoundaryArea areaBoundary = BoundaryArea.Default;
		public BoundaryDirection boundaryDirection = BoundaryDirection.Default;
		public TileStyle tileStyle = TileStyle.Normal;
		public ShieldStyle shieldStyle = ShieldStyle.Default;
		public SkillState skillState = SkillState.Non;
		public Player tilePlayer;
		public Enemy tileEnemy;

		public Vector2 Coordinate{set; get;}

		PlayerMove _moving;
		GameManager _gameManager;
		CameraController _cameraController;
		Board _board;
		Shield _shield;
		Sign _sword;

		readonly Vector3 _drawVector = new Vector3(1.8f, 0.1f, 1.8f);
		float checkRange = 2f;
		
		void Awake()
		{
			if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (_board == null) _board = FindObjectOfType<Board>();
			if (_shield == null) _shield = FindObjectOfType<Shield>();
			if (_sword == null) _sword = _gameManager.transform.Find("Sword").GetComponent<Sign>();
		}

		void Start()
		{
			if (tileStyle == TileStyle.NonWalkable) return;
			MakeShieldMap();
		}
		
		void OnDrawGizmos()
		{	
			ColorPick();
			ShieldPick();
			Gizmos.DrawCube(transform.position, _drawVector);
		}

		void ShieldPick()
		{
			if (shieldStyle == ShieldStyle.HalfShield)
			{
				Gizmos.color = Color.blue;
			}
			else if (shieldStyle == ShieldStyle.PerfectShield)
			{
				Gizmos.color = Color.green;
			}
			else if (shieldStyle == ShieldStyle.Block)
			{
				Gizmos.color = Color.cyan;
			}
		}

		void ColorPick()
		{
			if (tileStyle == TileStyle.Normal)
			{
				Gizmos.color = Color.black;
			}
			else if (tileStyle == TileStyle.NonWalkable)
			{
				Gizmos.color = Color.red;
			}
			else if (tileStyle == TileStyle.OneArea)
			{
				Gizmos.color = Color.white;
			}
			else if (tileStyle == TileStyle.TwoArea)
			{
				Gizmos.color = Color.grey;
			}
			else if (tileStyle == TileStyle.Test)
			{
				Gizmos.color = Color.yellow;
			}
		}

		void MakeShieldMap()
		{
			foreach (var direction in GameUtility.Directions)
			{
				if (Physics.Raycast(transform.position + _shield.halfShieldVector, direction, checkRange, LayerMask.GetMask("Obstacle")))
				{
					if (Physics.Raycast(transform.position + _shield.perfectShieldVector, direction, checkRange, LayerMask.GetMask("Obstacle")))
					{
						this.shieldStyle = ShieldStyle.PerfectShield;
					}
					else
					{
						this.shieldStyle = ShieldStyle.HalfShield;
					}
				}
			}
		}

		void MakeShield()
		{
			if (tileStyle == TileStyle.NonWalkable) return;
			if (tileStyle == TileStyle.Normal) return;

			foreach (var direction in GameUtility.Directions)
			{
				if (Physics.Raycast(transform.position + _shield.halfShieldVector, direction, checkRange, LayerMask.GetMask("Obstacle")))
				{
					_shield.transformDictionary [direction].position = transform.position;
					_shield.isShield = true;

					if (Physics.Raycast(transform.position + _shield.perfectShieldVector, direction, checkRange, LayerMask.GetMask("Obstacle")))
					{
						_shield.spriteRendererDict [direction].sprite = _shield.perfectShield;
					}
					else
					{
						_shield.spriteRendererDict [direction].sprite = _shield.halfShield;
					}
				}
			}
		}
		
		void MakeSign(string mask, Sign sign)
		{
			if (tileStyle == TileStyle.NonWalkable) return;
			if (tileStyle == TileStyle.Normal) return;
			if (tileStyle == TileStyle.TwoArea && !_gameManager.doubleClose) return;
			
			foreach (var direction in GameUtility.EightDirections)
			{
				if (Physics.Raycast(transform.position + sign.origin, direction, checkRange*1.5f, LayerMask.GetMask(mask)))
				{
					sign.transformDictionary [direction].position = transform.position;
					sign.isSign = true;
					_gameManager.soundManager.PlayUi(SoundUi.Hover);
				}
			}
		}

		bool CheckSign(string mask, Sign sign)
		{
			if (tileStyle == TileStyle.NonWalkable) return false;
			if (tileStyle == TileStyle.Normal) return false;
			if (tileStyle == TileStyle.TwoArea && !_gameManager.doubleClose) return false;

			foreach (var direction in GameUtility.EightDirections)
			{
				if (Physics.Raycast(transform.position + sign.origin, direction, checkRange*1.5f, LayerMask.GetMask(mask)))
				{
					sign.transformDictionary [direction].position = this.transform.position;
					sign.isSign = true;
					return true;
				}
			}
			
			return false;
		}

		void ShowPathIn()
		{
			if (tileStyle != TileStyle.OneArea && tileStyle != TileStyle.TwoArea) return;
			
			var way = _board.PathFinding.GreedPathFinding(_board.startNode, this, _board.NodeList);

			if (way != null)
			{
				_gameManager.round.position = transform.position + _gameManager.roundVector;
				_board.currentWay = way;
				MakeWayLine(way);
			}
			
		}

		void ShowPathOut()
		{
			if (tileStyle != TileStyle.Normal) return;

			var way = _board.PathFinding.GreedPathFinding(_board.startNode, this, _board.NodeList, PathFindingStyle.Out);
			
			if (way == null) return;
			
			_gameManager.round.position = way[0].transform.position + _gameManager.roundVector;
			
			_board.currentWay = way;
			MakeWayLine(way);
		}

		void MakeWayLine (List<TileNode> wayList)
		{
			if (wayList == null) return;
			
			Vector3[] wayArray = new Vector3[wayList.Count + 1];
			
			for (int i = 0; i < wayList.Count; i++)
			{
				wayArray[i] = wayList[i].transform.position + _board.lineVector;
			}
			wayArray[wayList.Count] = _board.startNode.transform.position + _board.lineVector;
			_board.line.positionCount = wayList.Count + 1;
			_board.line.SetPositions(wayArray);
		}

		void OutShied()
		{
			if (_shield.isShield)
			{
				foreach (var node in _shield.transformDictionary.Values)
				{
					node.position = Vector3.zero;
				} 
				_shield.isShield = false;
			}
		}

		void OutSign(Sign sign)
		{
			if (sign.isSign)
			{
				foreach (var node in _sword.transformDictionary.Values)
				{
					node.position = Vector3.zero;
				}

				sign.isSign = false;
			}
		}

		void OutRangeEnemy()
		{
			if (_gameManager.currentPlayer == null) return;
		}

		void DoByType()
		{
			if (_gameManager.currentPlayer.characterType == CharacterType.SwordMaster)
			{
				if (skillState == SkillState.BaseCloseAttack || CheckSign("Enemy", _sword))
				{
					if (_gameManager.doubleClose)
					{
						if (_gameManager.currentPlayer.currentVigor > 0)
						{
							Moving(true);
							StartCoroutine(AfterMoving(true));
						}
					}
					else
					{
						if (_gameManager.currentPlayer.currentVigor > 1) 
						{ 
							Moving(true); 
							StartCoroutine(AfterMoving(true));
						}
					}
				}
				if (skillState == SkillState.Non)
				{
					Moving(false);
					StartCoroutine(AfterMoving(false));
				}
			}
			
			else if (_gameManager.currentPlayer.characterType == CharacterType.Ranger)
			{
				var currentEnemy = _gameManager.activeEnemyList.Find(i =>
					GameUtility.Coordinate(transform.position) == GameUtility.Coordinate(i.transform.position));

				if (skillState == SkillState.Non)
				{
					Moving(false);
					StartCoroutine(AfterMoving(false));
				}
			}
		}

		void CheckByType()
		{
			if (_gameManager.currentPlayer.characterType == CharacterType.SwordMaster)
			{
				if (_gameManager.doubleClose)
				{
					if (_gameManager.currentPlayer.currentVigor > 0)
					{
						if (tileStyle == TileStyle.OneArea || tileStyle == TileStyle.TwoArea)
						{
							MakeSign("Enemy", _sword);	
						}
					}
				}
				else
				{
					if (_gameManager.currentPlayer.currentVigor > 1)
					{
						if (tileStyle == TileStyle.OneArea)
						{
							MakeSign("Enemy", _sword);	
						}
					}
				}
			}
            				
			else if (_gameManager.currentPlayer.characterType == CharacterType.Ranger)
			{
				if (tileStyle == TileStyle.NonWalkable)
				{
					
				}
				else
				{
					if (_gameManager.currentPlayer.currentVigor > 1)
					{
						if (tileStyle == TileStyle.OneArea)
						{
							_gameManager.currentPlayer.RangeOff(_gameManager.activeEnemyList);
							_gameManager.currentPlayer.RangeAttackList(transform.position);
						}
						else
						{
							_gameManager.currentPlayer.RangeOff(_gameManager.activeEnemyList);
						}
					}
				}
			}
		}
		
		void OnMouseEnter()
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (_gameManager.somethingOn) return;
			if (_gameManager.currentPlayer == null) return;
			_gameManager.currentPlayer.RangeOff(_gameManager.activeEnemyList);
			
			_gameManager.PathSetting(tileStyle != TileStyle.NonWalkable);

			if (_gameManager.currentPlayer.activeState == ActiveState.NotAnything &&
			    _gameManager.currentPlayer.turnState == TurnState.Active)
			{
				CheckByType();
				ShowPathIn();
				ShowPathOut();
			}
		}

		void OnMouseExit()
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (_gameManager.somethingOn) return;
			
			OutRangeEnemy();
			OutSign(_sword);
		}

		void OnMouseUp()
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (_gameManager.somethingOn) return;

			// enable moving
			if (_gameManager.currentPlayer == null) return;
			_gameManager.currentPlayer.RangeOff(_gameManager.activeEnemyList);
			
			if (_gameManager.currentPlayer.turnState != TurnState.Active) return;
			if (_gameManager.currentPlayer.currentVigor <= 0) return;
			if (_gameManager.currentPlayer.activeState != ActiveState.NotAnything) return;

			DoByType();
			// OutShied();
			OutSign(_sword);
		}

		IEnumerator AfterMoving(bool close)
		{
			while (_gameManager.currentPlayer.activeState == ActiveState.Moving)
			{
				yield return null;
			}
			
			_gameManager.Marked();
			
			if (close)_gameManager.currentPlayer.BaseCloseAttack();
		}
		
		void Moving(bool skill)
		{
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
			if (_gameManager.currentPlayer == null) return;
			if (_board.startNode == null) return;
			if (_board.currentWay == null) return;
			
			_gameManager.soundManager.PlayUi(SoundUi.Click);
			
			var roundTile = _board.NodeList.Find(i => i.Coordinate == GameUtility.Coordinate(_gameManager.round.transform.position));
			
			if (roundTile.tileStyle == TileStyle.OneArea || roundTile.tileStyle == TileStyle.TwoArea)
			{
				_gameManager.currentPlayer.MovingSkill(roundTile);
				_gameManager.currentPlayer.playerMove.IndicateUnit(_board.currentWay, skill);
				_board.currentWay = null;
			}
		}
	}
}