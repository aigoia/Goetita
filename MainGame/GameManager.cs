using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.MainGame {
	public enum UserState {
		Non,
		Base,
		CloseCombat,
	}

	public enum Turn {

		PlayerTurn,
		EnemyTurn,
	}

	public class GameManager : MonoBehaviour {

		public Turn turnState;
		public Player currentPlayer;

		public List<Player> playerList;
		public List<Player> activePlayerList = new List<Player> ();
		List<Enemy> _enemyList;
		public List<Enemy> activeEnemyList = new List<Enemy> ();
		List<TileNode> _closeCombatList;
		public GameObject holder;
		public GameObject milestone;
		readonly WaitForSeconds _wait = new WaitForSeconds (1.4f);

		public EffectManager effectManager;
		public SoundManager soundManager;
		public PlayerData playerData;
		public DeckManager deckManager;
		public MotionManager motionManager;

		public int turnTime = 0;
		public float waitTime = 1f;

		public float magicResist = 0.2f;
		public float armorPiercing = 0.2f;
		public int minimalDamage = 1;
		public int plusArmorPiercing = 1;

		// game setting
		public bool doubleClose = true;
		public bool inPlaceClose = false;
		public bool activeSkill = false;

		public Vector3 closePos = new Vector3 (0, -0.5f, 0);
		readonly float alpha = 1.5f;
		public Vector3 roundVector = new Vector3 (0, 0.12f, 0);
		[SerializeField] private int mark = 1;
		public CameraController cameraController;

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
		public MainCanvas mainCanvas;

		public AreaCheck areaCheck;
		public Common common;

		public float visionDistance = 26f;
		public WaitForSeconds waitClose = new WaitForSeconds (1.6f);

		public Settings settings;
		public UnityEvent gameOver;
		public UnityEvent victory;
		private Color _settingsColor;
		public AudioManager audioManager;

		public bool criticalHitCheck = false;
		public float criticalTime = 4f;
		public float criticalPercent = 75;
		public int normalDifficult = 0;
		public float criticalDamageUpMax = 1.9f;
		public float criticalDamageUpNormal = 1.5f;
		public float criticalDamageUpMin = 1.1f;
		public EaseType criticalEaseType;
		public float powerfulDeal = 0.75f;
		
		public bool showCard = false;

		void Awake () {
			if (mainCanvas == null) mainCanvas = FindObjectOfType<MainCanvas> ();
			if (common == null) common = GetComponent<Common> ();
			if (round == null) round = transform.Find ("Round");
			if (_playerTransform == null) _playerTransform = transform.Find ("PlayerUnit");
			if (_enemyTransform == null) _enemyTransform = transform.Find ("EnemyUnit");
			if (close == null) close = transform.Find ("Close");
			if (_cameraController == null) _cameraController = FindObjectOfType<CameraController> ();
			if (_board == null) _board = FindObjectOfType<Board> ();
			if (enemyAi == null) enemyAi = FindObjectOfType<EnemyAi> ();
			if (effectManager == null) effectManager = GetComponent<EffectManager> ();
			if (soundManager == null) soundManager = FindObjectOfType<SoundManager> ();
			if (areaCheck == null) areaCheck = GetComponent<AreaCheck> ();
			if (milestone == null) milestone = transform.Find ("Milestone").gameObject;
			if (playerData == null) playerData = transform.Find ("PlayerUnit").GetComponent<PlayerData> ();
			if (settings == null) settings = FindObjectOfType<Settings> ();
			if (cameraController == null) cameraController = FindObjectOfType<CameraController> ();
			if (audioManager == null) audioManager = FindObjectOfType<AudioManager> ();
			if (deckManager == null) deckManager = FindObjectOfType<DeckManager> ();
			if (motionManager == null) motionManager = FindObjectOfType<MotionManager> ();
		}

		private void Start () {

			// make enemy list
			_enemyList = new List<Enemy> (FindObjectsOfType<Enemy> ());
			MakeActiveEnemyList (_enemyList, activeEnemyList);

			_cameraController.postProcessing.profile.vignette.enabled = true;
			Marked ();

			// if (audioManager != null) audioManager.FadeInCaller();
		}

		public void MarkedDead (Enemy dead, Player player) {
			foreach (var enemy in activeEnemyList) {
				if (enemy.name == dead.name) {
					continue;
				}

				if (areaCheck.EnemySight (dead, enemy, visionDistance)) {
					// enemy.transform.LookAt(player.transform);
					player.marked = player.Mark;
				}
			}

			_cameraController.CheckMark ();
		}

		public void SingleMarked (Enemy enemy) {
			foreach (var player in playerList) {
				if (areaCheck.EnemySight (player, enemy, visionDistance)) {
					// enemy.transform.LookAt(player.transform);
					player.marked = player.Mark;
				}
			}
			_cameraController.CheckMark ();
		}

		public void Marked () {
			foreach (var enemy in activeEnemyList) {
				foreach (var player in playerList) {
					if (areaCheck.EnemySight (player, enemy, visionDistance)) {
						// enemy.transform.LookAt(player.transform);
						player.marked = player.Mark;
					}
				}
			}

			_cameraController.CheckMark ();
		}

		public void Marked (List<Vector3> way, Player player) {
			foreach (var enemy in activeEnemyList) {
				foreach (var node in way) {
					if (areaCheck.EnemySight (node, enemy, visionDistance)) {
						// enemy.transform.LookAt(node);
						player.marked = player.Mark;
					}
				}
			}

			_cameraController.CheckMark ();
		}

		public void SomeThingOnTrue () {
			somethingOn = true;
		}
		public void SomeThingOnFalse () {
			somethingOn = false;
		}

		public void GotoMainMenu () {
			foreach (var player in playerList) {
				print (player.currentHp);
				if (player.currentHp <= 0) {
					player.currentHp = 1;
					player.copyHp = 1;
				}
			}
			SaveCharacter ();
			// audioManager.FadeOutCaller();
			SceneManager.LoadScene ("Menu");
		}

		public void GotoWindow () {
			foreach (var player in playerList) {
				print (player.currentHp);
				if (player.currentHp <= 0) {
					player.currentHp = 1;
					player.copyHp = 1;
				}
			}
			SaveCharacter ();

			// if (audioManager != null) audioManager.FadeOutCaller();
			SceneManager.LoadScene ("Window");
		}
		public void GotoWin () {
			// settings.GiveItem();
			SaveCharacter ();

			// if (audioManager != null) audioManager.FadeOutCaller();
			SceneManager.LoadScene ("Window");
		}

		void SaveCharacter () {
			foreach (var player in activePlayerList) {
				player.currentHp = player.copyHp;
			}

			var characters = settings.LoadCharacter ();

			foreach (var character in characters) {
				// if character didn't attend battle, keep their hp
				character.currentHp = 1;
			}

			foreach (var player in activePlayerList) {
				characters.Find (i => i.characterName == player.characterName).currentHp = player.currentHp;
			}

			settings.SaveCharacter (characters);
		}

		void CloseAttack () {
			if (currentPlayer.characterType != CharacterType.Claymore) return;

			foreach (var direction in GameUtility.EightDirections) {
				if (Physics.Raycast (currentPlayer.transform.position + Vector3.up, direction, GameUtility.interval * alpha, LayerMask.GetMask ("Enemy"))) {
					currentPlayer.CloseOn ();
				}
			}
		}

		public void MakeActivePlayerList (List<Player> players, List<Player> activePlayers) {
			foreach (var unit in players) {
				if (unit.gameObject.activeSelf == true && unit.activeState != ActiveState.Dead) {
					if (unit.currentHp > 0 && unit.baseHp > 0) {
						activePlayers.Add (unit);
					}
				}
			}
		}

		private void MakeActiveEnemyList (List<Enemy> participants, List<Enemy> activeParticipants) {
			foreach (var unit in participants) {
				if (unit.gameObject.activeSelf == true && unit.activeState != ActiveState.Dead) {
					activeParticipants.Add (unit);
				}
			}
		}

		public void ResetClose () {
			GameUtility.ResetObjects (close, holder);
		}

		public void NextPlayer () {
			var checkVictory = CheckVictory ();
			if (checkVictory == true) return;

			foreach (var player in playerList) {
				player.round.SetActive (false);
			}

			currentPlayer = null;

			foreach (var player in activePlayerList) {
				if (player.activeState == ActiveState.Dead) continue;
				if (player.currentHp <= 0) continue;
				if (player.currentVigor <= 0) {
					player.turnState = TurnState.Ending;
					continue;
				}

				if (player.transform.position.y < 0) continue;

				if (player.turnState != TurnState.Ending) {
					currentPlayer = player;
					SelectPlayer (player);
					return;
				}
			}
			
			// cardManager.InitShow();

			print ("No one");
			PlayerTurnEnding ();
		}

		public void PathSetting (bool setting) {
			round.transform.gameObject.SetActive (setting);
			pathLine.gameObject.SetActive (setting);
		}

		public void SelectPlayer (Player thisPlayer) {
			if (turnState != Turn.PlayerTurn) return;
			_board.line.positionCount = 0;
			round.position = roundVector;

			foreach (var player in activePlayerList) {
				if (player.turnState == TurnState.Active) {
					player.turnState = TurnState.Waiting;
					player.userState = UserState.Non;
					player.PlayerOff ();
				}
			}

			if (thisPlayer.activeState == ActiveState.Dead) return;

			currentPlayer = thisPlayer;
			currentPlayer.userState = UserState.Base;
			currentPlayer.turnState = TurnState.Active;
			currentPlayer.PlayerOn ();
			_board.ResetBoard ();

			CloseUiCheck ();

			// profile
			mainCanvas.profileImage.gameObject.SetActive (true);
			mainCanvas.profileImage.sprite = settings.SetProfileImage (thisPlayer, "100x100");

			if (showCard == false)
			{
				deckManager.InitShow();
				showCard = true;
			}
			
			// mainCanvas.profileImage.sprite = settings.profileImageList.Find(image => image.name == thisPlayer.characterName).transform
			// 	.Find("100x100").GetComponent<Image>().sprite;
		}

		void CloseUiCheck () {
			if (currentPlayer.characterType == CharacterType.Claymore) {
				CloseAttack ();
			}
		}

		public void ResetRound () {
			round.transform.position = Vector3.zero;
		}

		public bool CheckGameOver () {
			if (activePlayerList.Count == 0) {
				print ("Game Over");
				// settings.ResetAll();
				StartCoroutine (WaitPlayerEnding ());
				return true;
			}

			return false;
		}

		public bool CheckVictory () {
			if (activeEnemyList.Count == 0) {
				print ("Victory");

				if (_board.initMapList.Exists (map => map.mapName == _board.currentMap.mapName)) {
					settings.baseData.initClear = true;
					settings.SaveBaseData ();
				}

				StartCoroutine (WaitEnemyEnding ());
				return true;
			}

			return false;
		}

		public bool CheckCloseVictory () {
			if (activeEnemyList.Count == 0) {
				print ("Victory");
				return true;
			}

			return false;
		}

		IEnumerator WaitEnemyEnding () {
			foreach (var enemy in _enemyList) {
				while (enemy.getHit == GetHit.GetHit) {
					yield return null;
				}
			}

			victory.Invoke ();
		}

		IEnumerator WaitPlayerEnding () {
			WaitForSeconds waitForSeconds;

			foreach (var player in playerList) {
				while (player.beHit == BeHit.BeHit) {
					yield return null;
				}
			}

			yield return _wait;
			gameOver.Invoke ();
		}

		public void EnemyTurnEnding () {
			if (turnState == Turn.PlayerTurn) return;

			activeEnemyList.ForEach (i => i.getHit = GetHit.Normal);
			activePlayerList.ForEach (i => i.giveHit = GiveHit.Normal);

			foreach (var player in playerList) {
				player.currentHp = player.copyHp;

				if (player.currentHp > 0) {
					if (!activePlayerList.Contains (player)) {
						activePlayerList.Add (player);
						player.activeState = ActiveState.NotAnything;
						player.turnState = TurnState.Waiting;
					}
				}
			}

			foreach (var player in playerList) {
				if (player.marked > 0) player.marked = player.marked - 1;
			}

			turnState = Turn.PlayerTurn;
			print ("PlayerTurn");
			// soundManager.PlaySystem(SoundSystem.Open);

			// if you put fog, you'll change this
			enemyAi.thisEnemy = null;
			mainCanvas.enemyTurnText.SetActive (false);

			if (activePlayerList.Count > 0) {
				foreach (var player in activePlayerList) {
					// refill vigor
					player.currentVigor = player.baseVigor;
					player.turnState = TurnState.Waiting;
				}

				Marked ();
				NextPlayer ();
			}
		}

		public void PlayerTurnEnding () {
			if (currentPlayer == null) return;
			if (currentPlayer.currentVigor > 0) return;

			foreach (var enemy in _enemyList) {
				enemy.aim.SetActive (false);
			}
			// remove the aim sign

			foreach (var player in activePlayerList) {
				if (player.activeState == ActiveState.Moving) return;
			}

			var waitingPlayerList = new List<Player> ();

			foreach (var player in activePlayerList) {
				if (player.turnState == TurnState.Waiting) {
					waitingPlayerList.Add (player);
				}
			}

			if (waitingPlayerList.Count != 0) {
				currentPlayer = waitingPlayerList[0];
				SelectPlayer (currentPlayer);
			} else {
				turnTime = turnTime + 1;
				EnemyTurnStart ();
			}
		}

		public void EnemyTurnStart () {
			if (turnState == Turn.EnemyTurn) return;

			mainCanvas.profileImage.gameObject.SetActive (false);

			foreach (var enemy in _enemyList) {
				enemy.aim.SetActive (false);
			}

			_board.line.positionCount = 0;
			round.position = roundVector;
			mainCanvas.enemyTurnText.SetActive (true);

			StartCoroutine (WaitPlayerAction ());
		}

		IEnumerator WaitPlayerAction () {
			if (currentPlayer != null) {
				foreach (var player in activePlayerList) {
					while (somethingOn) {
						yield return null;
					}
					player.PlayerOff ();
				}
			}

			print ("Check");

			foreach (var enemy in activeEnemyList) {
				enemy.currentVigor = enemy.baseVigor;
				enemy.turnState = TurnState.Waiting;
			}

			currentPlayer = null;

			_board.ResetBoard ();
			turnState = Turn.EnemyTurn;
			print ("EnemyTurn");

			if (enemyAi != null) {
				enemyAi.StartAi ();
			}
		}
	}
}