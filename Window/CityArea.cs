using System.Collections.Generic;
using Game.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Window
{
    public class CityArea : MonoBehaviour
    {
        private CityAreaManager _cityAreaManager;
        public float censorExtents = 1f;
        
        public List<CityArea> connectedNodeList;
        public List<Road> connectedRoadList;
    
        public GameManager gameManager;
        public AccidentManager accidentManager; 
        CharacterManager _characterManager;

        public IconBase iconBase;
        public Accident assignedAccident = null;
        public int id;
        private int cycle = 4;
        private int eventCount = 4;
        private int marketCount = 1;
        private int hospitalCount = 1;

        private void Awake()
        {
            if (_cityAreaManager == null) _cityAreaManager = FindObjectOfType<CityAreaManager>();
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
            if (accidentManager == null) accidentManager = FindObjectOfType<AccidentManager>();
            if (_characterManager == null) _characterManager = FindObjectOfType<CharacterManager>();
        }
    
        private void OnMouseUp()
        {
            if (gameManager.mainCanvas.activeSelf == false) print("MainCanvas is off");
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (_cityAreaManager.initCount >= 1)
            {
                var position = this.transform.position;
                iTween.MoveTo(_cityAreaManager.mainCharacter, position + _cityAreaManager.height, _cityAreaManager.moveSpeed); 
                _cityAreaManager.initCount -= 1;
                _cityAreaManager.uiManager.ButtonOff();
                _cityAreaManager.uiManager.ButtonOn(this);
                _cityAreaManager.dataManager.SavePosition(position + _cityAreaManager.height);
                
                return;
            }

            var originArea = _characterManager.WhereIam();
            if (originArea == null) return;
        
            // add turn
            if (originArea.connectedNodeList.Exists(i => i.name == this.name))
            {
                iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + _cityAreaManager.height, _cityAreaManager.moveSpeed);

                PlusTurnDate();
                SpendMovingCost();
                // accidentManager.RemoveSelectedAccident();
                accidentManager.SpendAllExpiry(_cityAreaManager.cityAreaList);
                _cityAreaManager.uiManager.ButtonOff();
                _cityAreaManager.uiManager.ButtonOn(this);
            }

            void SpendMovingCost()
            {
                var movingCost = 20;

                if (_cityAreaManager.dataManager.currentCharacterList.Count <= 2)
                {
                    movingCost = 20;
                }
                else if (_cityAreaManager.dataManager.currentCharacterList.Count == 3)
                {
                    movingCost = 30;
                }
                else if (_cityAreaManager.dataManager.currentCharacterList.Count == 4)
                {
                    movingCost = 50;
                }
                else if (_cityAreaManager.dataManager.currentCharacterList.Count == 5)
                {
                    movingCost = 90;
                }
                else if (_cityAreaManager.dataManager.currentCharacterList.Count == 6)
                {
                    movingCost = 160;
                }
                else
                {
                    movingCost = 200;
                }

                if (_cityAreaManager.dataManager.baseData.currentGold >= movingCost)
                {
                    _cityAreaManager.dataManager.baseData.currentGold = _cityAreaManager.dataManager.baseData.currentGold - movingCost;
                    _cityAreaManager.dataManager.RenewalGold();
                }
                else 
                {
                    LoseHp();
                    _cityAreaManager.dataManager.SaveAll();
                    _cityAreaManager.dataManager.SavePosition(transform.position + _cityAreaManager.height);
                    return;
                }
            
                gameManager.mainCamera.Hungry(false);
                _cityAreaManager.dataManager.RenewalGold();
                _cityAreaManager.dataManager.SavePosition(transform.position + _cityAreaManager.height);
            }

            void LoseHp()
            {
                foreach (var character in _cityAreaManager.dataManager.currentCharacterList)
                {
                    if (character.currentHp > 0)
                    {
                        if (character.currentHp > 3)
                        {
                            character.currentHp = (int) character.currentHp / 2;
                        }
                        else if (character.currentHp <= 3)
                        {
                            character.currentHp = character.currentHp - 1;
                        }    
                    }

                    if (HpCheck())
                    {
                        return;
                    }
                    else
                    {
                        gameManager.mainCamera.Hungry(true);
                        print(character.characterName +" lose HP to " + character.currentHp);
                    }
                }
            }

            bool HpCheck()
            {
                foreach (var character in _cityAreaManager.dataManager.currentCharacterList)
                {
                    if (character.currentHp > 0)
                    {
                        return false;
                    }
                }
            
                gameManager.gameOver.Invoke();
                return true;
            }
        
            void PlusTurnDate()
            {
                _cityAreaManager.dataManager.baseData.turnDate += 1;
                _cityAreaManager.dataManager.RenewalTurnData();
            }
        
            var remainder = _cityAreaManager.dataManager.baseData.turnDate & cycle;
        
            // Make Accident
            if (remainder == 0)
            {
                accidentManager.InputAccident(eventCount, marketCount, hospitalCount);
            }
        
        }
    }
}
