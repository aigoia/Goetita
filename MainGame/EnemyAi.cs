using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AreaOrder
{
    Base, Double, 
}

public enum WalkOrder
{
    Random, Base, Rush,
}

namespace Game.MainGame
{
    public class EnemyAi : MonoBehaviour
    {
        Board _board;
        GameManager _gameManager;
        private AreaCheck _areaCheck;
        public Enemy thisEnemy;

        private readonly int _nCount = -1; 
        readonly int emptyCount = 0;
        public readonly int emptyVigor = 0;
        public readonly int halfVigor = 1;
        public readonly int fullVigor = 2;

        void Awake()
        {
            if (_board == null) FindObjectOfType<Board>();
            if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();   
            _areaCheck = _gameManager.areaCheck;
        }

        public void StartAi()
        {
            _gameManager.activeEnemyList.ForEach(i => i.buyHit = BuyHit.Normal);
            _gameManager.activePlayerList.ForEach(i => i.beHit = BeHit.Normal);
            
            print(this +" is ready");
            _gameManager.Marked();

            StartCoroutine(StartEnemies());
            // StartCoroutine(ForcedEnding());
        }

        IEnumerator ForcedEnding()
        {
            var count = _gameManager.activeEnemyList.Count;
            var wait = new WaitForSeconds(15f * count);
            
            yield return wait;
            _gameManager.EnemyTurnEnding();
        }

        List<Enemy> MakeEndList()
        {
            var endList = new List<Enemy>();
            
            foreach (var enemy in _gameManager.activeEnemyList)
            {
                if (enemy == null) continue;
                if (enemy.turnState == TurnState.Ending) endList.Add(enemy);    
            }

            return endList;
        }

        void RandomBase(Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            if (enemy.currentVigor >= halfVigor)
            {
                var otherWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Random, null);
                if (otherWalk) enemy.currentVigor = emptyVigor;
            }
            
            _gameManager.Marked();
        }

        bool CheckMarked()
        {
            foreach (var player in _gameManager.activePlayerList)
            {
                if (player == null) continue;

                if (player.marked >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        void GoNear(Enemy enemy)
        {
            if (!CheckMarked()) return;
            if (enemy.currentVigor == emptyVigor) return;
            
             var endingEnemyList = MakeEndList();
                                    
             foreach (var endingEnemy in endingEnemyList)
             {
                 if (endingEnemy == null) continue;
                 
                 var baseNodeList = _areaCheck.BaseNode(endingEnemy);
                 if (baseNodeList == null) continue;
                 GameUtility.ShuffleList(baseNodeList);
                                        
                 foreach (var node in baseNodeList)
                 {
                     if (node == null) continue;
                     
                     if (_areaCheck.CanMove(enemy, node))
                     {
                         var otherWalk = enemy.UnitWalk(AreaOrder.Double, WalkOrder.Base, node);
                         if (otherWalk)
                         {
                             print("Go Near");
                             enemy.currentVigor = emptyVigor;
                         }
                         return;
                     } 
                 }
             }
             
             _gameManager.Marked();
        }

        void ChaseCloseStrike(List<Player> moveList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            Player targetPlayer = null;
            TileNode targetTileNode = null;

            // Check Player and TileNode
            foreach (var player in moveList)
            {
                if (player == null) continue;
                
                var tileList = _gameManager.areaCheck.ChaseTileList(player, enemy);
                if (tileList == null) continue;
                GameUtility.ShuffleList(tileList);
                            
                foreach (var node in tileList)
                {
                    if (node == null) continue;
                    
                    if (_areaCheck.CanMove(enemy, node))
                    { 
                        if (_areaCheck.CanChase(player, node))
                        {
                            targetPlayer = player; 
                            targetTileNode = node;
                            break;
                        }
                    }
                }
            }
                        
            if (targetPlayer != null && targetTileNode != null)
            {
                print("ChaseClose");
                var way = enemy.UnitWalk(AreaOrder.Double, WalkOrder.Base, targetTileNode);
                if (way) enemy.currentVigor = emptyVigor;
                // thisEnemy.Damage(targetPlayer);
            }
            
            _gameManager.Marked();
        }

        void DoubleRangeStrike(List<Player> doubleList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            foreach (var player in doubleList)
            {
                if (player == null) continue;
                
                if (_areaCheck.CanRange(player, enemy))
                {
                    StartCoroutine(AttackRange(player, enemy));
                    
                    if (enemy.currentVigor != emptyVigor) player.currentHp = enemy.Damage(player, player.currentHp);
                    enemy.currentVigor = emptyVigor;
                    break;
                }
            }
            
            _gameManager.Marked();
        }

        void MoveRangeStrike(List<Player> moveList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            Player targetPlayer = null;
            TileNode targetTileNode = null;

            // Check Player and TileNode
            foreach (var player in moveList)
            {
                if (player == null) continue;

                var tileList = _gameManager.areaCheck.ShootTileList(player, enemy);
                if (tileList == null) continue;
                GameUtility.ShuffleList(tileList);
                            
                foreach (var node in tileList)
                {
                    if (node == null) continue;
                    
                    if (_areaCheck.CanMove(enemy, node))
                    { 
                        if (_areaCheck.CanRange(player, node))
                        {
                            targetPlayer = player; 
                            targetTileNode = node;
                            break;
                        }
                    }
                }
            }
                        
            if (targetPlayer != null && targetTileNode != null)
            {
                print("ShootRange");
                
                StartCoroutine(ShootRange(targetPlayer, targetTileNode, enemy));
                
                if (enemy.currentVigor != emptyVigor) targetPlayer.currentHp = enemy.Damage(targetPlayer, targetPlayer.currentHp);
                enemy.currentVigor = emptyVigor;
            }
            
            _gameManager.Marked();
        }

        void BaseCloseStrike(List<Player> baseList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            foreach (var player in baseList)
            {
                if (player == null) continue;
                
                if (_areaCheck.CanSee(player, enemy))
                {
                    var playNearNode = _areaCheck.NearNode(player);
                    if (playNearNode == null) continue;
                    var shortNode = ShortNode(GameUtility.Coordinate(enemy.transform.position), playNearNode);
                    if (shortNode == null) continue;
                    
                    if (shortNode != null) StartCoroutine(AttackClose(player, enemy, shortNode));
                    
                    if (enemy.currentVigor != emptyVigor) player.currentHp = enemy.Damage(player, player.currentHp);
                    enemy.currentVigor = emptyVigor;
                    break;
                }
            }
            
            enemy.currentVigor = emptyVigor;
            _gameManager.Marked();
        }

        void DoubleCloseStrike(List<Player> doubleList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            if (_gameManager.doubleClose)
            {
                GameUtility.ShuffleList(doubleList);
                
                foreach (var player in doubleList)
                {
                    if (player == null) continue;
                    
                    if (_areaCheck.CanSee(player, enemy))
                    {
                        var playNearNode = _areaCheck.NearNode(player);
                        if (playNearNode == null) continue;
                        var shortNode = ShortNode(GameUtility.Coordinate(enemy.transform.position), playNearNode);
                        if (shortNode == null) continue;
                        
                        if (shortNode != null) StartCoroutine(AttackClose(player, enemy, shortNode));
                        
                        if (enemy.currentVigor != emptyVigor) player.currentHp = enemy.Damage(player, player.currentHp);
                        enemy.currentVigor = emptyVigor;
                        break;
                    }
                }
            }
            
            enemy.currentVigor = emptyVigor;
            _gameManager.Marked();
        }

        void DoubleCloseObstacle(List<Player> doubleList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            if (enemy.currentVigor >= fullVigor)
            {
                print("No one can see!");
                
                Player targetPlayer = null;
                TileNode middleNode = null;
                TileNode endTileNode = null;

                GameUtility.ShuffleList(doubleList);

                // Check Player and TileNode
                foreach (var player in doubleList)
                {
                    if (player == null) continue;
                    if (player.marked <= emptyCount) continue;
                    
                    var rushTileList = _gameManager.areaCheck.RushTileList(player, enemy);
                    GameUtility.ShuffleList(rushTileList);
                
                    foreach (var node in rushTileList)
                    {
                        if (node == null) continue;
                        
                        if (_areaCheck.CanMove(enemy, node))
                        {
                            if (_areaCheck.CanMove(player, node))
                            {
                                var playNearNode = _areaCheck.NearNode(player);
                                if (playNearNode == null) continue;
                                
                                foreach (var nearNode in playNearNode.ToList())
                                {
                                    if (nearNode == null) continue;
                                    
                                    GameUtility.ShuffleList(playNearNode);
                                    
                                    if (_areaCheck.CanMove(node, nearNode))
                                    {
                                        targetPlayer = player; 
                                        middleNode = node;
                                        endTileNode = nearNode;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            
                if (targetPlayer != null && middleNode != null && endTileNode != null)
                {
                    StartCoroutine(RushClose(targetPlayer, enemy, middleNode, endTileNode));
                    
                    if (enemy.currentVigor != emptyVigor) targetPlayer.currentHp = enemy.Damage(targetPlayer, targetPlayer.currentHp);
                    enemy.currentVigor = emptyVigor;
                }
                else
                {
                    print("They are over the wall!");
                    RandomBase(enemy);
                }
            }
            
            _gameManager.Marked();
        }

        void BaseCloseObstacle(List<Player> baseList, Enemy enemy)
        {
            if (enemy.currentVigor == emptyVigor) return;
            
            if (enemy.currentVigor >= fullVigor)
            {
                print("No one can see!");
                
                Player targetPlayer = null;
                TileNode middleNode = null;
                TileNode endTileNode = null;

                GameUtility.ShuffleList(baseList);

                // Check Player and TileNode
                foreach (var player in baseList)
                {
                    if (player == null) continue;
                    
                    var rushTileList = _gameManager.areaCheck.RushTileList(player, enemy);
                    GameUtility.ShuffleList(rushTileList);
                
                    foreach (var node in rushTileList)
                    {
                        if (node == null) continue;
                        
                        if (_areaCheck.CanMove(enemy, node))
                        {
                            if (_areaCheck.CanMove(player, node))
                            {
                                var playNearNode = _areaCheck.NearNode(player);
                                if (playNearNode == null) continue;
                                
                                foreach (var nearNode in playNearNode.ToList())
                                {
                                    if (nearNode == null) continue;
                                    
                                    GameUtility.ShuffleList(playNearNode);
                                    
                                    if (_areaCheck.CanMove(node, nearNode))
                                    {
                                        targetPlayer = player; 
                                        middleNode = node;
                                        endTileNode = nearNode;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            
                if (targetPlayer != null && middleNode != null && endTileNode != null)
                {
                    StartCoroutine(RushClose(targetPlayer, enemy, middleNode, endTileNode));
                    
                    if (enemy.currentVigor != emptyVigor) targetPlayer.currentHp = enemy.Damage(targetPlayer, targetPlayer.currentHp);
                    enemy.currentVigor = emptyVigor;
                }
                else
                {
                    print("They are over the wall!");
                    RandomBase(enemy);
                }
            }
            
            _gameManager.Marked();
        }
        
        IEnumerator StartEnemies()
        {
            thisEnemy = null;
            GameUtility.ShuffleList(_gameManager.activeEnemyList);
            var gameOver = false;

            foreach (var player in _gameManager.activePlayerList)
            {
                if (player == null) continue;
                
                player.copyHp = player.currentHp;
            }
            

            foreach (var currentEnemy in _gameManager.activeEnemyList.ToList())
            {
                if (currentEnemy == null) continue;
                
                while (currentEnemy.getHit == GetHit.GetHit)
                {
                    yield return null;
                }
                
                currentEnemy.RenewalPlayerList();
                
                this.thisEnemy = currentEnemy;
                print("Current enemy is " + this.thisEnemy.name);

                var doSomething = this.thisEnemy.baseVigor;
                var something = doSomething;
                var oneMore = new WaitUntil(() => this.thisEnemy.currentVigor < something);
                
                if (this.thisEnemy.characterType == CharacterType.Claymore)
                {
                    var baseList = _areaCheck.BaseList(this.thisEnemy);
                    var doubleList = _areaCheck.DoubleList(this.thisEnemy, baseList);

                    print(this.thisEnemy + " base count : " + baseList.Count + ", double count : " + doubleList.Count);

                    // hit 
                    if (baseList.Count != emptyCount && this.thisEnemy.currentVigor >= halfVigor)
                    {
                        GameUtility.ShuffleList(baseList);
                        BaseCloseStrike(baseList, this.thisEnemy);
                        
                        if (this.thisEnemy.currentVigor >= fullVigor)
                        {
                            BaseCloseObstacle(baseList, this.thisEnemy);
                        }
                    }
                    else if (doubleList.Count != emptyCount && this.thisEnemy.currentVigor >= fullVigor)
                    {
                        DoubleCloseStrike(doubleList, this.thisEnemy);
                        
                        if (this.thisEnemy.currentVigor >= fullVigor)
                        {
                            DoubleCloseObstacle(doubleList, this.thisEnemy);
                        }
                    }
                    
                    // chase
                    if (this.thisEnemy.currentVigor >= fullVigor)
                    {
                        var chaseList = _areaCheck.ChasePlayerList(_gameManager.activePlayerList, this.thisEnemy);
                        print(this.thisEnemy + " chase count : " + chaseList.Count);
                        
                        GameUtility.ShuffleList(chaseList);
                        ChaseCloseStrike(chaseList, this.thisEnemy);
                    }

                    // protect friends
                    if (this.thisEnemy.currentVigor >= fullVigor) 
                    {
                        GoNear(this.thisEnemy);
                    }
                    
                    RandomBase(this.thisEnemy);
                    this.thisEnemy.currentVigor = emptyVigor;
                }
                
                else if (this.thisEnemy.characterType == CharacterType.Ranger)
                {
                    var doubleList = _areaCheck.DoubleList(this.thisEnemy);
                    var moveList = _areaCheck.ShootPlayerList(_gameManager.activePlayerList, this.thisEnemy);
                    
                    print(this.thisEnemy + " range count : " + doubleList.Count + ", move and shoot count : " + moveList.Count);

                    if (doubleList.Count != emptyCount && this.thisEnemy.currentVigor >= halfVigor)
                    {
                        GameUtility.ShuffleList(doubleList);
                        DoubleRangeStrike(doubleList, this.thisEnemy);
                    }
                    if (moveList.Count != emptyCount && this.thisEnemy. currentVigor >= fullVigor)
                    {
                        GameUtility.ShuffleList(moveList);
                        MoveRangeStrike(moveList, this.thisEnemy);
                    }
                    else if (this.thisEnemy.currentVigor >= halfVigor) 
                    {
                        GoNear(this.thisEnemy);
                    }
                    
                    RandomBase(this.thisEnemy);
                    
                    this.thisEnemy.currentVigor = emptyVigor;
                }

                // if you make skill set, it'll need to change
                var condition = new WaitUntil(() => this.thisEnemy.currentVigor <= emptyVigor);
                yield return condition;

                if (this.thisEnemy != null)
                {
                    // there was a null bug
                    if (this.thisEnemy != null)
                    {
                        while (thisEnemy != null) 
                        {
                            if (this.thisEnemy.activeState == ActiveState.Moving)
                            {
                                yield return null;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    
                    if (this.thisEnemy != null)
                    {
                        this.thisEnemy.turnState = TurnState.Ending;  
                    }
                }
                if (this.thisEnemy != null)
                {
                    while (this.thisEnemy.buyHit == BuyHit.BuyHit)
                    { 
                        if (this.thisEnemy == null) break; 
                        yield return null;
                    }
                }
                if (this.thisEnemy != null)
                {
                    this.thisEnemy.turnState = TurnState.Ending;    
                }
                
                currentEnemy.RenewalPlayerList();
                gameOver = _gameManager.CheckGameOver();
                if (gameOver) break;
            }

            foreach (var player in _gameManager.playerList.ToList())
            {
                while (player.beHit == BeHit.BeHit)
                { 
                    yield return null;
                }
            }

            foreach (var enemy in _gameManager.activeEnemyList)
            {
                while (enemy.battle)
                {
                    yield return null;
                }
            }
            
            gameOver = _gameManager.CheckGameOver();
            if (!gameOver) _gameManager.EnemyTurnEnding();
        }

        IEnumerator RushClose(Player player, Enemy enemy, TileNode middleNode, TileNode endNode)
        {
            //	Rush means after Moving and Attack
            var firstWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, endNode);

            if (firstWalk)
            {
                while (enemy.activeState == ActiveState.Moving)
                {
                    yield return null;
                }
                
                print("CloseDirect");
                enemy.CloseAttackByEnemy(player);
                
                while (enemy.buyHit == BuyHit.BuyHit)
                { 
                    yield return null;
                }
            }
            else
            {
                var thisWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Rush, middleNode);

                while (enemy.activeState == ActiveState.Moving)
                {
                    yield return null;
                }
                
                enemy.transform.LookAt(player.transform);
    
                var canWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, endNode);
                
                while (enemy.activeState == ActiveState.Moving)
                {
                    yield return null;
                }

                if (canWalk)
                {
                    print("RushClose");
                    enemy.CloseAttackByEnemy(player);
                }
                else
                {
                    var otherWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Random, null);
                }

                while (enemy.buyHit == BuyHit.BuyHit)
                { 
                    yield return null;
                }
            }
            
            enemy.currentVigor = emptyVigor;
            _gameManager.Marked();
        }

        IEnumerator ShootRange(Player player, TileNode selectedNode, Enemy enemy)
        {
            //	Shoot means after Moving and Attack
            var canWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, selectedNode);

            while (enemy.activeState == ActiveState.Moving)
            {
                yield return null;
            }

            if (canWalk)
            {
                print("ShootRange");
                enemy.RangeAttackByEnemy(player);
            }

            while (enemy.buyHit == BuyHit.BuyHit)
            { 
                yield return null;
            }
            
            enemy.currentVigor = emptyVigor;
            _gameManager.Marked();
        }
        
        IEnumerator AttackRange(Player player, Enemy enemy)
        {
            print("AttackRange");
            enemy.RangeAttackByEnemy(player);

            while (enemy.buyHit == BuyHit.BuyHit)
            { 
                yield return null;
            }
            
            enemy.currentVigor = emptyVigor;
            _gameManager.Marked();
        }

        IEnumerator AttackClose(Player player, Enemy enemy, TileNode shortNode)
        {
            var canWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, shortNode);
            
            while (enemy.activeState == ActiveState.Moving)
            { 
                yield return null;
            }

            if (canWalk)
            {
                print("AttackClose");
                enemy.CloseAttackByEnemy(player);
            }
            else
            {
                var otherWalk = enemy.UnitWalk(AreaOrder.Base, WalkOrder.Random, null);
            }

            while (enemy.buyHit == BuyHit.BuyHit)
            {
                yield return null;
            }
            
            enemy.currentVigor = emptyVigor;
            _gameManager.Marked();
        }
        
        private TileNode ShortNode (Vector2 startPos ,List<TileNode> nodeList)
        {
            if (nodeList.Count == emptyCount) return null;  
            
            foreach (var node in nodeList)
            {
                node.distance = 0f;
                node.distance = Vector2.Distance(startPos, node.Coordinate);
            }

            var minDistance = nodeList.Min(n => n.distance);
            var newNode = nodeList.Find(n => n.distance == minDistance);  
            
            return newNode;
        }
    }
}