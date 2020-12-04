using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
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
        public GameObject hospitalButton;
        public GameObject recruitButton;
        
        public int marginalExpiry = 1;
        public Color disappearColor;
        public Color normalColor;
        
        public float third;
        public float second;
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
                if (Camera.main != null)
                    iconBase.transform.position =
                        Camera.main.WorldToScreenPoint(iconBase.baseTransform.position + posCorrection);
            }
        }
        
        public void ButtonOff()
        {
            eventButton.SetActive(false);
            marketButton.SetActive(false);
            hospitalButton.SetActive(false);
            inventoryButton.SetActive(false);
            recruitButton.SetActive(false);
        }

        public void ButtonOn(CityArea cityArea)
        {
            if (cityArea.assignedAccident == null)
            {
                eventButton.SetActive(false);
                hospitalButton.SetActive(false);
                recruitButton.SetActive(false);
                inventoryButton.SetActive(true);
                return;
            }
            if (cityArea.assignedAccident.accidentType == AccidentType.Event)
            {
                eventButton.SetActive(true);    
            }
            if (cityArea.assignedAccident.accidentType == AccidentType.Market)
            {
                marketButton.SetActive(true);
            }
            if (cityArea.assignedAccident.accidentType == AccidentType.Hospital)
            {
                hospitalButton.SetActive(true);
            }
            if (cityArea.assignedAccident.accidentType == AccidentType.Recruit)
            {
                recruitButton.SetActive(true);
            }

            inventoryButton.SetActive(true);
        }

        void AllIconOff(IconBase icon)
        {
            icon.transform.Find("MarketIcon").gameObject.SetActive(false);
            icon.transform.Find("EventIcon").gameObject.SetActive(false);
            icon.transform.Find("HospitalIcon").gameObject.SetActive(false);
            icon.transform.Find("RecruitIcon").gameObject.SetActive(false);
        }

        public void RenewalIcon()
        {
            foreach (var iconBase in iconBaseList)
            {
                CheckIcons(iconBase.baseTransform.GetComponent<CityArea>(), iconBase);
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
            iconBase.baseTransform = cityArea.transform;

            CheckIcons(cityArea, iconBase);
        }

        void CheckIcons(CityArea cityArea, IconBase icon)
        {
            CheckIcon(cityArea, icon, "MarketIcon", AccidentType.Market);
            CheckIcon(cityArea, icon, "EventIcon", AccidentType.Event);
            CheckIcon(cityArea, icon, "HospitalIcon", AccidentType.Hospital);
            CheckIcon(cityArea, icon, "RecruitIcon", AccidentType.Recruit);
        }

        void CheckIcon(CityArea cityArea, IconBase icon, string name, AccidentType accidentType)
        {
            if (cityArea.assignedAccident == null)
            {
                icon.transform.Find(name).gameObject.SetActive(false);
                return;
            }

            if (cityArea.assignedAccident.accidentType == accidentType)
            {
                AllIconOff(icon);
                icon.transform.Find(name).gameObject.SetActive(true);
                icon.transform.Find(name).transform.Find("Boundary").GetComponent<Image>().color = cityArea.assignedAccident.expiry < marginalExpiry ? disappearColor : normalColor;
            }
        }
    }
}
