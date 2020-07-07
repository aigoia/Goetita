using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Profile;
using UnityEngine;

public class CityArea : MonoBehaviour
{
    private CityAreaManager _cityAreaManager;
    private CharacterManager _characterManager;
    public float censorExtents = 1f;
    public Vector3 height = new Vector3(0, 1.5f, 0);
    public List<CityArea> connectedNodeList;
    public List<Road> connectedRoadList;
    public bool testIconOn = false;

    private void Awake()
    {
        if (_characterManager == null) _characterManager = FindObjectOfType<CharacterManager>();
        if (_cityAreaManager == null) _cityAreaManager = FindObjectOfType<CityAreaManager>();
    }

    private void OnMouseDown()
    {
        // print("Click City");
        var origins = Physics.BoxCastAll(_characterManager.mainCharacter.transform.position, Vector3.one * censorExtents, Vector3.forward, Quaternion.identity, 0, LayerMask.GetMask("CityArea"));
        var originArea = origins[0].transform.GetComponent<CityArea>();
        if (originArea == null) return;

        if (originArea.connectedNodeList.Exists(i => i.name == this.name))
        {
            iTween.MoveTo(_characterManager.mainCharacter, this.transform.position + height, 1f);   
        }
    }
    
    
}
