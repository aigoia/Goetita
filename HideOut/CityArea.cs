using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Data;
using Game.HideOut;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (_cityAreaManager.initCount >= 1)
        {
            // print(_cityAreaManager.mainCharacter);
            iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + height, 1f); 
            _cityAreaManager.initCount -= 1;
            
            _cityAreaManager.ButtonOff();
            _cityAreaManager.ButtonOn(this);
            return;
        }

        var originArea = _characterManager.WhereIam();
        if (originArea == null) return;
        
        // add turn
        if (originArea.connectedNodeList.Exists(i => i.name == this.name))
        {
            iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + height, moveSpeed);
            var waitForSeconds = new WaitForSeconds(moveSpeed);
            
            PlusTurnDate();
            accidentManager.removeSelectedAccident();
            
            accidentManager.SpendAllExpiry(_cityAreaManager.cityAreaList);
            _cityAreaManager.ButtonOff();
            _cityAreaManager.ButtonOn(this);
        }

        void PlusTurnDate()
        {
            _dataManager.turnDate += 1;
            _dataManager.RenewalTurnData();
        }
    }
}
