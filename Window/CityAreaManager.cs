using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;

namespace Game.Window
{
    public class CityAreaManager : MonoBehaviour
    {
        public GameObject mainCharacter;
        public List<CityArea> cityAreaList;
        public float roadCensor = 2.5f;
        public GameObject testIcon;
        public UiManager uiManager;
        public Vector3 areaScale = new Vector3(5f, 5f, 5f);
        public int initCount = 1;
        public Vector3 height = new Vector3(0, 0.5f, 0);
        public float moveSpeed = 1f;
        public DataManager dataManager;

        private void Awake()
        {
            if (mainCharacter == null) print("MainCharacter is Missing!");
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
            if (testIcon == null) print("TestIcon is Missing");
            if (uiManager == null) uiManager = FindObjectOfType<UiManager>();
        }

        private void Start()
        {
            UiCheck();
            dataManager.LoadPosition();
            iTween.MoveTo(mainCharacter, dataManager.whereData.currentPosition, moveSpeed);
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
    
        void UiCheck()
        {
            uiManager.ButtonOff();
        
            var origin = new Vector3(mainCharacter.transform.position.x, 0f, mainCharacter.transform.position.z);
        
            var hits = Physics.BoxCastAll(origin, areaScale, Vector3.up, 
                Quaternion.identity, 0f, LayerMask.GetMask("CityArea"));
        
            // print(hits[0].transform.name);
            if (hits.Length == 0) return;
            var cityArea = hits[0].transform.GetComponent<CityArea>();
            if (cityArea == null) return;
            
            uiManager.ButtonOn(cityArea);
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
}