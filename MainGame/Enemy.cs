using System.Collections;
using System.Collections.Generic;
// using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Game.MainGame
{
	public enum BeHit
	{
		Normal, BeHit
	}

	public enum BuyHit
	{
		Normal,
		BuyHit
	}

	public enum ResistType
	{
		Non, Magic
	}

	public enum AttackType
	{
		Non, Physics, Magic, ArmorPiercing	
	}

	public class Enemy : MonoBehaviour
	{
		public bool battle = false;

		public int grade = 0;
		public AttackType attackType = AttackType.Non;
		public ResistType resistType = ResistType.Non;
		public int armor = 0;

		public bool isImpact;

		// HP
		public int baseHp = 4;
		public int currentHp = 4;
		
		public CharacterBar characterBar;
		public GameObject aim;

		// deal
		public int baseDeal = 4;
		public int plusDeal = 1;
		
		public int baseVigor = 2;
		public int currentVigor = 2;
		[SerializeField] private float alpha = 1f;
		public float deadHeight = 1f;

		public CharacterType characterType = CharacterType.Default;
		
		public bool canTargeted = false;
		public EnemyMove enemyMove;
		GameManager _gameManager;
		Board _board;
		public EnemyAi enemyAi;
		PathFinding _pathFinding;
		public Animator animator;
		CameraController _cameraController;
		private SoundManager _soundManager;
		public Vector3 direction;
		Transform _model;
		public bool noWay = false;

		public GameObject moveBaseArea;
		public GameObject moveDoubleArea;
		public List<TileNode> baseRangeArea;
		public List<TileNode> rangeArea;
		public GameObject sight;

		public TurnState turnState = TurnState.Waiting;
		public ActiveState activeState = ActiveState.NotAnything;
		public GetHit getHit = GetHit.Normal;
		public BuyHit buyHit = BuyHit.Normal;

		private static readonly int BaseAttack = Animator.StringToHash("BaseAttack");
		private static readonly int Die = Animator.StringToHash("Die");
		private static readonly int Withstand = Animator.StringToHash("Withstand");
		private static readonly int RangeAttack = Animator.StringToHash("RangeAttack");
		private static readonly int Breath = Animator.StringToHash("Breath");

		public UnitClass enemyClass = UnitClass.One;
		public Trait trait = Trait.Non;
		
		void Awake()
		{
			currentHp = baseHp;
			if (animator == null) animator = GetComponent<Animator>();
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (_soundManager == null) _soundManager = FindObjectOfType<SoundManager>();
			if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
			if (_board == null) _board = FindObjectOfType<Board>();
			if (enemyAi == null) enemyAi = FindObjectOfType<EnemyAi>();
			if (enemyMove) enemyMove = GetComponent<EnemyMove>();
			if (moveBaseArea == null) moveBaseArea = transform.Find("Utility").Find("MoveBaseArea").gameObject;
			if (moveDoubleArea == null) moveDoubleArea = transform.Find("Utility").Find("MoveDoubleArea").gameObject;
			if (_model == null) _model = transform.Find("Model");
			if (aim == null) aim = transform.Find("Utility").Find("Range").gameObject;
		}

		[ContextMenu("Setting")]
		public void Setting()
		{
			animator = GetComponent<Animator>(); 
			aim = moveBaseArea = transform.Find("Utility").Find("Range").gameObject;
			moveBaseArea = transform.Find("Utility").Find("MoveBaseArea").gameObject;
			_model = transform.Find("Model");
			moveDoubleArea = transform.Find("Utility").Find("MoveDoubleArea").gameObject;
			_board = FindObjectOfType<Board>();
			enemyAi = FindObjectOfType<EnemyAi>();
			enemyMove = GetComponent<EnemyMove>();
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

		public bool UnitWalk(AreaOrder areaOrder, WalkOrder walkOrder, TileNode endNode)
		{
			// print(currentVigor);
			if (currentVigor == enemyAi.emptyVigor) return false;
			
			TileNode newNode = endNode;

			if (walkOrder == WalkOrder.Random)
			{
				var areaList = new List<TileNode>();
				
				if (areaOrder == AreaOrder.Base)
				{
					moveBaseArea.SetActive(true);
				}
				else if (areaOrder == AreaOrder.Double)
				{
					moveDoubleArea.SetActive(true);
				}
				else 
				{
					print("Wrong order for RandomWalk");
				}
				
				foreach (var node in _board.NodeList)
				{
					if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("Obstacle", "Player", "Enemy")))
					{
						continue;
					}
					if (areaOrder == AreaOrder.Base)
					{
						if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("MoveBaseArea")))
						{
							node.tileStyle = TileStyle.OneArea;
							areaList.Add(node);
						}
					}
					else if (areaOrder == AreaOrder.Double)
					{
						if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleArea")))
						{
							node.tileStyle = TileStyle.TwoArea;
							areaList.Add(node);
						}
					}
				}
				
				int randomNumber = Random.Range(0, areaList.Count -1);
				newNode = areaList[randomNumber];
				
				if (areaOrder == AreaOrder.Base)
				{
					moveBaseArea.SetActive(false);
				}
				else if (areaOrder == AreaOrder.Double)
				{
					moveDoubleArea.SetActive(false);
				}
			}

			else if (walkOrder == WalkOrder.Base || walkOrder == WalkOrder.Rush)
			{
				newNode = endNode;
			}

			var canMove = MovingSkill(newNode, _board.NodeList, areaOrder, walkOrder);
			return canMove;
		}

		public void CloseAttackByEnemy(Player player, float farFrom = 1)
		{	
			player.beHit = BeHit.BeHit;
			animator.SetTrigger(BaseAttack);
			transform.LookAt(player.transform);

			StartCoroutine(WaitCloseSkill(player, farFrom));
		}

		public void RangeAttackByEnemy(Player player)
		{
			transform.LookAt(player.transform);
			player.beHit = BeHit.Normal;
			StartCoroutine(WaitRangeSkill(player));
		}

		void DamageCopy(Player player)
		{
			player.copyHp = Damage(player, player.copyHp);
			
			player.characterBar.Fill(player, player.copyHp);
		}
		
		void CheckDead(Player player)
		{
			print("Copy HP : " + player.copyHp);
			print("Current HP : " + player.currentHp);
			
			if (player.copyHp <= 0)
			{
				player.activeState = ActiveState.Dead;
				player.animator.SetTrigger(Die);
				player.GetComponent<CapsuleCollider>().center = Vector3.zero;
				player.GetComponent<CapsuleCollider>().height = deadHeight;
				player.GetComponent<CapsuleCollider>().enabled = false;
			}
			else
			{
				player.transform.LookAt(transform);
				player.GetComponent<CapsuleCollider>().enabled = true;
				player.animator.SetTrigger(Withstand);
			}
			
			_gameManager.areaCheck.Attention(this);
		}
		
		public int Damage(Player player, int hp)
		{
			var newHp = hp;
			
			int damage = 0;

			if (this.attackType == AttackType.ArmorPiercing)
			{
				int newArmor =  (int)(player.armor * _gameManager.armorPiercing);
				damage = (int)Random.Range(0, plusDeal) + baseDeal - newArmor - _gameManager.plusArmorPiercing;
			}
			else if (this.attackType == AttackType.Physics)
			{
				damage = (int)Random.Range(0, plusDeal) + baseDeal - player.armor;
			}
			else if (this.attackType == AttackType.Magic)
			{
				if (player.resistType == ResistType.Magic)
				{
					damage = (int)((int)Random.Range(0, plusDeal) * _gameManager.magicResist) + baseDeal;
				}
				else
				{
					damage = (int)Random.Range(0, plusDeal) + baseDeal;	
				}
			}
			else if (this.attackType == AttackType.Non)
			{
				damage = (int)Random.Range(0, plusDeal) + baseDeal - player.armor;
			}

			if (damage <= 0) damage = _gameManager.minimalDamage;
            
			player.currentHp = player.currentHp - damage;
			
			newHp = newHp - damage;
			// print(player.name + " : " +player.currentHp)

			if (newHp <= 0)
			{
				var newList = new List<Player>();
				
				foreach (var unit in _gameManager.activePlayerList)
				{
					if (unit.name != player.name)
					{
						newList.Add(unit);
					}
				}
				
				_gameManager.activePlayerList = newList;
			}

			return newHp;
		}
		
		IEnumerator WaitCloseSkill(Player player, float farFrom)
		{
			battle = true;
			_model.position = transform.position;

			if (player.direction == Vector3.forward ||
			    player.direction == Vector3.back ||
			    player.direction == Vector3.left ||
			    player.direction == Vector3.right)
			{
				_model.position = transform.position - player.direction / farFrom;
			}
			else 
			{
				_model.position = transform.position + player.direction / (farFrom + farFrom);
			}
			
			while (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Sword"))
			{
				yield return null;
			}

			player.beHit = BeHit.BeHit;
			buyHit = BuyHit.BuyHit;
			
			while (_gameManager.motionManager.swordMin > animator.GetCurrentAnimatorStateInfo(0).normalizedTime ||
			       animator.GetCurrentAnimatorStateInfo(0).normalizedTime >_gameManager.motionManager.swordMax)
			{
				yield return null;
			}
			
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordHitEnemy)
			{
				yield return null; 
			}
			
			_gameManager.effectManager.SwordEffectEnemy(player, this);
			// _soundManager.PlaySound(Sound.Sword);
			_soundManager.PlaySound(isImpact == true ? Sound.Impact : Sound.Sword);

			
			DamageCopy(player);
			CheckDead(player);
			buyHit = BuyHit.Normal;

			while (!player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
			{
				yield return null;
			}

			var thisTime = player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				
			while (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.swordEndTimeEnemy)
			{
				yield return null;
			}

			_model.position = transform.position;
			player.beHit = BeHit.Normal;
			battle = false;
			if (player.activeState == ActiveState.Dead)
			{
				AfterDead(player);
			}
			
			currentVigor = enemyAi.emptyVigor;
		}

		IEnumerator WaitRangeSkill(Player player)
		{
			battle = true;
			// Damage(player);
			
			transform.LookAt(player.transform);
			animator.SetTrigger(RangeAttack);
			while (!animator.GetCurrentAnimatorStateInfo(0).IsTag ("Range"))
			{
				yield return null;
			}
			
			player.beHit = BeHit.BeHit;
			buyHit = BuyHit.BuyHit;

			while (_gameManager.motionManager.swordMin > animator.GetCurrentAnimatorStateInfo(0).normalizedTime ||
			       animator.GetCurrentAnimatorStateInfo(0).normalizedTime >_gameManager.motionManager.rangeMax)
			{
				yield return null;
			}
			
			_soundManager.PlaySound(Sound.Gun);
			
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeHit)
			{
				yield return null; 
			}
			
			_gameManager.effectManager.RangeEffectEnemy(player, this);
			
			DamageCopy(player);
			CheckDead(player);

			while (!player.animator.GetCurrentAnimatorStateInfo(0).IsTag ("Hit"))
			{
				yield return null;
			}
			
			buyHit = BuyHit.Normal;

			while (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _gameManager.motionManager.rangeEndTime)
			{
				yield return null;
			}

			if (player.activeState != ActiveState.Dead) player.GetComponent<CapsuleCollider>().enabled = true;
				
			player.beHit = BeHit.Normal;
			battle = false;
			if (player.activeState == ActiveState.Dead)
			{
				AfterDead(player);
			}
			
			currentVigor = enemyAi.emptyVigor;
		}

		public void RenewalPlayerList()
		{
			var newList = new List<Player>();
			
			foreach (var player in _gameManager.activePlayerList)
            {
            	if (player.activeState != ActiveState.Dead || player.currentHp <= 0)
            	{
            		newList.Add(player);
            	}
            }
			
			_gameManager.activePlayerList = newList;
		}

		void AfterDead(Player player)
		{
			player.characterBar.gameObject.SetActive(false);
			var position = player.transform.position;

			RenewalPlayerList();
			
			iTween.MoveTo(player.gameObject, new Vector3(position.x, GameUtility.Disappear, position.z), GameUtility.DisappearTime);
		}

		private bool MovingSkill(TileNode endNode, List<TileNode> allNodeList, AreaOrder areaOrder, WalkOrder walkOrder)
		{
			
			if (endNode == null) 
			{
				print("EndNode : " + endNode);
				return false;
			}
			
			var enemy = enemyAi.thisEnemy;
			if (enemy == null) 
			{
				print("Enemy : " + enemy);
				return false;
			}

			var startNode = allNodeList.Find(i => i.Coordinate == GameUtility.Coordinate (enemy.transform.position));
			if (startNode == null) 
			{
				print("StartNode : " + startNode );
				return false;
			}
			
			var way = _board.PathFinding.GreedPathFinding(startNode, endNode, _board.NodeList);
			if (way == null)
			{
				return false;
			}

			if (_cameraController.rangeLevel == 3)
			{
				enemy.transform.position = way[0].transform.position;
				return true;
			}
			if (_gameManager.audioManager != null)
			{
				if (!_gameManager.audioManager.enemyMove)
				{
					enemy.transform.position = way[0].transform.position;
					return true;	
				}
			}

			if (Vector3.Distance(_cameraController.transform.position, way[0].transform.position) < _cameraController.area)
			{ 
				enemy.enemyMove.IndicateUnit(way, areaOrder, walkOrder);	
			}
			else if (Vector3.Distance(_cameraController.transform.position, this.transform.position) < _cameraController.area)
			{
				enemy.enemyMove.IndicateUnit(way, areaOrder, walkOrder);	
			}
				
			else
			{
				enemy.transform.position = way[0].transform.position;
			}
			
			return true;
		}

		void AttackRangeByPlayer(Enemy enemy)
		{
			if (_gameManager.currentPlayer.characterType != CharacterType.Ranger) return;
			if (!_gameManager.currentPlayer.CheckRangeSingle(enemy, _gameManager.currentPlayer)) return;

			if (enemy.canTargeted && _gameManager.currentPlayer.currentVigor > 0)
			{
				// _soundManager.PlaySystem(SoundSystem.Click);
				_gameManager.currentPlayer.StartRangeAttack(enemy);
			}
		}
		
		private void OnMouseEnter()
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (_gameManager.currentPlayer == null) return;
			if (_gameManager.currentPlayer.characterType != CharacterType.Ranger) return;
			if (_gameManager.somethingOn) return;

			_gameManager.PathSetting(false);
			var hover = _gameManager.currentPlayer.CheckRangeSingle(this, _gameManager.currentPlayer);
			
			if (hover)
			{
				// _soundManager.PlaySystem(SoundSystem.Hover);
			}
		}

		void OnMouseUp()
		{
			if (EventSystem.current.IsPointerOverGameObject()) return;
			if (_gameManager.currentPlayer == null) return;

			AttackRangeByPlayer(this);
		}
	}
}