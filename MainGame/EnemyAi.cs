using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public Enemy currentEnemy;

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
        }

        List<Enemy> MakeEndList()
        {
            var endList = new List<Enemy>();
            
            foreach (var thisEnemy in _gameManager.activeEnemyList)
            {
                if (thisEnemy.turnState == TurnState.Ending) endList.Add(thisEnemy);    
            }

            return endList;
        }

        void RandomBase(Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            if (thisEnemy.currentVigor >= halfVigor)
            {
                var otherWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Random, null);
                if (otherWalk) thisEnemy.currentVigor = emptyVigor;
            }
        }

        bool CheckMarked()
        {
            foreach (var player in _gameManager.activePlayerList)
            {
                if (player.marked >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        void GoNear(Enemy thisEnemy)
        {
            if (!CheckMarked()) return;
            if (thisEnemy.currentVigor == emptyVigor) return;
            
             var endingEnemyList = MakeEndList();
                                    
             foreach (var endingEnemy in endingEnemyList)
             {
                 var baseNodeList = _areaCheck.BaseNode(endingEnemy);
                 GameUtility.ShuffleList(baseNodeList);
                                        
                 foreach (var node in baseNodeList)
                 {
                     if (_areaCheck.CanMove(thisEnemy, node))
                     {
                         var otherWalk = thisEnemy.UnitWalk(AreaOrder.Double, WalkOrder.Base, node);
                         if (otherWalk)
                         {
                             print("Go Near");
                             thisEnemy.currentVigor = emptyVigor;
                         }
                         return;
                     } 
                 }
             }
        }

        void ChaseCloseStrike(List<Player> moveList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            Player targetPlayer = null;
            TileNode targetTileNode = null;

            // Check Player and TileNode
            foreach (var player in moveList)
            {
                var tileList = _gameManager.areaCheck.ChaseTileList(player, thisEnemy);
                GameUtility.ShuffleList(tileList);
                            
                foreach (var node in tileList)
                {
                    if (_areaCheck.CanMove(thisEnemy, node))
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
                var way = thisEnemy.UnitWalk(AreaOrder.Double, WalkOrder.Base, targetTileNode);
                if (way) thisEnemy.currentVigor = emptyVigor;
            }
        }

        void DoubleRangeStrike(List<Player> doubleList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            foreach (var player in doubleList)
            {
                if (_areaCheck.CanRange(player, thisEnemy))
                {
                    thisEnemy.currentVigor = emptyVigor;
                    StartCoroutine(AttackRange(player, thisEnemy));
                    break;
                }
            }
        }

        void MoveRangeStrike(List<Player> moveList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            Player targetPlayer = null;
            TileNode targetTileNode = null;

            // Check Player and TileNode
            foreach (var player in moveList)
            {
                var tileList = _gameManager.areaCheck.ShootTileList(player, thisEnemy);
                GameUtility.ShuffleList(tileList);
                            
                foreach (var node in tileList)
                {
                    if (_areaCheck.CanMove(thisEnemy, node))
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
                thisEnemy.currentVigor = emptyVigor;
                StartCoroutine(ShootRange(targetPlayer, targetTileNode, thisEnemy));
            }
        }

        void BaseCloseStrike(List<Player> baseList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            foreach (var player in baseList)
            {
                if (_areaCheck.CanSee(player, thisEnemy))
                {
                    var playNearNode = _areaCheck.NearNode(player);
                    var shortNode = ShortNode(GameUtility.Coordinate(thisEnemy.transform.position), playNearNode);
                                
                    if (shortNode != null) StartCoroutine(AttackClose(player, thisEnemy, shortNode));
                    thisEnemy.currentVigor = emptyVigor;
                    break;
                }
            }
        }

        void DoubleCloseStrike(List<Player> doubleList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            if (_gameManager.doubleClose)
            {
                GameUtility.ShuffleList(doubleList);
                
                foreach (var player in doubleList)
                {
                    if (_areaCheck.CanSee(player, thisEnemy))
                    {
                        var playNearNode = _areaCheck.NearNode(player);
                        var shortNode = ShortNode(GameUtility.Coordinate(thisEnemy.transform.position), playNearNode);
                                    
                        if (shortNode != null) StartCoroutine(AttackClose(player, thisEnemy, shortNode));
                        thisEnemy.currentVigor = emptyVigor;
                        break;
                    }
                }
            }
        }

        void DoubleCloseObstacle(List<Player> doubleList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            if (thisEnemy.currentVigor >= fullVigor)
            {
                print("No one can see!");
                
                Player targetPlayer = null;
                TileNode middleNode = null;
                TileNode endTileNode = null;

                GameUtility.ShuffleList(doubleList);

                // Check Player and TileNode
                foreach (var player in doubleList)
                {
                    if (player.marked <= emptyCount) continue;
                    
                    var rushTileList = _gameManager.areaCheck.RushTileList(player, thisEnemy);
                    GameUtility.ShuffleList(rushTileList);
                
                    foreach (var node in rushTileList)
                    {
                        if (_areaCheck.CanMove(thisEnemy, node))
                        {
                            if (_areaCheck.CanMove(player, node))
                            {
                                var playNearNode = _areaCheck.NearNode(player);
                                
                                foreach (var nearNode in playNearNode)
                                {
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
                    StartCoroutine(RushClose(targetPlayer, thisEnemy, middleNode, endTileNode));
                    thisEnemy.currentVigor = emptyVigor;
                }
                else
                {
                    print("They are over the wall!");
                }
            }
        }

        void BaseCloseObstacle(List<Player> baseList, Enemy thisEnemy)
        {
            if (thisEnemy.currentVigor == emptyVigor) return;
            
            if (thisEnemy.currentVigor >= fullVigor)
            {
                print("No one can see!");
                
                Player targetPlayer = null;
                TileNode middleNode = null;
                TileNode endTileNode = null;

                GameUtility.ShuffleList(baseList);

                // Check Player and TileNode
                foreach (var player in baseList)
                {
                    var rushTileList = _gameManager.areaCheck.RushTileList(player, thisEnemy);
                    GameUtility.ShuffleList(rushTileList);
                
                    foreach (var node in rushTileList)
                    {
                        if (_areaCheck.CanMove(thisEnemy, node))
                        {
                            if (_areaCheck.CanMove(player, node))
                            {
                                var playNearNode = _areaCheck.NearNode(player);
                                
                                foreach (var nearNode in playNearNode)
                                {
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
                    StartCoroutine(RushClose(targetPlayer, thisEnemy, middleNode, endTileNode));
                    thisEnemy.currentVigor = emptyVigor;
                }
                else
                {
                    print("They are over the wall!");
                }
            }
        }
        
        IEnumerator StartEnemies()
        {
            currentEnemy = null;
            GameUtility.ShuffleList(_gameManager.activeEnemyList);
            var gameOver = false;

            foreach (var thisEnemy in _gameManager.activeEnemyList.ToList())
            {
                currentEnemy = thisEnemy;
                print("Current enemy is " + currentEnemy.name);

                var doSomething = this.currentEnemy.baseVigor;
                var something = doSomething;
                var oneMore = new WaitUntil(() => this.currentEnemy.currentVigor < something);
                

                if (currentEnemy.characterType == CharacterType.SwordMaster)
                {
                    var baseList = _areaCheck.BaseList(currentEnemy);
                    var doubleList = _areaCheck.DoubleList(currentEnemy, baseList);

                    print(currentEnemy + " base count : " + baseList.Count + ", double count : " + doubleList.Count);

                    // hit 
                    if (baseList.Count != emptyCount && currentEnemy.currentVigor >= halfVigor)
                    {
                        GameUtility.ShuffleList(baseList);
                        BaseCloseStrike(baseList, currentEnemy);
                        
                        if (currentEnemy.currentVigor >= fullVigor)
                        {
                            BaseCloseObstacle(baseList, currentEnemy);
                        }
                    }
                    else if (doubleList.Count != emptyCount && currentEnemy.currentVigor >= fullVigor)
                    {
                        DoubleCloseStrike(doubleList, currentEnemy);
                        
                        if (currentEnemy.currentVigor >= fullVigor)
                        {
                            DoubleCloseObstacle(doubleList, currentEnemy);
                        }
                    }
                    
                    // chase
                    if (currentEnemy.currentVigor >= fullVigor)
                    {
                        var chaseList = _areaCheck.ChasePlayerList(_gameManager.activePlayerList, currentEnemy);
                        print(currentEnemy + " chase count : " + chaseList.Count);
                        
                        GameUtility.ShuffleList(chaseList);
                        ChaseCloseStrike(chaseList, currentEnemy);
                    }

                    // protect friends
                    if (currentEnemy.currentVigor >= fullVigor) 
                    {
                        GoNear(currentEnemy);
                    }
                    
                    RandomBase(currentEnemy);
                    currentEnemy.currentVigor = emptyVigor;
                }
                
                else if (currentEnemy.characterType == CharacterType.Ranger)
                {
                    var doubleList = _areaCheck.DoubleList(currentEnemy);
                    var moveList = _areaCheck.ShootPlayerList(_gameManager.activePlayerList, currentEnemy);
                    
                    print(currentEnemy + " range count : " + doubleList.Count + ", move and shoot count : " + moveList.Count);

                    if (doubleList.Count != emptyCount && currentEnemy.currentVigor >= halfVigor)
                    {
                        GameUtility.ShuffleList(doubleList);
                        DoubleRangeStrike(doubleList, currentEnemy);
                    }
                    if (moveList.Count != emptyCount && currentEnemy. currentVigor >= fullVigor)
                    {
                        GameUtility.ShuffleList(moveList);
                        MoveRangeStrike(moveList, currentEnemy);
                    }
                    else if (currentEnemy.currentVigor >= halfVigor) 
                    {
                        GoNear(currentEnemy);
                    }
                    
                    RandomBase(currentEnemy);
                    
                    currentEnemy.currentVigor = emptyVigor;
                }

                // if you make skill set, it'll need to change
                var condition = new WaitUntil(() => this.currentEnemy.currentVigor <= emptyVigor);
                yield return condition;

                while (currentEnemy.activeState == ActiveState.Moving)
                { 
                    yield return null;
                }
                
                while (currentEnemy.buyHit == BuyHit.BuyHit)
                { 
                    yield return null;
                }

                currentEnemy.turnState = TurnState.Ending;
                
                gameOver = _gameManager.CheckGameOver();
                if (gameOver) break;
            }

            foreach (var player in _gameManager.activePlayerList.ToList())
            {
                while (player.beHit == BeHit.BeHit)
                { 
                    yield return null;
                }
            }
            
            if (!gameOver)_gameManager.EnemyTurnEnding();
        }

        IEnumerator RushClose(Player player, Enemy thisEnemy, TileNode middleNode, TileNode endNode)
        {
            //	Rush means after Moving and Attack
            var firstWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, endNode);

            if (firstWalk)
            {
                while (thisEnemy.activeState == ActiveState.Moving)
                {
                    yield return null;
                }
                
                print("CloseDirect");
                thisEnemy.CloseAttackByEnemy(player);
                
                while (thisEnemy.buyHit == BuyHit.BuyHit)
                { 
                    yield return null;
                }
            }
            else
            {
                var thisWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Rush, middleNode);

                while (thisEnemy.activeState == ActiveState.Moving)
                {
                    yield return null;
                }
                
                thisEnemy.transform.LookAt(player.transform);
    
                var canWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, endNode);
                
                while (thisEnemy.activeState == ActiveState.Moving)
                {
                    yield return null;
                }

                if (canWalk)
                {
                    print("RushClose");
                    thisEnemy.CloseAttackByEnemy(player);
                }
                else
                {
                    var otherWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Random, null);
                }

                while (thisEnemy.buyHit == BuyHit.BuyHit)
                { 
                    yield return null;
                }
            }
            
            thisEnemy.currentVigor = emptyVigor;
        }

        IEnumerator ShootRange(Player player, TileNode selectedNode, Enemy thisEnemy)
        {
            //	Shoot means after Moving and Attack
            var canWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, selectedNode);

            while (thisEnemy.activeState == ActiveState.Moving)
            {
                yield return null;
            }

            if (canWalk)
            {
                print("ShootRange");
                thisEnemy.RangeAttackByEnemy(player);
                thisEnemy.currentVigor = emptyVigor;
            }

            while (thisEnemy.buyHit == BuyHit.BuyHit)
            { 
                yield return null;
            }
            
        }
        
        IEnumerator AttackRange(Player player, Enemy thisEnemy)
        {
            print("AttackRange");
            thisEnemy.RangeAttackByEnemy(player);

            while (thisEnemy.buyHit == BuyHit.BuyHit)
            { 
                yield return null;
            }
        }

        IEnumerator AttackClose(Player player, Enemy thisEnemy, TileNode shortNode)
        {
            var canWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Base, shortNode);
            
            while (thisEnemy.activeState == ActiveState.Moving)
            { 
                yield return null;
            }

            if (canWalk)
            {
                print("AttackClose");
                thisEnemy.CloseAttackByEnemy(player);
            }
            else
            {
                var otherWalk = thisEnemy.UnitWalk(AreaOrder.Base, WalkOrder.Random, null);
            }

            while (thisEnemy.buyHit == BuyHit.BuyHit)
            { 
                yield return null;
            }
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