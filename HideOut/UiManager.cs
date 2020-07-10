using System;
using System.Collections.Generic;
using Game.MainGame;
using UnityEngine;

namespace Game.HideOut
{
    public class UiManager : MonoBehaviour
    {
        public Canvas iconCanvas;
        public GameObject iconObject;
        public Vector3 posCorrection = new Vector3(0f, 25f, 0f);
        public List<IconBase> iconBaseList;
        public CityAreaManager cityAreaManager;

        public GameObject eventButton;
        public GameObject marketButton;
        public GameObject inventoryButton;

        
        private void Awake()
        {
            if (iconCanvas == null) print("Icon Canvas is Null!");
            if (cityAreaManager == null) cityAreaManager = FindObjectOfType<CityAreaManager>();
        }

        private void Start()
        {
            foreach (var cityArea in cityAreaManager.cityAreaList)
            {
                MakeCityAreaIcon(cityArea);
            }
        }

        private void LateUpdate()
        {
            foreach (var iconBase in iconBaseList)
            {
                iconBase.transform.position = Camera.main.WorldToScreenPoint(iconBase.BaseTransform.position + posCorrection);
            }
        }

        void MakeCityAreaIcon(CityArea cityArea)
        {

            if (Camera.main == null) return;

            var posOnCamera = Camera.main.WorldToScreenPoint(cityArea.transform.position + posCorrection);

            var newClone = Instantiate 
            (
                iconObject, posOnCamera, Quaternion.identity, iconCanvas.transform.Find("IconManager")
            );
            var iconBase = newClone.GetComponent<IconBase>();
            iconBaseList.Add(iconBase);
            cityArea.iconBase = iconBase;
            iconBase.BaseTransform = cityArea.transform;

            CheckMarketIcon(cityArea, newClone);
            CheckEventIcon(cityArea, newClone);
        }

        void CheckMarketIcon(CityArea cityArea, GameObject icon)
        {
            icon.transform.Find("MarketIcon").gameObject.SetActive(cityArea.hasMarket);
        }
        
        void CheckEventIcon(CityArea cityArea, GameObject icon)
        {
            icon.transform.Find("EventIcon").gameObject.SetActive(cityArea.hasEvent);
        }
        
    }
    
}
