using System;
using System.Collections.Generic;
using Game.Data;
using Game.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Window
{
    public class CardManager : MonoBehaviour
    {
        public GameObject loadingScene;
        public DataManager dataManager;
        public CharacterManager characterManager;
        public List<Card> cardList = new List<Card>();
        public string easyText = "Easy";
        public string normalText = "Normal";
        public string hardText = "Hard";
        private GameManager _gameManager;

        private void Awake()
        {
            if (dataManager == null) FindObjectOfType<DataManager>();
            if (dataManager.currentData == null) dataManager.currentData = FindObjectOfType<CurrentData>();
            if (characterManager == null) characterManager = FindObjectOfType<CharacterManager>();
            if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
        }
        

        public void MakePanel()
        {
            foreach (var card in cardList)
            {
                if (card.cardType == CardType.Mission)
                {
                    var mission = characterManager.WhereIam().assignedAccident.mission;
                    card.head.text = mission.missionName;
                    card.gold.text = mission.goldReward.ToString() + " G";
                    
                    // print(mission.difficultLevel);
                    if (mission.difficultLevel == DifficultLevel.Easy)
                    {
                        card.information.text = easyText;
                    }
                    else if (mission.difficultLevel == DifficultLevel.Normal)
                    {
                        card.information.text = normalText;
                    }
                    else if (mission.difficultLevel == DifficultLevel.Hard)
                    {
                        card.information.text = hardText;
                    }
                }
                else if (card.cardType == CardType.SwordRecruit)
                {
                    card.head.text = dataManager.claymoreClass;
                    card.gold.text = dataManager.recruitSword + " G";       
                }
                else if (card.cardType == CardType.GunRecruit)
                {
                    card.head.text = dataManager.rangerClass;
                    card.gold.text = dataManager.recruitGun + " G";       
                }
                 
            }
        }

        public void GoToMainMap()
        {
            if (dataManager.currentData == null) dataManager.currentData = FindObjectOfType<CurrentData>();

            var cityArea = characterManager.WhereIam();
            dataManager.currentData.currentMission = cityArea.assignedAccident.mission;
            
            var characters = dataManager.LoadCharacter();
            loadingScene.SetActive(true);
            
            if (characters != null)
            {
                foreach (var character in characters)
                {
                    if (character.currentHp < 1)
                    {
                        character.currentHp = 1;
                    }
                }

                dataManager.SaveCharacter(characters);
            }
            
            if (_gameManager.audioManager != null) _gameManager.audioManager.FadeOutCaller();
            print("Map : " + cityArea.assignedAccident.mission.mapName + ", Mission : " + cityArea.assignedAccident.mission.missionName);
            SceneManager.LoadScene("Main");
        }
    }

    public enum CardType
    {
        Non, Mission, SwordRecruit, GunRecruit, DamageUp, HpUp, SpecialUp
    }
    
    [System.Serializable]
    public class Card
    {
        public CardType cardType;
        public TextMeshProUGUI head;
        public TextMeshProUGUI information;
        public TextMeshProUGUI gold;
        
        public Card(CardType cardType, TextMeshProUGUI head, TextMeshProUGUI information, TextMeshProUGUI gold)
        {
            this.cardType = cardType;
            this.head = head;
            this.information = information;
            this.gold = gold;
        }
    }
}
