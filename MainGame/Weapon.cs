using UnityEngine;

namespace Game.MainGame
{
	public class Weapon : MonoBehaviour {
		private enum WeaponType
		{
			Gun, Sword
		}

		[SerializeField] private WeaponType weaponType;
		BoxCollider _boxCollider;
	
		float _distance = GameUtility.interval / 2;
		Player _player;
		Enemy _enemy;
		readonly Vector3 _pos = new Vector3(0, 0, 0);
		public int damageMin;
		public int damageMax;
		readonly string[] _layerName = new string[]{"Enemy", "Player"};

		void Awake()
		{
			if (_boxCollider == null) _boxCollider = transform.GetChild (0).GetComponent<BoxCollider>();
			EquipSword();
		}

		void Start() 
		{
			RaycastHit[] hit;
			
			// ReSharper disable once Unity.PreferNonAllocAPI
			hit = Physics.BoxCastAll(transform.position + _pos, Vector3.one, Vector3.forward, Quaternion.identity,
				LayerMask.GetMask(_layerName));		

			foreach (var owner in hit)
			{			
				if (owner.transform.CompareTag("Player"))
				{
					_player = owner.transform.GetComponent<Player>();
					_player.baseWeapon = this;
					return;
				}
				else if (owner.transform.CompareTag("Enemy"))
				{
					_enemy = owner.transform.GetComponent<Enemy>();
					_enemy.baseWeapon = this;
					return;
				}
			}
		}

		void EquipGun() 
		{
			weaponType = WeaponType.Gun;
			var gun = new BaseGun();
			damageMin= gun.damageMin;
			damageMax= gun.damageMax;
		}

		void EquipSword()
		{
			weaponType = WeaponType.Sword;
			var sword = new BaseSword();
			damageMin= sword.damageMin;
			damageMax= sword.damageMax;
		}
	}

	public class BaseGun {

		public int damageMin = 4;
		public int damageMax = 5;
	}

	public class BaseSword {

		public int damageMin = 3;
		public int damageMax = 4;
	}
}