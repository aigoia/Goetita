using UnityEngine;
using UnityEngine.UI;

namespace Game.MainGame
{
	public class CharacterBar : MonoBehaviour {
		
		public Player player;
		public Enemy enemy;
		public Image healthBar;
		Transform _block;
		float _thisWidth;
		float _oneBlockPos;
		int _neededBlock;
		[SerializeField] private int bonus = 2;
		[SerializeField] GameObject blockObject;
		GameManager _gameManager;

		void Start()
		{
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (_block == null) _block = transform.Find("Block");
			if (healthBar == null) healthBar = transform.Find("HP").GetComponent<Image>();

			
			if (player != null)
			{
				float currentHp = player.currentHp;
				float baseHP = player.baseHp;
				
				// ReSharper disable once PossibleLossOfFraction
				healthBar.fillAmount = currentHp / baseHP;
				_thisWidth = GetComponent<RectTransform>().rect.width;
				_oneBlockPos = _thisWidth / baseHP;
				_neededBlock = (int) baseHP;
				MakeBlock(player);
			}
			
			if (enemy != null)
			{
				float currentHp = enemy.currentHp;
				float baseHP = enemy.baseHp;
				
				// ReSharper disable once PossibleLossOfFraction
				healthBar.fillAmount = currentHp / baseHP;
				_thisWidth = GetComponent<RectTransform>().rect.width;
				_oneBlockPos = _thisWidth / baseHP;
				_neededBlock = (int) baseHP;
				MakeBlock(enemy);
			}
		}
		

		void MakeBlock(Player thisPlayer)
		{
			// print(oneBlockPos);
			for (int i = 1; i < thisPlayer.baseHp ; i++)
			{
				var newBlock = Instantiate(blockObject, _block.position, Quaternion.identity, _block);
				newBlock.GetComponent<RectTransform>().localPosition = new Vector3(_oneBlockPos * i, 0f, 0f);
			}
		}
		
		void MakeBlock(Enemy thisEnemy)
		{
			// print(oneBlockPos);
			for (int i = 1; i < thisEnemy.baseHp ; i++)
			{
				var newBlock = Instantiate(blockObject, _block.position, Quaternion.identity, _block);
				newBlock.GetComponent<RectTransform>().localPosition = new Vector3(_oneBlockPos * i, 0f, 0f);
			}
		}
	
		public void Fill(Player thisPlayer, int hp)
		{
			float value = (float) hp / (float) thisPlayer.baseHp;
			healthBar.fillAmount = value;
		}
		
		public void Fill(Enemy thisEnemy)
		{
			float value = (float) thisEnemy.currentHp / (float) thisEnemy.baseHp;
			healthBar.fillAmount = value;
		}
	}
}