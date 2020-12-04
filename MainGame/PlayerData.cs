using System.Collections.Generic;
using Game.Data;
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

        private void Awake()
        {
            currentCharacterList = ES3.Load<List<Character>>("Characters", "Game");
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
            if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        }

        private void Start()
        {
            PlayerOn();
            Setting();
            gameManager.MakeActivePlayerList(gameManager.playerList, gameManager.activePlayerList);
            uiManager.InitSetting();
            
            
        }
        
        
        private void Setting()
        {
            foreach (var character in currentCharacterList)
            {
                if (character.CurrentHp <= 0) continue;
                
                var player = gameManager.playerList.Find(i => i.characterName == character.CharacterName);
                // print(player.name);
                // print("(" + character.CharacterId + ") : " + character.BaseHp);
                player.baseHp = character.BaseHp;
                player.currentHp = character.CurrentHp;
                player.copyHp = character.CurrentHp;

                player.baseDeal = character.BaseDeal;
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
                if (character.CurrentHp <= 0) continue;
                print(character.CharacterName);

                if (character.Type == CharacterClass.Claymore)
                {
                    var swordMaster = swordMasters.Find( sword =>
                        sword.GetComponent<Player>().characterName == character.CharacterName);
                    swordMaster.transform.position = initVectors[i];
                    swordMaster.SetActive(true);
                
                    s = s + 1;
                    i = i + 1;
                }
                else if (character.Type == CharacterClass.Ranger)
                {
                    var ranger = rangers.Find( gun =>
                        gun.GetComponent<Player>().characterName == character.CharacterName);
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

        public void WinNormal()
        {
            WinReword(200, 9);
        }
        
        public void WinReword(int rewordGold = 0, int rewordExp = 0)
        {
            var gold = ES3.Load<int>("Gold", "Game");
            gold = gold + rewordGold;
            ES3.Save<int>("Gold", gold, "Game");
            
            AddEqualExp(rewordExp);
        }

        void AddEqualExp(int exp)
        {
            var each = (int) exp / currentCharacterList.Count;
            
            foreach (var character in currentCharacterList)
            {
                if (character.CurrentHp <= 0) continue;
                character.Exp = character.Exp + each;
                
                print(character.Exp);
            }
            
            ES3.Save<List<Character>>("Characters", currentCharacterList, "Game");
        }
        
        [ContextMenu("Print Information")]
        public void PrintInformation()
        {
            var characterList = ES3.Load<List<Character>>("Characters", "Game");
            
            foreach (var character in characterList)
            {
                print(character.CharacterName + " (" + character.CharacterId + ") : ");
                print("Base HP : " + character.BaseHp + ", Current HP : " + character.CurrentHp);
                print("Level : " + character.Level + ", EXP : " + character.Exp);
            }
        }
    }
}
