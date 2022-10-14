using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.MainGame
{
	public enum CharacterType
	{
		Default, Claymore, Ranger,
	}

	public enum GetHit
	{
		Normal, GetHit
	}

	public enum GiveHit
	{
		Normal, GiveHit
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

		public AttackType attackType = AttackType.Non;
		public ResistType resistType = ResistType.Non;
		public int armor = 0;

		[SerializeField] private float farFrom = 2f;

		public int tapId;
		public int characterId;
		public string characterName;
		public bool isImpact = false;

		public bool isClosed = true;
		public bool canSelect = true;
		public List<TileNode> closeList;
		public float deadHeight = 1f;

		public bool hitPlayerAction = false;
		public bool doubleBase = true;

		public UserState userState = UserState.Non;
		public CharacterType characterType = CharacterType.Claymore;
		public int marked = 0;
		public readonly int Mark = 2;
		public bool isThrow = false;
		public UnitClass playerClass = UnitClass.One;
		public Trait trait = Trait.Non;

		public List<Transform> weaponList;
		MainCanvas _canvas;

		// HP
		public int baseHp = 4;
		public int currentHp = 4;
		public int copyHp = 4;
		public CharacterBar characterBar;

		// deal
		// public int weaponDeal = 4;
		public int baseDeal = 0;
		public int plusDeal = 2;

		private Enemy _currentEnemy;

		public int zeroVigor = 0;
		public int oneVigor = 1;
		public int twoVigor = 2;
		public int baseVigor = 2;
		public int currentVigor = 2;

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

		public TurnState turnState = TurnState.Waiting;
		public ActiveState activeState = ActiveState.NotAnything;
		public BeHit beHit = BeHit.Normal;
		public GiveHit giveHit = GiveHit.Normal;

		private static readonly int BaseAttack = Animator.StringToHash("BaseAttack");
		private static readonly int Die = Animator.StringToHash("Die");
		private static readonly int Withstand = Animator.StringToHash("Withstand");

		private static readonly int RangeAttack = Animator.StringToHash("RangeAttack");
		private IEnumerator _checkEnemyDead;
		private static readonly int Breath = Animator.StringToHash("Breath");

		public GameObject weapon;
		public GameObject weaponBack;
		public GameObject weaponSide;
		public GameObject tool;
		public GameObject toolLeft;
		public GameObject toolRight;
		private static readonly int In = Animator.StringToHash("SwordIn");
		private static readonly int Out = Animator.StringToHash("SwordOut");
		private static readonly int RangeIn1 = Animator.StringToHash("RangeIn");
		private static readonly int RangeOut1 = Animator.StringToHash("RangeOut");
		private static readonly int Throw = Animator.StringToHash("SwordThrow");
		private static readonly int RangeThrow1 = Animator.StringToHash("RangeThrow");

		private DeckManager _deckManager;
		private static readonly int SwordBase = Animator.StringToHash("SwordBase");
		private static readonly int SwordCard = Animator.StringToHash("SwordCard");
		private static readonly int SwordPowerFull = Animator.StringToHash("SwordPowerFull");
		private static readonly int RangeShadowWalk = Animator.StringToHash("RangeShadowWalk");
		private static readonly int RangeCard = Animator.StringToHash("RangeCard");
		private static readonly int RangeBase = Animator.StringToHash("RangeBase");

		void Awake()
		{
			if (_currentEnemy == null) _currentEnemy = GetComponent<Enemy>();
			if (_board == null) _board = FindObjectOfType<Board>();
			if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
			currentVigor = baseVigor;
			currentHp = baseHp;
			copyHp = baseHp;
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (_soundManager == null) _soundManager = FindObjectOfType<SoundManager>();
			if (playerMove == null) playerMove = GetComponent<PlayerMove>();
			if (animator == null) animator = GetComponent<Animator>();
			if (_playerUi == null) _playerUi = _gameManager.playerUi;
			if (_canvas == null) _canvas = _gameManager.GetComponent<UIManager>().canvas.GetComponent<MainCanvas>();

			if (_utility == null) _utility = transform.Find("Utility");
			if (moveBaseArea == null) moveBaseArea = _utility.Find("MoveBaseArea").gameObject;
			if (moveDoubleArea == null) moveDoubleArea = _utility.Find("MoveDoubleArea").gameObject;
			if (moveDoubleBlock == null) moveBaseBlock = _utility.Find("MoveBaseBlock").gameObject;
			if (moveDoubleBlock == null) moveDoubleBlock = _utility.Find("MoveDoubleBlock").gameObject;

			if (round == null) round = _utility.Find("Round").gameObject;
			if (rangeArea == null) rangeArea = _utility.Find("DoubleArea").gameObject;

			if (_model == null) _model = transform.Find("Model");
			if (_tail == null) _tail = _utility.Find("Tail").gameObject;

			_deckManager = _gameManager.deckManager;
		}

		private void Start()
		{
			DifferentBreath();

		}

		void DifferentBreath()
		{
			 var random = Random.Range(0.4f, 0.5f);
			 animator.SetFloat(Breath, random);
		}

		bool CheckBaseAttack()
		{
			if (characterType != CharacterType.Claymore) return false;

			foreach (var enemyDirection in GameUtility.EightDirections)
			{
				if (Physics.Raycast(transform.position + Vector3.up, enemyDirection, GameUtility.interval * GameUtility.Alpha, LayerMask.GetMask("Enemy")))
				{
					return true;
				}
			}

			return false;
		}

		public void SwordIn()
		{
			if (weapon == null) return;
			if (weaponBack == null) return;
			if (weaponSide == null) return;
			if (tool == null) return;

			StartCoroutine(SwordInAnimation());
		}

		IEnumerator SwordInAnimation()
		{
			animator.SetTrigger("SwordIn");

			yield return _gameManager.motionManager.wait;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordIn)
			{
				yield return null;
			}

			weapon.SetActive(false);
			weaponBack.SetActive(true);
		}

		public void SwordOut()
		{
			if (weapon == null) return;
			if (weaponBack == null) return;
			if (weaponSide == null) return;
			if (tool == null) return;

			StartCoroutine(SwordOutAnimation());
		}

		IEnumerator SwordOutAnimation()
		{
			animator.SetTrigger("SwordOut");

			yield return _gameManager.motionManager.wait;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordOut)
			{
				yield return null;
			}

			weapon.SetActive(true);
			weaponBack.SetActive(false);
		}

		public void RangeIn()
		{
			if (weapon == null) return;
			if (weaponBack == null) return;
			if (weaponSide == null) return;

			StartCoroutine(RangeInAnimation());
		}

		IEnumerator RangeInAnimation()
		{
			animator.SetTrigger("RangeIn");

			yield return _gameManager.motionManager.wait;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeIn)
			{
				yield return null;
			}

			weapon.SetActive(false);
			weaponSide.SetActive(true);
		}

		public void RangeOut()
		{
			if (weapon == null) return;
			if (weaponBack == null) return;
			if (weaponSide == null) return;

			StartCoroutine(RangeOutAnimation());
		}

		IEnumerator RangeOutAnimation()
		{
			animator.SetTrigger("RangeOut");

			yield return _gameManager.motionManager.wait;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeOut)
			{
				yield return null;
			}

			weapon.SetActive(true);
			weaponSide.SetActive(false);
		}

		public void SwordThrow()
		{
			if (weapon == null) return;
			if (weaponBack == null) return;
			if (weaponSide == null) return;
			if (tool == null) return;
			if (toolLeft == null) return;
			if (toolRight == null) return;

			StartCoroutine(SwordThrowAnimation());
		}

		IEnumerator SwordThrowAnimation()
		{
			var thisPosition = transform.position;
			var thisRotation = transform.rotation;

			isThrow = true;
			animator.SetTrigger("SwordThrow");

			yield return _gameManager.motionManager.wait;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordThrowIn)
			{
				yield return null;
			}

			weapon.SetActive(false);
			weaponBack.SetActive(true);

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordThrowReady)
			{
				yield return null;
			}

			tool.SetActive(false);
			toolLeft.SetActive(true);

			while (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordThrowPre"))
			{
				yield return null;
			}

			toolLeft.SetActive(false);
			toolRight.SetActive(true);

			animator.applyRootMotion = true;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordThrowFly)
			{
				yield return null;
			}

			toolRight.SetActive(false);
			weapon.SetActive(false);
			// toolFly.SetActive(true)

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordThrowOut)
			{
				yield return null;
			}

			weapon.SetActive(true);
			weaponBack.SetActive(false);

			animator.applyRootMotion = false;
			iTween.MoveTo(gameObject, thisPosition, _gameManager.waitTime);
			transform.rotation = thisRotation;

			isThrow = false;
		}

		public void RangeThrow()
		{
			if (weapon == null) return;
			if (weaponBack == null) return;
			if (weaponSide == null) return;
			if (tool == null) return;
			if (toolLeft == null) return;
			if (toolRight == null) return;

			StartCoroutine(RangeThrowAnimation());
		}

		IEnumerator RangeThrowAnimation()
		{
			var thisPosition = transform.position;
			var thisRotation = transform.rotation;

			isThrow = true;
			animator.SetTrigger("RangeThrow");

			yield return _gameManager.motionManager.wait;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeThrowIn)
			{
				yield return null;
			}

			weapon.SetActive(false);
			weaponSide.SetActive(true);

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeThrowReady)
			{
				yield return null;
			}

			tool.SetActive(false);
			toolLeft.SetActive(true);

			while (animator.GetCurrentAnimatorStateInfo(0).IsName("RangeThrowPre"))
			{
				yield return null;
			}

			toolLeft.SetActive(false);
			toolRight.SetActive(true);

			animator.applyRootMotion = true;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeThrowFly)
			{
				yield return null;
			}

			toolRight.SetActive(false);
			weapon.SetActive(false);
			// toolFly.SetActive(true)

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeThrowOut)
			{
				yield return null;
			}

			weapon.SetActive(true);
			weaponSide.SetActive(false);

			animator.applyRootMotion = false;
			iTween.MoveTo(gameObject, thisPosition, _gameManager.waitTime);
			transform.rotation = thisRotation;

			isThrow = false;
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

			enemy.GetComponent<CapsuleCollider>().enabled = false;
			if (Physics.Linecast(transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle", "Player", "Enemy")))
			{
				enemy.GetComponent<CapsuleCollider>().enabled = true;
				return false;
			}
			enemy.GetComponent<CapsuleCollider>().enabled = true;

			RangeSingleOn(enemy);
			return true;
		}

		public List<Enemy> RangeAttackList(Vector3 position)
		{
			RangeOff(_gameManager.activeEnemyList);

			var enemyRangeList = new List<Enemy>();

			var areaEnemyList = _gameManager.areaCheck.RangeEnemyList(this, AreaCheckOption.Round, position);
			if (areaEnemyList == null) return null;

			foreach (var enemy in areaEnemyList)
			{
				if (enemy == null) continue;

				if (enemy.activeState == ActiveState.Dead)
				{
					_gameManager.activeEnemyList.Remove(enemy);
					continue;
				}

				enemy.GetComponent<CapsuleCollider>().enabled = false;
				if (Physics.Linecast(position + Vector3.up, enemy.transform.position + Vector3.up, out _,
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
			enemy.aim.SetActive(true);
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

				enemy.aim.SetActive(true);
				enemy.canTargeted = true;
			}
		}

		public void RangeOff(List<Enemy> enemies)
		{
			foreach (var enemy in enemies)
			{
				enemy.aim.SetActive(false);
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
				if (Physics.Linecast(node.transform.position, testEnemy.transform.position, out _,
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

			if (characterType == CharacterType.Claymore && _gameManager.inPlaceClose) CloseAttackUiCheck();
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
			if (characterType != CharacterType.Claymore) return;
			print(_gameManager.currentPlayer.activeState);

			var currentEnemyList = new List<Enemy>();

			foreach (var enemyDirection in GameUtility.EightDirections)
			{
				if (Physics.Raycast(transform.position + Vector3.up, enemyDirection, out var hit, GameUtility.interval * GameUtility.Alpha, LayerMask.GetMask("Enemy")))
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
			// CloseCamera();
		}

		void EndAllEnemy()
		{
			_currentEnemy = null;
			_board.line.gameObject.SetActive(true);
			_gameManager.PathSetting(true);

			NonCloseCamera();
			_model.position = transform.position;
			currentVigor = zeroVigor;
			EndPlayer();

			_gameManager.currentPlayer = null;
			if (!_gameManager.CheckCloseVictory())
			{
				_gameManager.somethingOn = false;
				// _cardManager.AllReset();
				_gameManager.NextPlayer();
			}
		}

		IEnumerator CloseSelectEnemy(List<Enemy> currentEnemyList, float farFrom = 2f)
		{
			StartAllEnemy();

			if (currentEnemyList == null)
			{
				CheckPlayerTurn();
				print("Enemy List is Empty!!");
				yield break;
			}

			if (currentEnemyList.Count == 0)
			{
				CheckPlayerTurn();
				print("Enemy List is Empty!!");
				yield break;
			}

			GameUtility.ShuffleList(currentEnemyList);
			var enemy = currentEnemyList[0];

			enemy.GetComponent<CapsuleCollider>().enabled = false;
			_gameManager.somethingOn = true;

			var damage = Damage(enemy);

			// critical check
			if (_gameManager.criticalHitCheck)
			{
				_gameManager.mainCanvas.gameObject.SetActive(true);

				var criticalImage = Instantiate(_gameManager.mainCanvas.criticalImage, _gameManager.mainCanvas.transform);

				criticalImage.gameObject.SetActive(true);
				criticalImage.GetComponent<Image>().sprite = _gameManager.settings.SetProfileImage(this, "Full");
				criticalImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
					_gameManager.mainCanvas.codeNameList[characterName];

				iTween.MoveAdd(criticalImage.gameObject, _gameManager.mainCanvas.criticalImageMove, _gameManager.criticalTime);

				yield return new WaitForSeconds(_gameManager.criticalTime);

				DestroyImmediate(criticalImage);
				_gameManager.mainCanvas.gameObject.SetActive(false);
			}

			CloseCamera();

			DamageAccept(enemy, damage);
			var cardType = DeckType.Non;

			if (CardCheck(DeckType.PowerFull))
			{
				animator.SetTrigger(SwordPowerFull);
				cardType = DeckType.PowerFull;
			}
			else if (CardCheck(DeckType.ShadowWalk))
			{
				animator.SetTrigger(SwordCard);
				cardType = DeckType.ShadowWalk;
			}
			else
			{
				animator.SetTrigger(SwordBase);
				cardType = DeckType.Non;
			}

			transform.LookAt(enemy.transform);
			if (enemy != currentEnemyList[0]) _cameraController.RotateButton(1);


			StartCoroutine(WaitCloseSkill(enemy, farFrom, cardType));

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

		IEnumerator WaitCloseSkill(Enemy enemy, float farFromCharacter, DeckType deckType)
		{
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

			while (_gameManager.motionManager.swordMin > animator.GetCurrentAnimatorStateInfo(0).normalizedTime ||
			       animator.GetCurrentAnimatorStateInfo(0).normalizedTime >_gameManager.motionManager.swordMax)
			{
				yield return null;
			}
			

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < SwordHitTime(deckType))
			{
				yield return null;
			}

			_gameManager.effectManager.SwordEffectPlayer(enemy, this);
			_soundManager.PlaySound(isImpact == true ? Sound.Impact : Sound.Sword);

			CheckDead(enemy);
			giveHit = GiveHit.Normal;

			_deckManager.AllReset();
			while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsTag ("Hit"))
			{
				yield return null;
			}


			while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordEndTime)
			{
				yield return null;
			}

			if (enemy.activeState != ActiveState.Dead) enemy.GetComponent<CapsuleCollider>().enabled = true;

			if (enemy.activeState == ActiveState.Dead)
			{
				AfterDead(enemy);
			}

			enemy.getHit = GetHit.Normal;
			turnState = TurnState.Ending;

			// _cardManager.AllReset();
		}

		float SwordHitTime(DeckType deckType)
		{
			if (deckType == DeckType.Non)
			{
            	return _gameManager.motionManager.swordHit;			
			}
			else if (deckType == DeckType.PowerFull)
			{
				return _gameManager.motionManager.swordHitPowerFull;
			}
			else if (deckType == DeckType.ShadowWalk)

			{
				return _gameManager.motionManager.swordHitCard;
			}

			return _gameManager.motionManager.swordHit;
		}
		
		float RangeHitTime(DeckType deckType)
		{
			if (deckType == DeckType.Non)
			{
				return _gameManager.motionManager.rangeHit;			
			}
			else if (deckType == DeckType.PowerFull)
			{
				return _gameManager.motionManager.rangeHitCard;
			}
			else if (deckType == DeckType.ShadowWalk)

			{
				return _gameManager.motionManager.rangeHitShadow;
			}

			return _gameManager.motionManager.rangeHit;
		}


		bool CardCheck(DeckType deckType)
		{
			foreach (var card in _deckManager.cardList)
			{
				if (card.deckType == deckType)
				{
					if (card.cardOn) return true;
				}
			}

			return false;
		}

		int CardCount(DeckType deckType)
		{
			int i = 0;
			foreach (var card in _deckManager.cardList)
			{
            	if (card.deckType == deckType)
            	{
            		i += 1;
            	}
            }

			return i;
		}

		IEnumerator WaitRangeSkill(Enemy enemy)
		{
			enemy.GetComponent<CapsuleCollider>().enabled = false;
			_gameManager.somethingOn = true;

			var damage = Damage(enemy);

			// critical check
			if (_gameManager.criticalHitCheck)
			{
				_gameManager.mainCanvas.gameObject.SetActive(true);

				var criticalImage = Instantiate(_gameManager.mainCanvas.criticalImage, _gameManager.mainCanvas.transform);



				criticalImage.gameObject.SetActive(true);
				criticalImage.GetComponent<Image>().sprite = _gameManager.settings.SetProfileImage(this, "Full");
				criticalImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
					_gameManager.mainCanvas.codeNameList[characterName];

				iTween.MoveAdd(criticalImage.gameObject, _gameManager.mainCanvas.criticalImageMove, _gameManager.criticalTime);

				yield return new WaitForSeconds(_gameManager.criticalTime);

				DestroyImmediate(criticalImage);

				_gameManager.mainCanvas.gameObject.SetActive(false);
			}

			DamageAccept(enemy, damage);

			CloseCamera();
			
			var cardType = DeckType.Non;

			
			if (CardCheck(DeckType.ShadowWalk))
			{
				animator.SetTrigger(RangeShadowWalk);
				cardType = DeckType.ShadowWalk;
			}
			else if (CardCheck(DeckType.PowerFull))
            {
            	animator.SetTrigger(RangeCard);
            	cardType = DeckType.PowerFull;
            }
			else
			{
				animator.SetTrigger(RangeBase);
				cardType = DeckType.Non;
			}

			while (!animator.GetCurrentAnimatorStateInfo(0).IsTag ("Range"))
			{
				yield return null;
			}

			enemy.getHit = GetHit.GetHit;
			giveHit = GiveHit.Normal;

			while (_gameManager.motionManager.swordMin > animator.GetCurrentAnimatorStateInfo(0).normalizedTime ||
			       animator.GetCurrentAnimatorStateInfo(0).normalizedTime >_gameManager.motionManager.rangeMax)
			{
				yield return null;
			}

			if (cardType == DeckType.ShadowWalk)
			{
				
			}
			else
			{
				_soundManager.PlaySound(Sound.OneShot);
			}
			
			
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < RangeHitTime(cardType))
			{
				yield return null;
			}
			
			if (cardType == DeckType.ShadowWalk)
			{
				_soundManager.PlaySound(Sound.Sword);
				_gameManager.effectManager.SwordEffectPlayer(enemy, this);
				// transform.LookAt(transform.position + Vector3.back * 180);
			}
			else
			{
				_gameManager.effectManager.RangeEffectPlayer(enemy, this);
			}
			

			CheckDead(enemy);
			giveHit = GiveHit.Normal;

			var victory = _gameManager.CheckVictory();

			if (!victory)
			{
				_gameManager.somethingOn = false;
				_gameManager.NextPlayer();
			}

			NonCloseCamera();

			// camera red
			if (characterType != CharacterType.Claymore)
			{
				if (CardCheck(DeckType.ShadowWalk))
				{

				}
				else
				{
					marked = Mark;
					_cameraController.CheckMark();
				}
			}

			_deckManager.AllReset();
			while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsTag ("Hit"))
			{
				yield return null;
			}

			while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeEndTime)
			{
				yield return null;
			}

			if (enemy.activeState != ActiveState.Dead) enemy.GetComponent<CapsuleCollider>().enabled = true;

			if (enemy.activeState == ActiveState.Dead)
			{
				AfterDead(enemy);
			}

			enemy.getHit = GetHit.Normal;
			turnState = TurnState.Ending;
			if (!victory) CheckPlayerTurn();

			// _cardManager.AllReset();
		}

		public void MovingSkill( TileNode clickedTile = null)
		{
			if (clickedTile == null) return;

			if (clickedTile.tileStyle == TileStyle.OneArea)
			{
				print("One Vigor");
				currentVigor = currentVigor - oneVigor;
			}
			else if (clickedTile.tileStyle == TileStyle.TwoArea || clickedTile.tileStyle == TileStyle.Normal)
			{
				print("Two Vigor");
				currentVigor = zeroVigor;
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

		int Damage(Enemy enemy)
		{
			int damage = 0;
			int cardBaseDeal = 0;
			int powerfullDeal = 0;

			// card base deal
			if (characterType == CharacterType.Claymore)
			{
				foreach (var card in _deckManager.cardList)
				{
					if (card.cardOn == false) continue;

					if (card.deckType == DeckType.ShadowWalk)
					{
						cardBaseDeal += _deckManager.cardDeal;
					}
				}
			}
			else if (characterType == CharacterType.Ranger)
			{
				foreach (var card in _deckManager.cardList)
				{
					if (card.cardOn == false) continue;

					if (card.deckType == DeckType.PowerFull)
					{
						cardBaseDeal += _deckManager.cardDeal;
					}
				}
			}

			int plusDamage = (int) Random.Range(0, plusDeal + _gameManager.normalDifficult);
			float floatPlusDamage = plusDamage;
			float floatPlusDeal = plusDeal;
			float damagePercent =  (floatPlusDamage / floatPlusDeal) * 100;
			
			// critical hit check
            _gameManager.criticalHitCheck = false;
            if (damagePercent > _gameManager.criticalPercent)
            {
                _gameManager.criticalHitCheck = true;

                if (currentHp == baseHp)
                {
                	plusDamage = (int)((float)plusDamage * _gameManager.criticalDamageUpMin);
                }
                else if (currentHp == 1)
                {
                	plusDamage = (int)((float)plusDamage * _gameManager.criticalDamageUpNormal);
                }
                else
                {
                	plusDamage = (int)((float)plusDamage * _gameManager.criticalDamageUpMax);
                }
            }

            // powerful skill
            if (characterType == CharacterType.Claymore)
            {
            	foreach (var card in _deckManager.cardList)
            	{
            		if (card.cardOn == false) continue;

            		if (card.deckType == DeckType.PowerFull)
            		{
	                    plusDamage = 0;
	                    powerfullDeal = (int) (plusDeal * _gameManager.powerfulDeal);
                        _gameManager.criticalHitCheck = false;
                    }
            	}
            }
            
            print("Damage :" + plusDamage + " + " + baseDeal + " + " + cardBaseDeal + " + " + powerfullDeal);
			damage = plusDamage + baseDeal + cardBaseDeal + powerfullDeal;
			
			if (this.attackType == AttackType.ArmorPiercing)
			{
				int newArmor =  (int)(enemy.armor * _gameManager.armorPiercing - _gameManager.plusArmorPiercing);
				damage = damage - newArmor;
			}
			else if (this.attackType == AttackType.Physics)
			{
				damage = damage - enemy.armor;
			}
			else if (this.attackType == AttackType.Magic)
			{
				if (enemy.resistType == ResistType.Magic)
				{
					damage = (int)(damage * _gameManager.magicResist);
				}
				else
				{
					damage = damage;
				}
			}
			else if (this.attackType == AttackType.Non)
			{
				damage = damage - enemy.armor;
			}
			
			if (damage <= 0) damage = _gameManager.minimalDamage;
			print("Final " + damage);
			return damage;
		}

		void DamageAccept(Enemy enemy, int damage)
		{
			enemy.currentHp = enemy.currentHp - damage;
			enemy.characterBar.Fill(enemy);

			if (enemy.currentHp <= 0)
			{
				enemy.GetComponent<CapsuleCollider>().enabled = false;
				enemy.sight.SetActive(false);
				_gameManager.activeEnemyList.Remove(enemy);
				_gameManager.MarkedDead(enemy, this);
			}

			currentVigor = 0;
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

			}
			else
			{
				enemy.transform.LookAt(transform);
				marked = Mark;
				_gameManager.Marked();
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

		void OnMouseUp()
		{
			if (_gameManager.soundManager.audioManager != null) _gameManager.soundManager.audioManager.click = true;
			// if (_gameManager.soundManager.audioManager != null) _gameManager.soundManager.audioManager.VolumeUp();

			foreach (var player in _gameManager.playerList)
			{
				player.round.SetActive(false);
			}

			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (canSelect == false) return;
			if (_gameManager.somethingOn == true) return;

			if (_gameManager.currentPlayer != this)_playerUi.UiSetActive(false);

			if (characterType == CharacterType.Claymore)
			{

			}
			else if (characterType == CharacterType.Ranger)
			{

			}

			if (turnState == TurnState.Waiting && currentVigor > 0 )
			{
				// _soundManager.PlaySystem(SoundSystem.Click);
				_gameManager.SelectPlayer(this);
			}
		}
	}
}
