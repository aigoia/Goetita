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
        public Settings settings;

        private void Awake()
        {
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
            if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
            if (settings == null) settings = FindObjectOfType<Settings>();
        }

        private void Start()
        {
            currentCharacterList = settings.LoadCharacter();
          
            PlayerOn();
            Setting();
            gameManager.MakeActivePlayerList(gameManager.playerList, gameManager.activePlayerList);
            uiManager.InitSetting();
        }

        private void Setting()
        {
            foreach (var character in currentCharacterList)
            {
                if (character.currentHp <= 0) continue;
                
                var player = gameManager.playerList.Find(i => i.characterId == character.characterId);
                // print(player.name);
                // print("(" + character.CharacterId + ") : " + character.BaseHp);
                player.baseHp = character.baseHp;
                player.currentHp = character.currentHp;
                player.copyHp = character.currentHp;

                player.baseDeal = character.baseDeal;
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
                print(character.characterName);

                if (character.type == CharacterClass.Claymore)
                {
                    var swordMaster = swordMasters.Find( sword =>
                        sword.GetComponent<Player>().characterId == character.characterId);
                    swordMaster.transform.position = initVectors[i];
                    swordMaster.SetActive(true);
                
                    s = s + 1;
                    i = i + 1;
                }
                else if (character.type == CharacterClass.Ranger)
                {
                    var ranger = rangers.Find( gun =>
                        gun.GetComponent<Player>().characterId == character.characterId);
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
        }
        
        public void WinNormal()
        {
            if (settings.currentData == null) return;
            if (settings.currentData.currentMission != null)
            {
                print("Gold : " + settings.currentData.currentMission.goldReward +", Exp : " + settings.currentData.currentMission.expReward);
                WinReword(settings.currentData.currentMission.goldReward, settings.currentData.currentMission.expReward);   
            }
            settings.currentData.currentMission = null;
        }

        void WinReword(int rewordGold = 0, int rewordExp = 0)
        {
            settings.baseData.currentGold = settings.baseData.currentGold + rewordGold;
            settings.SaveBaseData();
            AddEqualExp(rewordExp);
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
                print(character.characterName + " (" + character.characterId + ") : ");
                print("Base HP : " + character.baseHp + ", Current HP : " + character.currentHp);
                print("Level : " + character.level + ", EXP : " + character.exp);
            }
        }
    }
}
