using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.MainGame
{
	public enum UserState
	{
		Non, Base, CloseCombat, 
	}

	public enum Turn {

		PlayerTurn, EnemyTurn,
	}

	public class GameManager : MonoBehaviour {

		public Turn turnState;
		public Player currentPlayer;
		
		List<Player> _playerList;
		public List<Player> activePlayerList = new List<Player>();
		List<Enemy> _enemyList;
		public List<Enemy> activeEnemyList = new List<Enemy>();
		List<TileNode> _closeCombatList;
		List<Weapon> _weaponList;
		public GameObject holder;
		public GameObject milestone;
		
		public EffectManager effectManager;
		
		// game setting
		public bool doubleClose = true;
		public bool inPlaceClose = false;
		public bool activeSkill = false;
		
		public float swordEndTime = 0.9f;
		public float swordHit = 0.4f;
		public float swordMin = 0.15f;
		public float swordMax = 0.20f;
		public float rangeEndTime = 0f;
		public float rangeHit = 0.3f;
		public float rangeMin = 0.15f;
		public float rangeMax = 0.20f;
		
		public Vector3 closePos = new Vector3 (0, -0.5f, 0);
		readonly float alpha = 1.5f;
		public Vector3 roundVector = new Vector3 (0, 0.1f, 0);
		[SerializeField] private int mark = 1;
		
		public Transform round;
		public Transform pathLine;
		public Transform close;
		Transform _playerTransform;
		Transform _enemyTransform;
		public bool somethingOn = false;
		
		public Transform testObjects;

		[SerializeField] GameObject utilityImage;
		public EnemyAi enemyAi;
		Board _board;
		public PlayerUi playerUi;
		CameraController _cameraController;
		public GameObject testObjectA;
		public GameObject testObjectB;
		public GameObject testObjectC;
		MainCanvas _mainCanvas;
		
		public AreaCheck areaCheck;
		public Common common;

		public float visionRange = 24f;
		public WaitForSeconds waitClose = new WaitForSeconds(1.6f);

		public UnityEvent gameOver;
		public UnityEvent victory;
		
		void Awake()
		{
			if (_mainCanvas == null) _mainCanvas = FindObjectOfType<MainCanvas>();
			if (common == null) common = GetComponent<Common>();
			if (round == null) round = transform.Find("Round");
			if (_playerTransform == null)_playerTransform = transform.Find("PlayerUnit");
			if (_enemyTransform == null)_enemyTransform = transform.Find("EnemyUnit");
			if (close == null) close = transform.Find("Close");
			if (_cameraController == null)_cameraController = FindObjectOfType<CameraController>();
			if (_board == null) _board = FindObjectOfType<Board>();
			_playerList = new List<Player>(FindObjectsOfType<Player>());
			MakeActivePlayerList(_playerList, activePlayerList);
			_enemyList = new List<Enemy>(FindObjectsOfType<Enemy>());
			if (enemyAi == null) enemyAi = FindObjectOfType<EnemyAi>();
			MakeActiveEnemyList(_enemyList, activeEnemyList);
			_weaponList = new List<Weapon>(FindObjectsOfType<Weapon>());
			if (effectManager == null) effectManager = GetComponent<EffectManager>();
			if (areaCheck == null) areaCheck = GetComponent<AreaCheck>();
			if (milestone == null) milestone = transform.Find("Milestone").gameObject;
		}

		private void Start()
		{
			Marked();
		}

		public void Marked()
		{
			foreach (var enemy in activeEnemyList)
			{
				foreach (var player in _playerList)
				{
					if (areaCheck.CanSee(player, enemy, visionRange))
					{
						player.marked = player.Mark;
					}
				}
			}
		}

		public void SomeThingOnTrue()
		{
			somethingOn = true;
		}
		public void SomeThingOnFalse()
		{
			somethingOn = false;
		}

		public void GotoMainMenu()
		{
			SceneManager.LoadScene("GameMenu");
		}

		void CloseAttack()
		{
			if (currentPlayer.characterType != CharacterType.SwordMaster) return;
			
			foreach (var direction in GameUtility.EightDirections)
			{ 
				if (Physics.Raycast(currentPlayer.transform.position + Vector3.up, direction, GameUtility.interval * alpha, LayerMask.GetMask("Enemy"))) 
				{
					currentPlayer.CloseOn();
				}
			}
			
		}
		
		private void MakeActivePlayerList(List<Player> players, List<Player> activePlayers)
		{
			foreach (var unit in players)
			{
				if (unit.gameObject.activeSelf == true && unit.activeState != ActiveState.Dead)
				{
					activePlayers.Add(unit);
				}
			}
		}

		private void MakeActiveEnemyList(List<Enemy> participants, List<Enemy> activeParticipants)
		{
			foreach (var unit in participants)
			{
				if (unit.gameObject.activeSelf == true && unit.activeState != ActiveState.Dead)
				{
					activeParticipants.Add(unit);
				}
			}
		}

		public void ResetClose()
		{
			GameUtility.ResetObjects(close, holder);
		}

		public void NextPlayer()
		{
			currentPlayer = null;

			foreach (var player in activePlayerList)
			{
				if (player.activeState == ActiveState.Dead) continue;

				if (player.turnState != TurnState.Ending)
				{
					currentPlayer = player;
					SelectPlayer(player);
					return;
				}
			}
		}

		public void PathSetting(bool setting)
		{
			round.transform.gameObject.SetActive(setting);
			pathLine.gameObject.SetActive(setting);
		}

		public void SelectPlayer(Player thisPlayer)
		{
			if (turnState != Turn.PlayerTurn) return;
			_board.line.positionCount = 0;
			round.position = roundVector;

			foreach (var player in activePlayerList)
			{
				if (player.turnState == TurnState.Active)
				{
					player.turnState = TurnState.Waiting;
					player.userState = UserState.Non;
					player.PlayerOff();
				}
			}

			if (thisPlayer.activeState == ActiveState.Dead) return;

			currentPlayer = thisPlayer;
			currentPlayer.userState = UserState.Base;
			currentPlayer.turnState = TurnState.Active;
			currentPlayer.PlayerOn();
			_board.ResetBoard();
			
			CloseUiCheck();
		}
		
		void CloseUiCheck()
		{
			if (currentPlayer.characterType == CharacterType.SwordMaster)
			{
				CloseAttack();
			}
		}

		public void ResetRound()
		{
			round.transform.position = Vector3.zero;
		}

		public bool CheckGameOver()
		{
			if (activePlayerList.Count == 0)
			{
				print("Game Over");
				StartCoroutine(WaitPlayerEnding());
				return true;
			}

			return false;
		}
		
		public bool CheckVictory()
		{
			if (activeEnemyList.Count == 0)
			{
				print("Victory");
				StartCoroutine(WaitEnemyEnding());
				return true;
			}

			return false;
		}
		
		public bool CheckCloseVictory()
		{
			if (activeEnemyList.Count == 0)
			{
				print("Victory");
				return true;
			}

			return false;
		}

		IEnumerator WaitEnemyEnding()
		{
			foreach (var enemy in _enemyList)
			{
				while (enemy.getHit == GetHit.GetHit)
				{
					yield return null;
				}				
			}
			
			victory.Invoke();
		}
		
		IEnumerator WaitPlayerEnding()
		{
			foreach (var player in _playerList)
			{
				while (player.beHit == BeHit.BeHit)
				{
					yield return null;
				}				
			}

			gameOver.Invoke();
		}
		
		public void EnemyTurnEnding()
		{
			activeEnemyList.ForEach(i => i.getHit = GetHit.normal);
			activePlayerList.ForEach(i => i.giveHit = GiveHit.normal);
			
			foreach (var player in activePlayerList)
			{
				if (player.marked > 0) player.marked = player.marked - 1;
			}

			turnState = Turn.PlayerTurn;
			print("PlayerTurn");
		
			// if you put fog, you'll change this
			enemyAi.currentEnemy = null;
			_mainCanvas.enemyTurnText.SetActive(false);
		
			if (activePlayerList.Count > 0)
			{
				foreach (var player in activePlayerList)
				{
					player.currentVigor = player.baseVigor;
					player.turnState = TurnState.Waiting;
				}
				
				NextPlayer();
			}
		}

		public void PlayerTurnEnding()
		{
			if (currentPlayer == null) return;
			if (currentPlayer.currentVigor > 0) return;

			foreach (var player in activePlayerList)
			{
				if (player.activeState == ActiveState.Moving) return;
			}

			var waitingPlayerList = new List<Player>();

			foreach (var player in activePlayerList)
			{
				if (player.turnState == TurnState.Waiting)
				{
					waitingPlayerList.Add(player);	
				}
			}
		
			if (waitingPlayerList.Count != 0)
			{
				currentPlayer = waitingPlayerList[0];
				SelectPlayer(currentPlayer);
			}
			else
			{
				EnemyTurnStart();
			}
		}

		public void EnemyTurnStart()
		{
			if (turnState == Turn.EnemyTurn) return;
			
			_board.line.positionCount = 0;
			round.position = roundVector;
			_mainCanvas.enemyTurnText.SetActive(true);
			
			StartCoroutine(WaitPlayerAction());
		}
		
		IEnumerator WaitPlayerAction()
		{
			if (currentPlayer != null)
			{
				foreach (var player in activePlayerList)
				{
					while (somethingOn) 
					{
						yield return null;
					}
					player.PlayerOff();
				}
			}
			
			print("Check");
			
			foreach (var enemy in activeEnemyList) 
			{
				enemy.currentVigor = enemy.baseVigor; 
				enemy.turnState = TurnState.Waiting;
			}
			
			currentPlayer = null;
			
			_board.ResetBoard();
			turnState = Turn.EnemyTurn;
			print("EnemyTurn");

			if (enemyAi != null) 
			{ 
				enemyAi.StartAi();
			}
		}
	}
}