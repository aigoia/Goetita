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

        public int marginalExpiry = 1;
        public Color disappearColor;
        public Color normalColor;
        
        public float third;
        public float Second;
        public float first;
        
        
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

        public void RenewalIcon()
        {
            foreach (var iconBase in iconBaseList)
            {
                CheckColor(iconBase.BaseTransform.GetComponent<CityArea>(), iconBase);
                CheckEventIcon(iconBase.BaseTransform.GetComponent<CityArea>(), iconBase);
                CheckMarketIcon(iconBase.BaseTransform.GetComponent<CityArea>(), iconBase);
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

            CheckIcon(cityArea, iconBase);
        }

        void CheckIcon(CityArea cityArea, IconBase icon)
        {
            CheckColor(cityArea, icon);
            CheckMarketIcon(cityArea, icon);
            CheckEventIcon(cityArea, icon);
        }

        void CheckMarketIcon(CityArea cityArea, IconBase icon)
        {
            if (cityArea.assignedAccident == null)
            {
                icon.transform.Find("MarketIcon").gameObject.SetActive(false);
                return;
            }

            if (cityArea.assignedAccident.accidentType == AccidentType.Market)
            {
                icon.transform.Find("MarketIcon").gameObject.SetActive(true);    
            }
            
        }
        
        void CheckEventIcon(CityArea cityArea, IconBase icon)
        {
            if (cityArea.assignedAccident == null)
            {
                icon.transform.Find("EventIcon").gameObject.SetActive(false);
                return;
            }

            if (cityArea.assignedAccident.accidentType == AccidentType.Event)
            {
                icon.transform.Find("EventIcon").gameObject.SetActive(true);
            }
        }

        void CheckColor(CityArea cityArea, IconBase icon)
        {
            if (cityArea.assignedAccident == null || cityArea.assignedAccident.expiry >= marginalExpiry)
            {   
                // if (cityArea.assignedAccident != null) print(cityArea.assignedAccident.expiry);
                
                icon.marketBoundary.color = normalColor;
                icon.EventBoundary.color = normalColor;
                return;
            }

            if (cityArea.assignedAccident.expiry < marginalExpiry)
            {
                // print(cityArea.assignedAccident.expiry);
                icon.EventBoundary.color = disappearColor;
                icon.marketBoundary.color = disappearColor;
            }
        }
    }
}
