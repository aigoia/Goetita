using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.MainGame
{
    public class Board : MonoBehaviour {

        public TileNode startNode;
        public List<TileNode> currentWay;
        List<TileNode> _currentBaseBoundary;
        List<TileNode> _currentDoubleBoundary;
        List<TileNode> _currentObstacleBoundary;

        [SerializeField] private Transform boardBoundary;
        [SerializeField] Transform boundary;
        [SerializeField] Transform tileNode;
        [SerializeField] GameObject boundaryObject;
        [SerializeField] GameObject boundaryBlock;
        [SerializeField]GameObject tileObject;
        public readonly Vector3 Box = new Vector3(1f, 1f, 1f);

        List<TileNode> _nodeList;
        public List<TileNode> NodeList => _nodeList;

        public LineRenderer line;
        public Vector3 lineVector = new Vector3(0, 0.03f, 0);

        [SerializeField] private static int xSize = 32;
        [SerializeField] private static int ySize = 32;
        public Vector2 boardSize = new Vector2(xSize, ySize);

        PathFinding _pathFinding;
        public PathFinding PathFinding => _pathFinding;
        public Transform mapGround;

        GameManager _gameManager;
        EnemyAi _enemyAi;
        Shield _shield;

        void Awake()
        {   
            if (line == null) line = transform.Find("Line").GetComponent<LineRenderer>();
            if (_pathFinding == null) _pathFinding = GetComponent<PathFinding>();
            if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
            if (_enemyAi == null) _enemyAi = _gameManager.GetComponent<EnemyAi>();
            if (boundary == null) boundary = transform.Find("Boundary");
            if (boardBoundary == null) boundary = transform.Find("BoardBoundary");
            if (tileNode == null) tileNode = transform.Find("TileNode");

            MakeGround();
            MakeMap();
            
            if (_nodeList == null) _nodeList = new List<TileNode>(FindObjectsOfType<TileNode>());
        }

        private void MakeGround()
        {
            
        }

        [ContextMenu("Make Map")]
        void MakeMap()
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    MakeNode(x, y);
                }
            }
            
            var newList = new List<TileNode>(FindObjectsOfType<TileNode>());

            foreach (var node in newList)
            {
                foreach (var direction in GameUtility.Directions)
                {
                    RaycastHit hit;
                    Physics.Raycast(node.transform.position, direction, out hit, GameUtility.interval);

                    if (hit.transform == null)
                    {
                        if (direction == Vector3.forward)
                        {
                            Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.identity, boardBoundary);
                        }
                        else if (direction == Vector3.left)
                        {
                            Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.Euler(0f, 90f, 0f), boardBoundary);
                        }
                        else if (direction == Vector3.back)
                        {
                            Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.identity, boardBoundary);
                        }
                        else if (direction == Vector3.right)
                        {
                            Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.Euler(0f, 90f, 0f), boardBoundary);
                        }
                    }
                }
            }
        }

        void MakeNode(int x, int y)
        {
            var worldPos = new Vector3(x * GameUtility.interval, 0, y * GameUtility.interval);
            var node = Instantiate(tileObject, worldPos , tileObject.transform.rotation, tileNode);
            node.name = "TileNode(" + x + ", " +y + ")";
            var getNode = node.GetComponent<TileNode>();

            if (getNode == null)
            {
                node.AddComponent<TileNode>();
                getNode = node.GetComponent<TileNode>();
            }

            getNode.Coordinate = new Vector2(x, y);
            SetTileStyle(getNode);
        }

        public void ResetBoundary()
        {
            GameUtility.ResetObjects(boundary, _gameManager.holder);
        }

        void FindPlayerNode(Player player)
        {
            if (_gameManager.currentPlayer == null) return;

            startNode = NodeList.Find(i => i.Coordinate == GameUtility.Coordinate(player.transform.position));
        }

        void FindEnemyNode(Enemy enemy)
        {
            if (_gameManager.currentPlayer == null) return;

            startNode = NodeList.Find(i => i.Coordinate == GameUtility.Coordinate(enemy.transform.position));
        }

        public void ResetEnemyAiBoard(Enemy enemy)
        {
            if (enemy == null) return;

            foreach (var node in _nodeList)
            {
                node.tileStyle = TileStyle.Normal;
            }
        }

        public void MiniReset(Enemy enemy)
        {
            if (enemy == null) return;

            foreach (var node in NodeList)
            {
                node.tileStyle = TileStyle.Normal;
            }
        }
        
        public void ResetEnemyBoard(Enemy enemy)
        {
            if (enemy == null) return;

            foreach (var node in NodeList)
            {
                node.tileStyle = TileStyle.Normal;
            }

            FindEnemyNode(enemy);
            MoveAreaOnEnemy(enemy);
            
            foreach (var node in NodeList)
            {
                node.gameObject.layer = LayerMask.NameToLayer("Node");
                node.tileStyle = TileStyle.Normal;
                node.shieldStyle = ShieldStyle.Default;
                SetEnemyTileStyle(node, enemy.transform.position);
            }

            MoveAreaOffEnemy(enemy);
        }

        public void ResetBoard()
        {
            FindPlayerNode(_gameManager.currentPlayer);
            MoveAreaOnPlayer(_gameManager.currentPlayer);
            ResetBoundary();
            _gameManager.ResetClose();


            if (_gameManager.turnState == Turn.PlayerTurn)
            {
                _currentObstacleBoundary = new List<TileNode>();
                _currentBaseBoundary = new List<TileNode>();
                _currentDoubleBoundary = new List<TileNode>();
            }
            
            foreach (var node in NodeList)
            {
                node.gameObject.layer = LayerMask.NameToLayer("Node");
                node.tileStyle = TileStyle.Normal;
                node.shieldStyle = ShieldStyle.Default;
                
                if (_gameManager.currentPlayer != null ) SetTileStyle(node, _gameManager.currentPlayer.transform.position);
            }

            if (_gameManager.turnState == Turn.PlayerTurn)
            {
                foreach (var node in _currentObstacleBoundary)
                {
                    MakeBoundary(node, "BaseNode");
                    MakeBoundary(node, "DoubleNode");
                }
                foreach (var node in _currentBaseBoundary)
                {
                    MakeBoundary(node, "BaseNode");
                }
                foreach (var node in _currentDoubleBoundary)
                {
                    MakeBoundary(node, "DoubleNode");
                }
            }
            
            MoveAreaOffPlayer(_gameManager.currentPlayer);
        }

        void MakeBoundary(TileNode node, string nodeLayer)
        {
            if (_gameManager.somethingOn) return;

            if (boundary.GetChild(0) == null)
            {
                Instantiate(_gameManager.holder, Vector3.zero, Quaternion.identity, boundary);
            }

            foreach (var direction in GameUtility.Directions)
            {
                if (Physics.Raycast(node.transform.position, direction, GameUtility.interval, LayerMask.GetMask(nodeLayer)))
                {
                    if (direction == Vector3.forward)
                    {
                        Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.identity, boundary.GetChild(0));
                    }
                    else if (direction == Vector3.left)
                    {
                        Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.Euler(0f, 90f, 0f), boundary.GetChild(0));
                    }
                    else if (direction == Vector3.back)
                    {
                        Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.identity, boundary.GetChild(0));
                    }
                    else if (direction == Vector3.right)
                    {
                        Instantiate(boundaryBlock, node.transform.position + direction, Quaternion.Euler(0f, 90f, 0f), boundary.GetChild(0));
                    }
                }
            }
        }

        void MoveAreaOnPlayer(Player player)
        {
            if (player == null) return;

            if (player.currentVigor == 2)
            {
                player.moveDoubleBlock.SetActive(true);
                player.moveDoubleArea.SetActive(true);
                player.moveBaseBlock.SetActive(true);
                player.moveBaseArea.SetActive(true);
            }
            else if (_gameManager.currentPlayer.currentVigor == 1)
            {
                player.moveDoubleBlock.SetActive(false);
                player.moveDoubleArea.SetActive(false);
                player.moveBaseBlock.SetActive(true);
                player.moveBaseArea.SetActive(true);
            }
            else if (_gameManager.currentPlayer.currentVigor <= 0)
            {
                player.moveDoubleBlock.SetActive(false);
                player.moveDoubleArea.SetActive(false);
                player.moveBaseBlock.SetActive(false);
                player.moveBaseArea.SetActive(false);
            }
        }

        void MoveAreaOffPlayer(Player player)
        {
            if (_gameManager.currentPlayer == null) return;

            player.moveDoubleBlock.SetActive(false);
            player.moveDoubleArea.SetActive(false);
            player.moveBaseBlock.SetActive(false);
            player.moveBaseArea.SetActive(false);
        }

        void MoveAreaOnEnemy(Enemy enemy)
        {
            if (enemy == null) return;

            if (enemy.currentVigor == 2)
            {
                enemy.moveDoubleArea.SetActive(true);
                enemy.moveBaseArea.SetActive(true);
            }
            else if (enemy.currentVigor == 1)
            {
                enemy.moveDoubleArea.SetActive(false);
                enemy.moveBaseArea.SetActive(true);
            }
            else if (enemy.currentVigor <= 0)
            {
                enemy.moveDoubleArea.SetActive(false);
                enemy.moveBaseArea.SetActive(false);
            }
        }

        void MoveAreaOffEnemy(Enemy enemy)
        {
            if (enemy == null) return;

            enemy.moveDoubleArea.SetActive(false);
            enemy.moveBaseArea.SetActive(false);
        }

        public bool CheckObstacle(TileNode node, Vector3 pos)
        {
            if (pos == new Vector3()) return false;
            
            RaycastHit hit;
            if (Physics.Linecast(node.transform.position, pos, out hit,
                LayerMask.GetMask("Obstacle")))
            {
                return true;
            }
            
            return false;
        }
        
        void SetEnemyTileStyle(TileNode node, Vector3 pos = new Vector3())
        {
            if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("Obstacle", "Player", "Enemy")))
            {
                node.tileStyle = TileStyle.NonWalkable;

                if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseBlock")))
                {
                    _currentObstacleBoundary.Add(node);
                }
                else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleBlock")))
                { 
                    _currentObstacleBoundary.Add(node);
                }
            }
            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseArea")))
            {
                node.tileStyle = TileStyle.OneArea;
                node.gameObject.layer = LayerMask.NameToLayer("BaseNode");
            }
            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseBlock")))
            {
                node.areaBoundary = BoundaryArea.OneBlock;
                _currentBaseBoundary.Add(node);

                 if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleArea")))
                {
                    node.tileStyle = TileStyle.TwoArea;
                    node.gameObject.layer = LayerMask.NameToLayer("DoubleNode");
                }
            }
            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleArea")))
            {
                node.tileStyle = TileStyle.TwoArea;
                node.gameObject.layer = LayerMask.NameToLayer("DoubleNode");
            }

            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleBlock")))
            {
                node.areaBoundary = BoundaryArea.TowBlock;
                _currentDoubleBoundary.Add(node);

                 node.tileStyle = TileStyle.Normal;
            }

            else
            {
                node.tileStyle = TileStyle.Normal;
            }
        
        }

        void SetTileStyle(TileNode node, Vector3 pos = new Vector3())
        {
            if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("Obstacle", "Player", "Enemy")))
            {
                node.tileStyle = TileStyle.NonWalkable;

                if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseBlock")))
                {
                    _currentObstacleBoundary.Add(node);
                }
                else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleBlock")))
                {
                    _currentObstacleBoundary.Add(node);
                }
            }
            else if (CheckObstacle(node, pos))
            {
                if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseBlock")))
                {
                    node.tileStyle = TileStyle.HideOneArea;
                    _currentObstacleBoundary.Add(node);
                }
                else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleBlock")))
                {
                    node.tileStyle = TileStyle.HideOneArea;
                    _currentObstacleBoundary.Add(node);
                }
            }
            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseArea")))
            {
                node.tileStyle = TileStyle.OneArea;
                node.gameObject.layer = LayerMask.NameToLayer("BaseNode");
            }
            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveBaseBlock")))
            {
                node.areaBoundary = BoundaryArea.OneBlock;
                _currentBaseBoundary.Add(node);

                if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleArea")))
                {
                    node.tileStyle = TileStyle.TwoArea;
                    node.gameObject.layer = LayerMask.NameToLayer("DoubleNode");
                }
            }
            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleArea")))
            {
                node.tileStyle = TileStyle.TwoArea;
                node.gameObject.layer = LayerMask.NameToLayer("DoubleNode");
            }

            else if (Physics.CheckBox(node.transform.position, Box, Quaternion.identity, LayerMask.GetMask("MoveDoubleBlock")))
            {
                node.areaBoundary = BoundaryArea.TowBlock;
                _currentDoubleBoundary.Add(node);

                node.tileStyle = TileStyle.Normal;
            }

            else
            {
                node.tileStyle = TileStyle.Normal;
            }
        }
    }
}
