using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
	public class UiManager : MonoBehaviour {

		GameManager _gameManager;
		[SerializeField] GameObject barObject;
		public Transform canvas;
		[SerializeField] Vector3 posCorrection = new Vector3(0f, 0f, 0f);
		public List<BarNode> barList = new List<BarNode>();
		[SerializeField] Color32 playerColor = new Color32(128, 224, 224, 255);
		[SerializeField] Color32 enemyColor = new Color32(224, 128, 224, 255);

		void Awake()
		{
			if (_gameManager == null) _gameManager = GetComponent<GameManager>();
			if (canvas) canvas = FindObjectOfType<MainCanvas>().transform;
		}

		void Start()
		{
			foreach (var player in _gameManager.activePlayerList)
			{
				MakeHpBar(player, playerColor);
			}

			foreach (var enemy in _gameManager.activeEnemyList)
			{
				MakeHpBar(enemy, enemyColor);
			}
		}
		
		void MakeHpBar(Enemy enemy, Color color)
		{
			if (Camera.main == null) return;
			
			var newClone = Instantiate 
			(
				barObject, 
				Camera.main.WorldToScreenPoint(enemy.transform.position + posCorrection),
				Quaternion.identity, canvas.Find("Hp")
			);

			var newBar = new BarNode{BaseTransform = enemy.transform, BarObject = newClone};
			barList.Add(newBar);
			var hpBar = newClone.GetComponent<CharacterBar>();
			hpBar.healthBar.color = color;
			enemy.characterBar = hpBar;
			hpBar.enemy = enemy;
		}

		void MakeHpBar(Player player, Color color)
		{
			if (Camera.main != null)
			{
				var newClone = Instantiate 
				(
					barObject, 
					Camera.main.WorldToScreenPoint(player.transform.position + posCorrection),
					Quaternion.identity, canvas.Find("Hp")
				);

				var newBar = new BarNode{BaseTransform = player.transform, BarObject = newClone};
				barList.Add(newBar);
				var hpBar = newClone.GetComponent<CharacterBar>();
				hpBar.healthBar.color = color;
				player.characterBar = hpBar;
				hpBar.player = player;
			}
		}

		void Update()
		{
			foreach (var bar in barList)
			{
				if (Camera.main != null)
					bar.BarObject.transform.position =
						Camera.main.WorldToScreenPoint(bar.BaseTransform.position + posCorrection);
			}
		}
	}

	public class BarNode
	{
		public Transform BaseTransform;
		public GameObject BarObject;
	}
}