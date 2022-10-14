using System;
using System.Collections.Generic;
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
        public int cycle = 4;
        public int eventCount = 3;
        public int marketCount = 1;
        public int hospitalCount = 1;
        public int recruitCount = 1;
        private MeshRenderer _cityMesh;
        private MeshRenderer _baseMesh;
        
        private void Awake()
        {
            if (_cityAreaManager == null) _cityAreaManager = FindObjectOfType<CityAreaManager>();
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
            if (accidentManager == null) accidentManager = FindObjectOfType<AccidentManager>();
            if (_characterManager == null) _characterManager = FindObjectOfType<CharacterManager>();

            if (_cityMesh == null) _cityMesh = transform.Find("City").gameObject.GetComponent<MeshRenderer>();
            if (_baseMesh == null) _baseMesh = transform.Find("Base").gameObject.GetComponent<MeshRenderer>();
        }

        private void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var originArea = _characterManager.WhereIam();
            
            if (originArea == null)
            {
                _cityMesh.material = _cityAreaManager.pointMaterial;
                gameManager.hover.Play();
                return;
            }
            
            if (originArea.connectedNodeList.Exists(i => i.name == this.name))
            {
                _cityMesh.material = _cityAreaManager.pointMaterial;
                gameManager.hover.Play();
                return;
            }
        }

        private void OnMouseExit()
        {
            _cityMesh.material = _cityAreaManager.baseMaterial;
        }

        private void OnMouseUp()
        {
            if (gameManager.mainCanvas.activeSelf == false) print("MainCanvas is off");
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var originArea = _characterManager.WhereIam();
            
            if (originArea == null)
            {
                var position = this.transform.position;
                iTween.MoveTo(_cityAreaManager.mainCharacter, position + _cityAreaManager.height, _cityAreaManager.moveSpeed); 
                // gameManager.dataManager.baseData.initCount -= 1;
                _cityAreaManager.uiManager.ButtonOff();
                _cityAreaManager.uiManager.ButtonOn(this);
                _cityAreaManager.dataManager.SavePosition(position + _cityAreaManager.height);
                
                return;
            }
            
            // add turn
            if (originArea.connectedNodeList.Exists(i => i.name == this.name))
            {
                iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + _cityAreaManager.height, _cityAreaManager.moveSpeed);
                
                gameManager.move.Play();
                
                PlusTurnDate();
                SpendMovingCost();
                // accidentManager.RemoveSelectedAccident();
                accidentManager.SpendAllExpiry(_cityAreaManager.cityAreaList);
                _cityAreaManager.uiManager.ButtonOff();
                _cityAreaManager.uiManager.ButtonOn(this);
            }
            

            void SpendMovingCost()
            {
                var movingCost = _cityAreaManager.movingCost[0];

                if (_cityAreaManager.dataManager.CurrentCharacterList.Count <= 1)
                {
                    movingCost = _cityAreaManager.movingCost[0];
                }
                else if (_cityAreaManager.dataManager.CurrentCharacterList.Count == 2)
                {
                    movingCost = _cityAreaManager.movingCost[1];
                }
                else if (_cityAreaManager.dataManager.CurrentCharacterList.Count == 3)
                {
                    movingCost = _cityAreaManager.movingCost[2];
                }
                else if (_cityAreaManager.dataManager.CurrentCharacterList.Count == 4)
                {
                    movingCost = _cityAreaManager.movingCost[3];
                }
                else if (_cityAreaManager.dataManager.CurrentCharacterList.Count == 5)
                {
                    movingCost = _cityAreaManager.movingCost[4];
                }
                else if (_cityAreaManager.dataManager.CurrentCharacterList.Count == 6)
                {
                    movingCost = _cityAreaManager.movingCost[5];
                }
                else
                {
                    movingCost = _cityAreaManager.movingCost[0];
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
                gameManager.mainCamera.Hungry(true);

                var characters = _cityAreaManager.dataManager.CurrentCharacterList;
                
                foreach (var character in characters)
                {
                    if (character.currentHp > 1)
                    {
                        if (character.currentHp > 3)
                        {
                            character.currentHp = (int) character.currentHp / 2;
                        }
                        if (character.currentHp <= 3)
                        {
                            character.currentHp = character.currentHp - 1;
                        }
                    }

                    // if (HpCheck())
                    // {
                    //     return;
                    // }
                    else
                    {
                        gameManager.mainCamera.Hungry(true);
                        print(character.characterName +" lose HP to " + character.currentHp);
                    }
                }
                
                _cityAreaManager.dataManager.SaveCharacter(characters);
            }

            bool HpCheck()
            {
                foreach (var character in _cityAreaManager.dataManager.CurrentCharacterList)
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
                accidentManager.InputAccident(eventCount, marketCount, hospitalCount, recruitCount);
            }
        
        }
    }
}
