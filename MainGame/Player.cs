using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Game.MainGame
{
	public enum CharacterType
	{
		Default, SwordMaster, Ranger,
	}

	public enum GetHit
	{
		normal, GetHit
	}

	public enum GiveHit
	{
		normal, GiveHit
	}
	
	public enum TurnState 
	{
		Waiting, Active, Ending,
	}

	public enum ActiveState
	{
		NotAnything, Moving, UseSkill, Dead,
	}

	public class Player : MonoBehaviour {
		
		[SerializeField] private float farFrom = 2f;

		public int activeNumber;
		
		public bool isClosed = true;
		public bool canSelect = true;
		public List<TileNode> closeList;
		public float deadHeight = 1f;

		public bool hitPlayerAction = false;
		public bool doubleBase = true;

		public UserState userState = UserState.Non;
		public CharacterType characterType = CharacterType.SwordMaster;
		public int marked = 0;
		public readonly int Mark = 2;
		
		MainCanvas _canvas;

		// HP
		public int baseHp = 4;
		public int currentHp;
		public CharacterBar characterBar;
		
		private Enemy _currentEnemy;

		public int baseVigor = 2;
		public int currentVigor;

		public GameObject moveBaseArea;
		public GameObject moveBaseBlock;
		public GameObject moveDoubleArea;
		public GameObject moveDoubleBlock;
		
		public GameObject rangeArea;
		
		PlayerUi _playerUi;
		public GameObject round;
		Transform _utility;
		Transform _model;
		GameObject _tail;
		public Vector3 direction;

		public PlayerMove playerMove;
		GameManager _gameManager;
		public Animator animator;
		CameraController _cameraController;
		private SoundManager _soundManager;

		Board _board;
		public Weapon baseWeapon;

		public TurnState turnState = TurnState.Waiting;
		public ActiveState activeState = ActiveState.NotAnything;
		public BeHit beHit = BeHit.Normal;
		public GiveHit giveHit = GiveHit.normal;
		
		private static readonly int BaseAttack = Animator.StringToHash("BaseAttack");
		private static readonly int Die = Animator.StringToHash("Die");
		private static readonly int Withstand = Animator.StringToHash("Withstand");
		
		private static readonly int RangeAttack = Animator.StringToHash("RangeAttack");
		private IEnumerator _checkEnemyDead;

		void Awake()
		{
			if (_currentEnemy == null) _currentEnemy = GetComponent<Enemy>();
			if (_board == null) _board = FindObjectOfType<Board>();
			if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
			currentVigor = baseVigor;
			currentHp = baseHp;
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (_soundManager == null) _soundManager = FindObjectOfType<SoundManager>();
			if (playerMove == null) playerMove = GetComponent<PlayerMove>();
			if (animator == null) animator = GetComponent<Animator>();
			if (_playerUi == null) _playerUi = _gameManager.playerUi;
			if (_canvas == null) _canvas = _gameManager.GetComponent<UiManager>().canvas.GetComponent<MainCanvas>();

			if (_utility == null) _utility = transform.Find("Utility");
			if (moveBaseArea == null) moveBaseArea = _utility.Find("MoveBaseArea").gameObject;
			if (moveDoubleArea == null) moveDoubleArea = _utility.Find("MoveDoubleArea").gameObject;
			if (moveDoubleBlock == null) moveBaseBlock = _utility.Find("MoveBaseBlock").gameObject;
			if (moveDoubleBlock == null) moveDoubleBlock = _utility.Find("MoveDoubleBlock").gameObject;
			
			if (round == null) round = _utility.Find("Round").gameObject;
			if (rangeArea == null) rangeArea = _utility.Find("DoubleArea").gameObject;
			
			if (_model == null) _model = transform.Find("Model");
			if (_tail == null) _tail = _utility.Find("Tail").gameObject;
		}
		
		bool CheckBaseAttack()
		{
			if (characterType != CharacterType.SwordMaster) return false;
				
			foreach (var enemyDirection in GameUtility.EightDirections)
			{
				if (Physics.Raycast(transform.position + Vector3.up, enemyDirection, GameUtility.interval * GameUtility.Alpha, LayerMask.GetMask("Enemy")))
				{
					return true;
				}
			}
			
			return false;
		}

		public void CloseAttackUiCheck()
		{
			if (CheckBaseAttack())
			{
				CloseOn();
			}
		}

		public void CloseOn()
		{
			if (!_gameManager.activeSkill) return;
			
			foreach (var icon in _gameManager.playerUi.activeUiList)
			{
				if (icon.name == "CloseAttack")
				{
					icon.SetActive(true);
				}
			}
		}

		public bool CheckRangeSingle(Enemy enemy, Player player)
		{
			RangeOff(_gameManager.activeEnemyList);
			if (player == null) return false;

			var areaEnemyList = _gameManager.areaCheck.RangeEnemyList(player, AreaCheckOption.Player, transform.position);
			if (areaEnemyList == null) return false;
			if (!areaEnemyList.Contains(enemy))
			{
				return false;
			}
			
			RaycastHit hit;
			enemy.GetComponent<CapsuleCollider>().enabled = false;
			if (Physics.Linecast(transform.position + Vector3.up, enemy.transform.position + Vector3.up, out hit,
				LayerMask.GetMask("Obstacle", "Player", "Enemy")))
			{
				enemy.GetComponent<CapsuleCollider>().enabled = true;
				return true;
			}
			enemy.GetComponent<CapsuleCollider>().enabled = true;
			
			RangeSingleOn(enemy);
			return false;
		}

		public List<Enemy> RangeAttackList(Vector3 position)
		{
			RangeOff(_gameManager.activeEnemyList);

			var enemyRangeList = new List<Enemy>();

			var areaEnemyList = _gameManager.areaCheck.RangeEnemyList(this, AreaCheckOption.Round, position);
			if (areaEnemyList == null) return null;
			
			foreach (var enemy in areaEnemyList)
			{
				if (enemy.activeState == ActiveState.Dead)
				{
					_gameManager.activeEnemyList.Remove(enemy);
					continue;
				}
				
				RaycastHit hit;
				enemy.GetComponent<CapsuleCollider>().enabled = false;
				if (Physics.Linecast(position + Vector3.up, enemy.transform.position + Vector3.up, out hit,
					LayerMask.GetMask("Obstacle", "Player", "Enemy")))
				{
					enemy.GetComponent<CapsuleCollider>().enabled = true;
					continue;
				}
				
				enemyRangeList.Add(enemy);
				enemy.GetComponent<CapsuleCollider>().enabled = true;
			}

			RangeOn(enemyRangeList);
			return enemyRangeList;
		}

		void RangeSingleOn(Enemy enemy)
		{
			if (_cameraController.rangeLevel == 0 || _cameraController.rangeLevel == 1)
			{
				enemy.characterBar.rangeDown.SetActive(true);
			}
			else if (_cameraController.rangeLevel == 2)
			{
				enemy.characterBar.rangeUp.SetActive(true);
			}
            
			enemy.canTargeted = true;
		}

		void RangeOn(List<Enemy> enemies)
		{
			if (_gameManager.somethingOn) return;
			
			foreach (var enemy in enemies)
			{
				if (enemy.activeState == ActiveState.Dead) continue;
				if (enemy.GetComponent<CapsuleCollider>().enabled == false) continue;
				if (enemy.currentHp <= 0) continue;
				
				if (_cameraController.rangeLevel == 0 || _cameraController.rangeLevel == 1)
				{
					enemy.characterBar.rangeDown.SetActive(true);
				}
				else if (_cameraController.rangeLevel == 2)
				{
					enemy.characterBar.rangeUp.SetActive(true);
				}

				enemy.canTargeted = true;
			}
		}

		public void RangeOff(List<Enemy> enemies)
		{
			foreach (var enemy in enemies)
			{
				enemy.characterBar.rangeDown.SetActive(false);
				enemy.characterBar.rangeUp.SetActive(false);
			}
		}

		public void RangeArea()
		{
			var testEnemy = _gameManager.activeEnemyList[0];
			testEnemy.baseRangeArea = new List<TileNode>();
			testEnemy.rangeArea = new List<TileNode>();
			testEnemy.moveDoubleArea.SetActive(true);
			
			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("Obstacle", "Player", "Enemy")))
				{
					continue;
				}
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleArea")))
				{
					testEnemy.baseRangeArea.Add(node);
				}
			}
			
			foreach (var node in testEnemy.baseRangeArea)
			{
				RaycastHit hit;
				if (Physics.Linecast(node.transform.position, testEnemy.transform.position, out hit,
					LayerMask.GetMask(("Obstacle"))))
				{
					
				}
				else
				{
					testEnemy.rangeArea.Add(node);
				}
			}
			
			testEnemy.moveDoubleArea.SetActive(false);
		}

		public void PlayerOn()
		{
			RangeOff(_gameManager.activeEnemyList);

			foreach (var icon in _playerUi.iconList)
			{
				icon.GetComponent<IconManager>().highlighted.SetActive(false);
			}
			
			if (characterType == CharacterType.SwordMaster && _gameManager.inPlaceClose) CloseAttackUiCheck();
			_playerUi.gameObject.SetActive(true);
			round.SetActive(true);
			_tail.SetActive(true);

			foreach (var icon in _playerUi.iconList)
			{
				icon.GetComponent<IconManager>().SettingIcon();
			}
		}

		public void PlayerOff()
		{
			_playerUi.gameObject.SetActive (false);
			round.SetActive (false);
			_tail.SetActive (false);
		}

		public void BaseCloseAttack()
		{
			_playerUi.UiSetActive(false);
			if (characterType != CharacterType.SwordMaster) return;
			print(_gameManager.currentPlayer.activeState);
			
			var currentEnemyList = new List<Enemy>();

			foreach (var enemyDirection in GameUtility.EightDirections)
			{
				RaycastHit hit;
				if (Physics.Raycast(transform.position + Vector3.up, enemyDirection, out hit, GameUtility.interval * GameUtility.Alpha, LayerMask.GetMask("Enemy")))
				{
					if (hit.transform == null) continue;
					if (hit.transform.GetComponent<Enemy>().activeState == ActiveState.Dead) continue;

					var enemy = hit.transform.GetComponent<Enemy>();
					enemy.direction = enemyDirection;
					currentEnemyList.Add(enemy);
				}
			}

			StartCoroutine(CloseSelectEnemy(currentEnemyList, 2f));
		}

		void CloseCamera()
		{
			if (isClosed)
			{
				_cameraController.CameraMoveKey(isClosed, false);
				_cameraController.CloseCamera(this.transform.position + _gameManager.closePos, isClosed, true);
				_cameraController.mainCanvas.gameObject.SetActive(false);
			}
		}

		void NonCloseCamera()
		{
			if (isClosed)
			{
				_cameraController.CameraMoveKey(isClosed, true);
				_cameraController.CloseCamera(transform.position, isClosed, false);
				_cameraController.mainCanvas.gameObject.SetActive(true);
			}
		}

		void StartAllEnemy()
		{
			_board.line.gameObject.SetActive(false);
			_gameManager.somethingOn = true;
			_gameManager.PathSetting(false);
			
			PlayerOff();
			CloseCamera();
		}

		void EndAllEnemy()
		{
			_currentEnemy = null;
			_board.line.gameObject.SetActive(true);
			_gameManager.PathSetting(true);
			
			NonCloseCamera();
			_model.position = transform.position;
			currentVigor = 0;
			EndPlayer();
			
			_gameManager.currentPlayer = null;
			if (!_gameManager.CheckCloseVictory())
			{
				_gameManager.somethingOn = false;
				_gameManager.NextPlayer();
			}
		}

		IEnumerator CloseSelectEnemy(List<Enemy> currentEnemyList, float farFrom = 2f)
		{
			StartAllEnemy();
		
			GameUtility.ShuffleList(currentEnemyList);
			var enemy = currentEnemyList[0];
			
			animator.SetTrigger(BaseAttack);

			transform.LookAt(enemy.transform);
			if (enemy != currentEnemyList[0]) _cameraController.RotateButton(1);

			StartCoroutine(WaitCloseSkill(enemy, farFrom));

			while (giveHit == GiveHit.GiveHit)
			{
				yield return null;
			}
			
			var victory = _gameManager.CheckVictory();
			EndAllEnemy();
			
			while (enemy.getHit == GetHit.GetHit)
			{
				yield return null;
			}
			
			if (!victory) CheckPlayerTurn();
		}

		void CheckPlayerTurn()
		{
			foreach (var player in _gameManager.activePlayerList)
			{
				if (player.turnState != TurnState.Ending) return;
			}
			
			_gameManager.EnemyTurnStart();
		}

		IEnumerator WaitCloseSkill(Enemy enemy, float farFromCharacter)
		{
			enemy.GetComponent<CapsuleCollider>().enabled = false; 
			_gameManager.somethingOn = true;             
			enemy.getHit = GetHit.GetHit;
			giveHit = GiveHit.GiveHit;                         
			
			if (enemy.direction == Vector3.forward ||
			    enemy.direction == Vector3.back ||
			    enemy.direction == Vector3.left ||
			    enemy.direction == Vector3.right)
			{
				_model.position = this.transform.position - enemy.direction / farFromCharacter;
			}
			else 
			{
				_model.position = this.transform.position + enemy.direction / (farFromCharacter + farFromCharacter);
			}
			
			while (!animator.GetCurrentAnimatorStateInfo(0).IsTag ("Sword"))
			{
				yield return null;
			}
			
			while (_gameManager.swordMin > animator.GetCurrentAnimatorStateInfo(0).normalizedTime ||
			    animator.GetCurrentAnimatorStateInfo(0).normalizedTime >_gameManager.swordMax)
			{
				yield return null;
			}
			
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.swordHit)
			{
				yield return null; 
			}
			
			_gameManager.effectManager.SwordEffectPlayer(enemy, this);
			_soundManager.PlaySound(Sound.Sword);
			Damage(enemy);
			CheckDead(enemy);
			giveHit = GiveHit.normal;
			
			while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsTag ("Hit"))
			{
				yield return null;
			}
            
			while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.swordEndTime)
			{
				yield return null;
			}
			
			if (enemy.activeState != ActiveState.Dead) enemy.GetComponent<CapsuleCollider>().enabled = true;

			if (enemy.activeState == ActiveState.Dead)
			{
				AfterDead(enemy);
			}
			
			enemy.getHit = GetHit.normal;
		}
		
		IEnumerator WaitRangeSkill(Enemy enemy)
		{
			CloseCamera();
			
			animator.SetTrigger(RangeAttack);
			while (!animator.GetCurrentAnimatorStateInfo(0).IsTag ("Range"))
			{
				yield return null;
			}
			
			_gameManager.somethingOn = true;

			enemy.getHit = GetHit.GetHit;
			giveHit = GiveHit.normal;

			while (_gameManager.swordMin > animator.GetCurrentAnimatorStateInfo(0).normalizedTime ||
			       animator.GetCurrentAnimatorStateInfo(0).normalizedTime >_gameManager.rangeMax)
			{
				yield return null;
			}
			
			_soundManager.PlaySound(Sound.EnergyGun);
			
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.rangeHit)
			{
				yield return null; 
			}
			
			_gameManager.effectManager.RangeEffectPlayer(enemy, this);
			Damage(enemy);
			CheckDead(enemy);
			giveHit = GiveHit.normal;
			
			var victory = _gameManager.CheckVictory();

			if (!victory)
			{
				_gameManager.somethingOn = false;
				_gameManager.NextPlayer();
			}

			NonCloseCamera();

			while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsTag ("Hit"))
			{
				yield return null;
			}
            
			while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.rangeEndTime)
			{
				yield return null;
			}

			if (enemy.activeState != ActiveState.Dead) enemy.GetComponent<CapsuleCollider>().enabled = true;
			
			if (enemy.activeState == ActiveState.Dead)
			{
				AfterDead(enemy);
			}
			
			enemy.getHit = GetHit.normal;
			if (!victory) CheckPlayerTurn();
		}

		void Damage(Enemy enemy)
		{
			var damage = (int) Random.Range (baseWeapon.damageMin, baseWeapon.damageMax +1);
			enemy.currentHp = enemy.currentHp - damage;
			enemy.characterBar.Fill(enemy);
		}

		public void MovingSkill( TileNode clickedTile = null)
		{
			if (clickedTile == null) return;
	
			if (clickedTile.tileStyle == TileStyle.OneArea) 
			{
				print("One Vigor");
				currentVigor = currentVigor -1;
			}
			else if (clickedTile.tileStyle == TileStyle.TwoArea || clickedTile.tileStyle == TileStyle.Normal)
			{
				print("Two Vigor");
				currentVigor = currentVigor -2;
			}
			CheckVigor();
		}

		void CheckVigor()
		{
			if (currentVigor <= 0)
			{
				EndPlayer();
			}
		}

		void EndPlayer()
		{
			turnState = TurnState.Ending;
			print(this.name + " is end");
			PlayerOff();
			_board.ResetBoundary();
			_board.line.positionCount = 0;
		}

		void RangeEndPlayer(Enemy enemy)
		{
			EndPlayer();
			_gameManager.ResetRound();
			RangeOff(_gameManager.activeEnemyList);
		}

		public void StartRangeAttack(Enemy enemy)
		{
			enemy.GetComponent<CapsuleCollider>().enabled = false;
			RangeEndPlayer(enemy);
			transform.LookAt(enemy.transform);
			StartCoroutine(WaitRangeSkill(enemy));
		}

		void CheckDead(Enemy enemy)
		{
			if (enemy.currentHp <= 0)
			{
				enemy.animator.SetTrigger (Die);
				enemy.activeState = ActiveState.Dead;
				enemy.GetComponent<CapsuleCollider>().center = Vector3.zero;
				enemy.GetComponent<CapsuleCollider>().height = deadHeight;
				enemy.GetComponent<CapsuleCollider>().enabled = false;
				_gameManager.activeEnemyList.Remove(enemy);
			}
			else
			{
				enemy.transform.LookAt(transform);
				enemy.GetComponent<CapsuleCollider>().enabled = true;
				enemy.animator.SetTrigger (Withstand);
			}
			
			_gameManager.areaCheck.Attention(this);
		}

		void AfterDead(Enemy enemy)
		{
			enemy.characterBar.gameObject.SetActive(false);
			var position = enemy.transform.position;
			iTween.MoveTo(enemy.gameObject, new Vector3(position.x, GameUtility.Disappear, position.z), GameUtility.DisappearTime);
		}

		void OnMouseDown()
		{
			if (canSelect == false) return;
			if (_gameManager.somethingOn == true) return;
			
			if (_gameManager.currentPlayer != this)_playerUi.UiSetActive(false);

			if (characterType == CharacterType.SwordMaster)
			{

			}
			else if (characterType == CharacterType.Ranger)
			{
				
			}
			
			if (turnState == TurnState.Waiting && currentVigor > 0 )
			{
				_gameManager.SelectPlayer(this);
			}
		}
	}
}