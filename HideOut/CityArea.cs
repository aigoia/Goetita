using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.HideOut;
using Profile;
using UnityEngine;

public class CityArea : MonoBehaviour
{
    private CityAreaManager _cityAreaManager;
    public float censorExtents = 1f;
    public Vector3 height = new Vector3(0, 1.5f, 0);
    public List<CityArea> connectedNodeList;
    public List<Road> connectedRoadList;
    
    public bool hasMarket = false;
    public bool hasEvent = false;

    public IconBase iconBase;
    
    private void Awake()
    {
        if (_cityAreaManager == null) _cityAreaManager = FindObjectOfType<CityAreaManager>();
    }
    
    private void OnMouseUp()
    {
        if (_cityAreaManager.initCount >= 1)
        {
            print(_cityAreaManager.mainCharacter);
            iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + height, 1f); 
            _cityAreaManager.initCount -= 1;
            
            _cityAreaManager.ButtonOff();
            _cityAreaManager.ButtonOn(this);
            return;
        }

        var origins = Physics.BoxCastAll(_cityAreaManager.mainCharacter.transform.position, Vector3.one * censorExtents, Vector3.forward, Quaternion.identity, 0, LayerMask.GetMask("CityArea"));

        if (origins.Length == 0) return;
        
        var originArea = origins[0].transform.GetComponent<CityArea>();
        if (originArea == null) return;
        
        if (originArea.connectedNodeList.Exists(i => i.name == this.name))
        {
            iTween.MoveTo(_cityAreaManager.mainCharacter, this.transform.position + height, 1f);
            _cityAreaManager.ButtonOff();
            _cityAreaManager.ButtonOn(this);
        }
    }
}
