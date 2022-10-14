using System.Collections.Generic;
using UnityEngine;

namespace Game.MainGame
{
	public enum AreaCheckOption
	{
		Round, Player,
	}

	public class AreaCheck : MonoBehaviour
	{
		public float sightCheck = 4f;
		private GameManager _gameManager;
		private Board _board;

		private void Awake()
		{
			if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
			if (_board == null) _board = FindObjectOfType<Board>();
		}

		public List<TileNode> NearNode(Enemy enemy)
		{
			if (enemy == null) return null;
			var nearNode = new List<TileNode>();

			foreach (var direction in GameUtility.Directions)
			{
				Physics.Raycast(enemy.transform.position, direction, out var hit, GameUtility.interval);

				if (hit.transform == null) continue;
				if (hit.transform.GetComponent<TileNode>().tileStyle == TileStyle.NonWalkable) continue;

				nearNode.Add(hit.transform.GetComponent<TileNode>());
			}

			return nearNode;
		}

		public TileNode GetNode(Enemy enemy)
		{
			if (enemy == null) return null;

			Physics.Raycast(enemy.transform.position, Vector3.up, out var hit, GameUtility.interval);

			if (hit.transform == null) return null;
			if (hit.transform.GetComponent<TileNode>().tileStyle == TileStyle.NonWalkable) return null;

			return hit.transform.GetComponent<TileNode>();
		}

		public List<TileNode> BaseNode(Enemy enemy)
		{
			var baseNode = new List<TileNode>();
			enemy.moveBaseArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, _board.box, Quaternion.identity, LayerMask.GetMask("MoveBaseArea")))
				{
					baseNode.Add(node);
				}
			}

			enemy.moveBaseArea.SetActive(false);

			return baseNode;
		}

		public List<TileNode> NearNode(Player player)
		{
			if (player == null) return null;
			var newList = new List<TileNode>();

			foreach (var direction in GameUtility.Directions)
			{
				Physics.Raycast(player.transform.position, direction, out var hit, GameUtility.interval);

				if (hit.transform == null) continue;
				if (hit.transform.GetComponent<TileNode>() == null) continue;
				if (hit.transform.GetComponent<TileNode>().tileStyle == TileStyle.NonWalkable) continue;

				newList.Add(hit.transform.GetComponent<TileNode>());
			}

			return newList;
		}

		public bool NearEnemyCheck(Player player)
		{
			if (player == null) return false;

			foreach (var direction in GameUtility.Directions)
			{
				Physics.Raycast(player.transform.position + Vector3.up, direction, out var hit, GameUtility.interval);

				if (hit.transform == null) continue;

				if (hit.transform.CompareTag("Enemy"))
				{

					return true;
				}
			}

			return false;
		}

		public void Attention(Enemy enemy)
		{
			foreach (var enemyDirection in GameUtility.EightDirections)
			{
				if (Physics.Raycast(enemy.transform.position + Vector3.up, enemyDirection, out var hit, GameUtility.interval * GameUtility.Alpha, LayerMask.GetMask("Player")))
				{
					if (hit.transform == null) continue;
					if (hit.transform.GetComponent<Player>().activeState == ActiveState.Dead) continue;
					hit.transform.LookAt(enemy.transform);
					_gameManager.Marked();
				}
			}
		}

		public void Attention(Player player)
		{
			foreach (var enemyDirection in GameUtility.EightDirections)
			{
				if (Physics.Raycast(player.transform.position + Vector3.up, enemyDirection, out var hit, GameUtility.interval * GameUtility.Alpha, LayerMask.GetMask("Enemy")))
				{
					if (hit.transform == null) continue;
					if (hit.transform.GetComponent<Enemy>().activeState == ActiveState.Dead) continue;
					// player.marked = player.Mark;
					hit.transform.LookAt(player.transform);
					_gameManager.Marked();
				}
			}
		}

		public List<Player> BaseList(Enemy enemy)
		{
			var baseList = new List<Player>();
			_board.ResetEnemyBoard(enemy);

			var areaList = new List<TileNode>();

			enemy.moveBaseArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveBaseArea")))
				{
					areaList.Add(node);
				}
			}

			enemy.moveBaseArea.SetActive(false);

			foreach (var node in areaList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("Player")))
				{
					foreach (var player in _gameManager.activePlayerList)
					{
						if (player.marked > 0)
						{
							if (player.currentHp > 0 || player.activeState != ActiveState.Dead)
							{
								if (player.transform.position == node.transform.position)
								{
									if (enemy.characterType == CharacterType.Claymore)
									{
										// var way = _board.PathFinding.GreedPathFinding(GetNode(enemy), node, _board.NodeList);
										// if (way == null) continue;

										baseList.Add(player);

									}
									else if (enemy.characterType == CharacterType.Ranger)
									{
										baseList.Add(player);
									}
								}
							}
						}
					}
//					baseList.Add(_gameManager.activePlayerList.Find(i => i.transform.position == node.transform.position && i.marked > 0));
				}
			}

			return baseList;
		}

		public List<Player> DoubleList(Enemy enemy, List<Player> baseList = null)
		{
			var doubleList = new List<Player>();
			_board.ResetEnemyBoard(enemy);

			var areaList = new List<TileNode>();
			enemy.moveDoubleArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveDoubleArea")))
				{
					areaList.Add(node);
				}
			}

			enemy.moveDoubleArea.SetActive(false);

			foreach (var node in areaList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("Player")))
				{
					foreach (var player in _gameManager.activePlayerList)
					{
						if (player.marked > 0)
						{
							if (player.currentHp > 0 || player.activeState != ActiveState.Dead)
							{
								if (enemy.characterType == CharacterType.Claymore)
								{
									// var way = _board.PathFinding.GreedPathFinding(GetNode(enemy), node, _board.NodeList);
									// if (way == null) continue;

									if (player.transform.position == node.transform.position) doubleList.Add(player);
								}
								else if (enemy.characterType == CharacterType.Ranger)
								{
									if (player.transform.position == node.transform.position) doubleList.Add(player);
								}

							}
						}
					}
				}
			}

			if (baseList == null) return doubleList;

			var onlyDoubleList = new List<Player>();

			foreach (var player in doubleList)
			{
				if (!baseList.Contains(player)) onlyDoubleList.Add(player);
			}

			return onlyDoubleList;
		}

		public List<Player> ShootPlayerList(List<Player> allPlayers, Enemy enemy)
		{
			//	Shoot means after Moving and Attack
			var playerList = new List<Player>();
			var areaList = new List<TileNode>();

            enemy.moveBaseArea.SetActive(true);

            foreach (var node in _board.NodeList)
            {
	            if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
		            LayerMask.GetMask("MoveBaseArea")))
	            {
		            areaList.Add(node);
	            }
            }

            enemy.moveBaseArea.SetActive(false);

            foreach (var player in allPlayers)
            {
	            if (player.marked <= 0) continue;

	            player.moveDoubleArea.SetActive(true);

	            foreach (var node in areaList)
	            {
		            if (node.tileStyle == TileStyle.NonWalkable) continue;

		            if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
			            LayerMask.GetMask("MoveDoubleArea")))
		            {
			            playerList.Add(player);
			            break;
		            }
	            }

	            player.moveDoubleArea.SetActive(false);
            }

			return playerList;
		}

		public List<Player> ChasePlayerList(List<Player> allPlayers, Enemy enemy)
		{
			var playerList = new List<Player>();
			var areaList = new List<TileNode>();

			enemy.moveDoubleArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveDoubleArea")))
				{
					areaList.Add(node);
				}
			}

			enemy.moveDoubleArea.SetActive(false);

			foreach (var player in allPlayers)
			{
				if (player.marked <= 0) continue;

				player.moveBaseArea.SetActive(true);

				foreach (var node in areaList)
				{
					if (node.tileStyle == TileStyle.NonWalkable) continue;

					if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
						LayerMask.GetMask("MoveBaseArea")))
					{
						playerList.Add(player);
						break;
					}
				}

				player.moveBaseArea.SetActive(false);
			}

			return playerList;
		}

		public bool CanMove(Enemy enemy, TileNode tileNode)
		{
			if (tileNode == null) return false;
			if (enemy == null) return false;

			if (tileNode.tileStyle == TileStyle.NonWalkable) return false;
			if (_board.CheckObstacle(tileNode, enemy.transform.position)) return false;

			return true;
		}

		public bool CanMove(Player player, TileNode tileNode)
		{
			if (player == null) return false;
			if (tileNode == null) return false;

			if (tileNode.tileStyle == TileStyle.NonWalkable) return false;
			if (_board.CheckObstacle(tileNode, player.transform.position)) return false;

			return true;
		}

		public bool CanMove(TileNode startNode, TileNode endNode)
		{
			if (startNode == null) return false;
			if (endNode == null) return false;

			if (endNode.tileStyle == TileStyle.NonWalkable) return false;
			if (_board.CheckObstacle(startNode, endNode.transform.position)) return false;

			return true;
		}

		public bool CanRange(Player player, Enemy enemy)
		{
			if (player == null) return false;
			if (enemy == null) return false;
			if (player.activeState == ActiveState.Dead) return false;
			if (player.copyHp <= 0) return false;
			if (player.currentHp <= 0) return false;

			player.GetComponent<CapsuleCollider>().enabled = false;
			enemy.GetComponent<CapsuleCollider>().enabled = false;

			if (Physics.Linecast(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle", "Player", "Enemy")))
			{
				player.GetComponent<CapsuleCollider>().enabled = true;
				enemy.GetComponent<CapsuleCollider>().enabled = true;
				return false;
			}

			player.GetComponent<CapsuleCollider>().enabled = true;
			enemy.GetComponent<CapsuleCollider>().enabled = true;
			return true;
		}

		public bool CanRange(Player player, TileNode tileNode)
		{
			if (player == null) return false;
			if (tileNode == null) return false;
			if (player.activeState == ActiveState.Dead) return false;
			if (player.copyHp <= 0) return false;
			if (player.currentHp <= 0) return false;

			player.GetComponent<CapsuleCollider>().enabled = false;

			if (Physics.Linecast(player.transform.position + Vector3.up, tileNode.transform.position + Vector3.up, out var hit,
				LayerMask.GetMask("Obstacle", "Player", "Enemy")))
			{
				player.GetComponent<CapsuleCollider>().enabled = true;
				return false;
			}

			player.GetComponent<CapsuleCollider>().enabled = true;
			return true;
		}

		public bool CanChase(Player player, TileNode tileNode)
		{
			if (player == null) return false;
			if (tileNode == null) return false;
			if (player.activeState == ActiveState.Dead) return false;
			if (player.copyHp <= 0) return false;
			if (player.currentHp <= 0) return false;

			if (_board.CheckObstacle(tileNode, player.transform.position)) return false;

			return true;
		}


		public bool CanSee(TileNode player, TileNode enemy)
		{
			if (player == null) return false;
			if (enemy == null) return false;


			if (Physics.Linecast(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle")))
			{
				return false;
			}
			return true;
		}

		public bool CanSee(Player player, Enemy enemy, float range)
		{
			if (player == null) return false;
			if (enemy == null) return false;
			if (player.activeState == ActiveState.Dead) return false;
			if (player.copyHp <= 0) return false;
			if (player.currentHp <= 0) return false;


			var distance = Vector3.Distance(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up);
			if (distance > range)
			{
				return false;
			}

			if (Physics.Linecast(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle")))
			{
				return false;
			}
			return true;
		}

		public bool CanSee(Player player, Enemy enemy)
		{
			if (player == null) return false;
			if (enemy == null) return false;
			if (player.activeState == ActiveState.Dead) return false;
			if (player.copyHp <= 0) return false;
			if (player.currentHp <= 0) return false;

			if (Physics.Linecast(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle")))
			{
				return false;
			}
			return true;
		}

		public bool EnemySight(Enemy dead, Enemy enemy, float visionDistance = 0)
		{
			if (dead == null) return false;
			if (enemy == null) return false;

			var distance = Vector3.Distance(dead.transform.position + Vector3.up, enemy.transform.position + Vector3.up);
			if (distance > visionDistance)
			{
				return false;
			}

			if (Physics.Linecast(dead.transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle")))
			{
				return false;
			}

			RaycastHit ray;
			if (Physics.Raycast(dead.transform.position, Vector3.down, out ray, sightCheck, LayerMask.GetMask("Sight")))
			{
				// print(ray.transform.parent.parent.name);
				if (ray.transform.parent.parent.name == enemy.name)
				{
					return true;
				}
			}

			return false;
		}

		public bool EnemySight(Player player, Enemy enemy, float visionDistance = 0)
		{
			if (player == null) return false;
			if (enemy == null) return false;
			if (player.activeState == ActiveState.Dead) return false;
			if (player.copyHp <= 0) return false;
			if (player.currentHp <= 0) return false;

			var distance = Vector3.Distance(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up);
			if (distance > visionDistance)
			{
				return false;
			}

			if (Physics.Linecast(player.transform.position + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle")))
			{
				return false;
			}

			RaycastHit ray;
			if (Physics.Raycast(player.transform.position, Vector3.down, out ray, sightCheck, LayerMask.GetMask("Sight")))
			{
				// print(ray.transform.parent.parent.name);
				if (ray.transform.parent.parent.name == enemy.name)
				{
					return true;
				}
			}

			return false;
		}

		public bool EnemySight(Vector3 node, Enemy enemy, float visionDistance = 0)
		{
			if (enemy == null) return false;

			var distance = Vector3.Distance(node + Vector3.up, enemy.transform.position + Vector3.up);
			if (distance > visionDistance)
			{
				return false;
			}

			if (Physics.Linecast(node + Vector3.up, enemy.transform.position + Vector3.up, out _,
				LayerMask.GetMask("Obstacle")))
			{
				return false;
			}

			RaycastHit ray;
			if (Physics.Raycast(node, Vector3.down, out ray, sightCheck, LayerMask.GetMask("Sight")))
			{
				if (ray.transform.parent.parent.parent.name == enemy.name)
				{
					return true;
				}
			}

			return false;
		}

		public List<TileNode> RushTileList(Player player, Enemy enemy)
		{
			// Rush means after Moving and Attack
			var rushTileList = new List<TileNode>();
			var areaList = new List<TileNode>();

			enemy.moveBaseArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveBaseArea")))
				{
					areaList.Add(node);
				}
			}

			enemy.moveBaseArea.SetActive(false);
			player.moveBaseArea.SetActive(true);

			foreach (var node in areaList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveBaseArea")))
				{
					rushTileList.Add(node);
				}
			}

			player.moveBaseArea.SetActive(false);
			return rushTileList;
		}

		public List<TileNode> ChaseTileList(Player player, Enemy enemy)
		{
			var chaseTileList = new List<TileNode>();
			var areaList = new List<TileNode>();

			enemy.moveDoubleArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveDoubleArea")))
				{
					areaList.Add(node);
				}
			}

			enemy.moveDoubleArea.SetActive(false);
			player.moveBaseArea.SetActive(true);

			foreach (var node in areaList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveBaseArea")))
				{
					chaseTileList.Add(node);
				}
			}

			player.moveBaseArea.SetActive(false);
			return chaseTileList;
		}

		public List<TileNode> ShootTileList(Player player, Enemy enemy)
		{
			//	Shoot means after Moving and Attack
			var shootTileList = new List<TileNode>();
			var areaList = new List<TileNode>();

			enemy.moveBaseArea.SetActive(true);

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("MoveBaseArea")))
				{
					areaList.Add(node);
				}
			}

			enemy.moveBaseArea.SetActive(false);
			player.moveDoubleArea.SetActive(true);

			foreach (var node in areaList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
						LayerMask.GetMask("MoveDoubleArea")))
				{
					shootTileList.Add(node);
				}
			}

			player.moveDoubleArea.SetActive(false);
			return shootTileList;
		}

		public List<Enemy> RangeEnemyList(Player player, AreaCheckOption checkOption, Vector3 position)
		{
			var rangeEnemyList = new List<Enemy>();
			var areaList = new List<TileNode>();
			_board.ResetBoard();

			if (checkOption == AreaCheckOption.Round)
			{
				player.rangeArea.transform.position = position;
				player.rangeArea.SetActive(true);
			}
			else if (checkOption == AreaCheckOption.Player)
			{
				player.rangeArea.transform.position = player.transform.position;
				player.rangeArea.SetActive(true);
			}

			foreach (var node in _board.NodeList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity,
					LayerMask.GetMask("RangeArea")))
				{
					areaList.Add(node);
				}
			}

			if (checkOption == AreaCheckOption.Round)
			{
				player.rangeArea.transform.position = player.transform.position;
				player.rangeArea.SetActive(false);
			}
			else if (checkOption == AreaCheckOption.Player)
			{
				player.rangeArea.transform.position = player.transform.position;
				player.rangeArea.SetActive(false);
			}

			foreach (var node in areaList)
			{
				if (Physics.CheckBox(node.transform.position, GameUtility.Box, Quaternion.identity, LayerMask.GetMask("Enemy")))
				{
					rangeEnemyList.Add(_gameManager.activeEnemyList.Find(enemy => GameUtility.Coordinate(enemy.transform.position) == GameUtility.Coordinate(node.transform.position)));
				}
			}

			return rangeEnemyList;
		}
	}
}
