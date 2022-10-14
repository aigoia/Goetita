using System.Collections.Generic;
using Game.Data;
using Game.Menu;
using UnityEngine;
using Character = Game.Data.Character;

namespace Game.MainGame
{
    public class PlayerData : MonoBehaviour
    {
        public List<GameObject> swordMasters;
        public List<GameObject> rangers;
        public List<Character> currentCharacterList = new List<Character>();
        public GameManager gameManager;
        public UIManager uiManager;
        public List<Vector3> initVectors = new List<Vector3>();
        public Settings settings;
        public Board board;
        public int memoryFactor = 7;
        private AudioManager _audioManager;

        private void Awake()
        {
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
            if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
            if (settings == null) settings = FindObjectOfType<Settings>();
            if (board == null) board = FindObjectOfType<Board>();
            if (_audioManager == null) _audioManager = FindObjectOfType<AudioManager>();
        }

        private void Start()
        {
            currentCharacterList = settings.LoadCharacter();
            
            PlayerOn();
            Setting();
            gameManager.MakeActivePlayerList(gameManager.playerList, gameManager.activePlayerList);
            uiManager.InitSetting();
            SetItems();
            board.ResetBoard();
            
            MakeDeck();
        }
        
        void MakeDeck()
        {
            var deckTypes = new List<DeckType>();

            foreach (var character in currentCharacterList)
            {
                if (character.classType == CharacterClass.Claymore)
                {
                    deckTypes.Add(DeckType.ShadowWalk);

                    if (character.characterName == "Tina")
                    {
                        deckTypes.Add(DeckType.ShadowWalk);
                    }

                    foreach (var item in character.itemList)
                    {
                        if (item.itemType == ItemType.Weapon)
                        {
                            if (item.itemGrade == ItemGrade.One)
                            {
                                deckTypes.Add(DeckType.ShadowWalk);
                            }
                            else if (item.itemGrade == ItemGrade.Two)
                            {
                                deckTypes.Add(DeckType.ShadowWalk);
                                deckTypes.Add(DeckType.ShadowWalk);
                            }
                            else if (item.itemGrade == ItemGrade.Three)
                            {
                                deckTypes.Add(DeckType.ShadowWalk);
                                deckTypes.Add(DeckType.ShadowWalk);
                                deckTypes.Add(DeckType.ShadowWalk);
                            }
                            else if (item.itemGrade == ItemGrade.Four)
                            {
                                deckTypes.Add(DeckType.ShadowWalk);
                                deckTypes.Add(DeckType.ShadowWalk);
                                deckTypes.Add(DeckType.ShadowWalk);
                                deckTypes.Add(DeckType.ShadowWalk);
                            }
                            break;
                        }
                    }
                }
                else if (character.classType == CharacterClass.Ranger)
                {
                    deckTypes.Add(DeckType.PowerFull);
                    
                    if (character.characterName == "Tina")
                    {
                        deckTypes.Add(DeckType.PowerFull);
                    }
                    
                    foreach (var item in character.itemList)
                    {
                        if (item.itemType == ItemType.Weapon)
                        {
                            if (item.itemGrade == ItemGrade.One)
                            {
                                deckTypes.Add(DeckType.PowerFull);
                            }
                            else if (item.itemGrade == ItemGrade.Two)
                            {
                                deckTypes.Add(DeckType.PowerFull);
                                deckTypes.Add(DeckType.PowerFull);
                            }
                            else if (item.itemGrade == ItemGrade.Three)
                            {
                                deckTypes.Add(DeckType.PowerFull);
                                deckTypes.Add(DeckType.PowerFull);
                                deckTypes.Add(DeckType.PowerFull);
                            }
                            else if (item.itemGrade == ItemGrade.Four)
                            {
                                deckTypes.Add(DeckType.PowerFull);
                                deckTypes.Add(DeckType.PowerFull);
                                deckTypes.Add(DeckType.PowerFull);
                                deckTypes.Add(DeckType.PowerFull);
                            }
                            
                            break;
                        }
                    }
                }
            }

            GameUtility.ShuffleList(deckTypes);
            gameManager.deckManager.cardTypeList = deckTypes;

            var cards = gameManager.deckManager.cardList;
            
            gameManager.deckManager.ResetCard();
            
            int count = 0;
            foreach (var deck in deckTypes)
            {
                print(deck);
                
                if (count < 3)
                {
                    cards[count].gameObject.SetActive(true);
                    cards[count].deckType = deck;
                    print(deck);
                    var image = cards[count].images.Find(i => i.name == deck.ToString());
                    // print(image);
                    image.gameObject.SetActive(true);

                    count += 1;
                }
                else
                {
                    gameManager.deckManager.waitingList.Add(deck);
                }
            }

            foreach (var card in gameManager.deckManager.cardList)
            {
                card.gameObject.SetActive(false);
            }
        }
        
        void SetItems()
        {
            foreach (var character in currentCharacterList)
            {
                if (character.itemList == null) continue;
                if (character.baseHp <= 0) continue;

                var player = gameManager.activePlayerList.Find(i => i.characterName == character.characterName);
                if (player == null) continue;
                
                
                // setting default item
                if (character.itemList.Exists(i => i.itemType == ItemType.Weapon))
                {
                    foreach (var weapon in player.weaponList)
                    {
                        weapon.gameObject.SetActive(false);
                        // print(weapon.name);
                    }

                    var itemWeapon = character.itemList.Find(i => i.itemType == ItemType.Weapon);
                    // print(itemWeapon.itemName);
                    
                    player.weaponList.Find(i => i.name == itemWeapon.itemName).gameObject.SetActive(true);
                    if (itemWeapon.itemName == "Hammer") player.isImpact = true;
                }
                else
                {
                    if (character.classType == CharacterClass.Claymore)
                    {
                        var baseSword = settings.baseItemList.Find(i => i.itemName == "Base Sword");

                        if (baseSword != null)
                        {
                            player.baseDeal = player.baseDeal + baseSword.baseInt;
                            player.plusDeal = player.plusDeal + baseSword.plusInt;
                        }
                    }
                    else if (character.classType == CharacterClass.Ranger)
                    {
                        var baseGun = settings.baseItemList.Find(i => i.itemName == "Base Gun");
					
                        if (baseGun != null)
                        {
                            player.baseDeal = player.baseDeal + baseGun.baseInt;
                            player.plusDeal = player.plusDeal + baseGun.plusInt;
                        }
                    }
                }
                if (character.itemList.Exists(i => i.itemType == ItemType.Armor))
                {
				    
                }
                else
                {
                    
                    var loadBaseArmor = settings.baseItemList.Find(i => i.itemName == "Base Armor");
                        
                    if (loadBaseArmor != null) 
                    {
                            player.armor = player.armor + loadBaseArmor.baseInt;
                    }	
                    
                }
                
                // setting items
                foreach (var item in character.itemList)
                {
                    if (item.itemType == ItemType.Weapon)
                    {
                        player.baseDeal = player.baseDeal + item.baseInt;
                        player.plusDeal = player.plusDeal + item.plusInt;

                        if (item.trait == Data.Trait.ArmorPiercing)
                        {
                            player.attackType = AttackType.ArmorPiercing;
                        }
                    } 
                    else if (item.itemType == ItemType.Armor)
                    {
                        player.armor = player.armor = item.baseInt;
                        player.baseHp = player.baseHp + item.plusInt;
                        player.currentHp = player.currentHp + item.plusInt;
                    }
                }
                
                print(player.characterName + " deal : " + player.baseDeal + " + " + player.plusDeal);
                print(player.characterName + " armor : " + player.armor);
                print(player.characterName + " hp : " + player.currentHp + " / " + player.baseHp);
                print(player.characterName + " attack type : " + player.attackType);
            }
        }

        private void Setting()
        {
            foreach (var character in currentCharacterList)
            {
                if (character.currentHp <= 0) continue;

                var player = gameManager.playerList.Find(i => i.characterName == character.characterName);
                // print(player.name);
                // print("(" + character.CharacterId + ") : " + character.BaseHp);
                if (player == null) continue;
                
                player.baseHp = character.baseHp;
                player.currentHp = character.currentHp;
                player.copyHp = character.currentHp;
                player.baseDeal = character.baseDeal;
                player.plusDeal = character.plusDeal;
                player.armor = character.baseArmor;
                // print(player.characterName + " deal : " + player.baseDeal + " + " + player.plusDeal);
            }
        }

        void PlayerOn()
        {
            int i = 0;
            int s = 0;
            int r = 0;
        
            gameManager.playerList = new List<Player>();

            foreach (var character in currentCharacterList)
            {
                if (character.currentHp <= 0) continue;
                // print(character.characterName);

                if (character.classType == CharacterClass.Claymore)
                {
                    var swordMaster = swordMasters.Find( sword =>
                        sword.GetComponent<Player>().characterName == character.characterName);
                    swordMaster.transform.position = initVectors[i];
                    swordMaster.SetActive(true);
                
                    s = s + 1;
                    i = i + 1;
                }
                else if (character.classType == CharacterClass.Ranger)
                {
                    var ranger = rangers.Find( gun =>
                        gun.GetComponent<Player>().characterName == character.characterName);
                    ranger.transform.position = initVectors[i];
                    ranger.SetActive(true);
                    
                    r = r + 1;
                    i = i + 1;
                }
                else
                {
                    print("No Type One");
                }
            }

            gameManager.playerList = new List<Player>(FindObjectsOfType<Player>());
        }

        public void LoseNormal()
        {
            if (settings.currentData == null) return;
            
            settings.currentData.currentMission = null;
            if (_audioManager != null) _audioManager.FadeOutCaller();
        }
        
        public void WinNormal()
        {
            if (settings.currentData == null) return;
            if (settings.currentData.currentMission != null)
            {
                // print("Gold : " + settings.currentData.currentMission.goldReward +", Exp : " + settings.currentData.currentMission.expReward);
                settings.mapName.text = settings.currentData.currentMission.missionName;
                settings.mapGold.text = settings.currentData.currentMission.goldReward.ToString();
                settings.mapExp.text = settings.currentData.currentMission.expReward.ToString();

                print("Ok");
                WinReward(settings.currentData.currentMission.goldReward, 
                    settings.currentData.currentMission.expReward);   
            }
            settings.currentData.currentMission = null;
            if (_audioManager != null) _audioManager.FadeOutCaller();
        }

        void WinReward(int rewordGold = 0, int rewordExp = 0)
        {
            settings.baseData.currentGold = settings.baseData.currentGold + rewordGold;
            settings.SaveBaseData();
            AddEqualExp(rewordExp);
            
            settings.LoadOwnedItems();
            int card = rewordExp / memoryFactor;
            // print(card);
            for (int i = 0; i < card; i++)
            {
                settings.ownedItems.Add(FindBaseItem("Memory"));
            }
            settings.SaveOwnedItems();
        }
        
        Item FindBaseItem(string itemName)
        {
            return settings.baseItemList.Find(i => i.itemName == itemName);
        }

        void AddEqualExp(int exp)
        {
            var each = (int) exp / currentCharacterList.Count;
            
            foreach (var character in currentCharacterList)
            {
                if (character.currentHp <= 0) continue;
                character.exp = character.exp + each;
                
                print(character.characterName + " EXP : character.Exp");
            }
            
            settings.SaveCharacter(currentCharacterList);
        }
        
        [ContextMenu("Print Information")]
        public void PrintInformation()
        {
            var characterList = settings.LoadCharacter();
            
            foreach (var character in characterList)
            {
                print(character.characterName + " (" + character.characterName + ") : ");
                print("Base HP : " + character.baseHp + ", Current HP : " + character.currentHp);
                print("Level : " + character.level + ", EXP : " + character.exp);
            }
        }
    }
}
