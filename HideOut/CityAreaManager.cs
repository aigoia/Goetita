using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Profile;
using UnityEngine;
using UnityEngine.Serialization;

public class CityAreaManager : MonoBehaviour
{
    public GameObject mainCharacter;
    public List<CityArea> cityAreaList;
    public float roadCensor = 1f;
    public IconManager iconManager;

    private void Awake()
    {
        if (iconManager == null) iconManager = FindObjectOfType<IconManager>();
        if (mainCharacter == null) print("MainCharacter is Missing!");
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
			
            var hitList = Physics.BoxCastAll(thisTransform.position, Vector3.one * roadCensor, forward, Quaternion.identity, 0, LayerMask.GetMask ("Road"));
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