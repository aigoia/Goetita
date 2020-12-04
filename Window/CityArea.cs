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
        public Vector3 height = new Vector3(0, 1.5f, 0);
        public List<CityArea> connectedNodeList;
        public List<Road> connectedRoadList;
    
        public GameManager gameManager;
        public AccidentManager accidentManager; 
        CharacterManager _characterManager;
        DataManager _dataManager;
    
        public IconBase iconBase;
        public Accident assignedAccident = null;
        public int id;
        public float moveSpeed = 1f;
        private int cycle = 4;
        private int eventCount = 4;
        private int marketCount = 1;
        private int hospitalCount = 1;

        private void Awake()
        {
            if (_dataManager == null) _dataManager = FindObjectOfType<DataManager>();
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
                // print(_cityAreaManager.mainCharacter);
                iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + height, 1f); 
                _cityAreaManager.initCount -= 1;
            

                _cityAreaManager.uiManager.ButtonOff();
                _cityAreaManager.uiManager.ButtonOn(this);
                return;
            }

            var originArea = _characterManager.WhereIam();
            if (originArea == null) return;
        
            // add turn
            if (originArea.connectedNodeList.Exists(i => i.name == this.name))
            {
                iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + height, moveSpeed);

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

                if (_dataManager.currentCharacterList.Count <= 2)
                {
                    movingCost = 20;
                }
                else if (_dataManager.currentCharacterList.Count == 3)
                {
                    movingCost = 30;
                }
                else if (_dataManager.currentCharacterList.Count == 4)
                {
                    movingCost = 50;
                }
                else if (_dataManager.currentCharacterList.Count == 5)
                {
                    movingCost = 90;
                }
                else if (_dataManager.currentCharacterList.Count == 6)
                {
                    movingCost = 160;
                }
                else
                {
                    movingCost = 200;
                }

                if (_dataManager.gold >= movingCost)
                {
                    _dataManager.gold = _dataManager.gold - movingCost;
                    _dataManager.RenewalGold();
                }
                else 
                {
                    LoseHp();
                    _dataManager.SaveAll();
                    return;
                }
            
                gameManager.mainCamera.Hungry(false);
                _dataManager.RenewalGold();
                

            }

            void LoseHp()
            {
                foreach (var character in _dataManager.currentCharacterList)
                {
                    if (character.CurrentHp > 0)
                    {
                        if (character.CurrentHp > 3)
                        {
                            character.CurrentHp = (int) character.CurrentHp / 2;
                        }
                        else if (character.CurrentHp <= 3)
                        {
                            character.CurrentHp = character.CurrentHp - 1;
                        }    
                    }

                    if (HpCheck())
                    {
                        return;
                    }
                    else
                    {
                        gameManager.mainCamera.Hungry(true);
                        print(character.CharacterName +" lose HP to " + character.CurrentHp);
                    }
                }
            }

            bool HpCheck()
            {
                foreach (var character in _dataManager.currentCharacterList)
                {
                    if (character.CurrentHp > 0)
                    {
                        return false;
                    }
                }
            
                gameManager.gameOver.Invoke();
                return true;
            }
        
            void PlusTurnDate()
            {
                _dataManager.turnDate += 1;
                _dataManager.RenewalTurnData();
            }
        
            var remainder = _dataManager.turnDate & cycle;
        
            // Make Accident
            if (remainder == 0)
            {
                accidentManager.InputAccident(eventCount, marketCount, hospitalCount);
            }
        
        }
    }
}
