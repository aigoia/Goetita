using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.HideOut;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CityAreaManager : MonoBehaviour
{
    public GameObject mainCharacter;
    public List<CityArea> cityAreaList;
    public float roadCensor = 2.5f;
    public GameObject testIcon;
    public UiManager uiManager;
    public Vector3 areaScale = new Vector3(5f, 5f, 5f);
    public int initCount = 1;

    private void Awake()
    {
        if (mainCharacter == null) print("MainCharacter is Missing!");
        if (testIcon == null) print("TestIcon is Missing");
        if (uiManager == null) uiManager = FindObjectOfType<UiManager>();
    }

    private void Start()
    {
        UiCheck();
    }

    [ContextMenu("Input ID")]
    public void InputId()
    {
        var n = 0;
        
        foreach (var cityArea in cityAreaList)
        {
            cityArea.id = n;
            n += 1;
        }
    }

    public void ButtonOff()
    {
        uiManager.eventButton.SetActive(false);
        uiManager.marketButton.SetActive(false);
        uiManager.inventoryButton.SetActive(false);
    }

    public void ButtonOn(CityArea cityArea)
    {
        if (cityArea.assignedAccident == null)
        {
            uiManager.eventButton.SetActive(false);
            uiManager.eventButton.SetActive(false);
            uiManager.inventoryButton.SetActive(true);
            return;
        }
        
        if (cityArea.assignedAccident.accidentType == AccidentType.Event)
        {
            uiManager.eventButton.SetActive(true);    
        }
        
        if (cityArea.assignedAccident.accidentType == AccidentType.Market)
        {
            uiManager.marketButton.SetActive(true);
        }

        uiManager.inventoryButton.SetActive(true);
    }


    void UiCheck()
    {
        ButtonOff();
        
        var origin = new Vector3(mainCharacter.transform.position.x, 0f, mainCharacter.transform.position.z);
        
        var hits = Physics.BoxCastAll(origin, areaScale, Vector3.up, 
            Quaternion.identity, 0f, LayerMask.GetMask("CityArea"));
        
        // print(hits[0].transform.name);
        if (hits.Length == 0) return;
        
        var cityArea = hits[0].transform.GetComponent<CityArea>();

        if (cityArea == null) return;
        
        ButtonOn(cityArea);
    }

    [ContextMenu("Collect Nodes")]
    public void CollectNodes()
    {
        cityAreaList = transform.GetComponentsInChildren<CityArea>().ToList();
    }
    
    [ContextMenu("Collect Connected Roads")]
    public void CollectConnectedRoad()
    {
        foreach (var cityArea in cityAreaList)
        {
            // print(cityArea.name);
            var thisTransform = cityArea.transform;
            var forward = thisTransform.forward;
            var connectedRoadList = new List<Road>();
			
            var hitList = Physics.SphereCastAll(thisTransform.position, roadCensor, forward, 0, LayerMask.GetMask ("Road"));
            foreach (var hit in hitList)
            {
                var newHit = hit.transform.GetComponent<Road>();
                if (newHit == null) continue;
                connectedRoadList.Add(newHit);
            }

            cityArea.connectedRoadList = connectedRoadList;
        }
    }

    [ContextMenu("Collect Connected Nodes")]
    public void CollectConnectedNode()
    {
        foreach (var cityArea in cityAreaList)
        {
            if (cityArea.connectedRoadList == null) continue;
            
            var connectedRoadList = cityArea.connectedRoadList;
            var connectedNodeList = new List<CityArea>();
            var nodeList = cityAreaList;
            
            foreach (var newRoad in connectedRoadList)
            {
                foreach (var node in nodeList)
                {
                    foreach (var oldRoad in node.connectedRoadList)
                    {
                        if (newRoad == oldRoad)
                        {
                            if (node == transform.GetComponent<CityArea>()) continue;
                            if (node == cityArea) continue;
                                
                            connectedNodeList.Add(node);
                        }
                    }
                }
            }
            
            cityArea.connectedNodeList = connectedNodeList;
        }
    }
}