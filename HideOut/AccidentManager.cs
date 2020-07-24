using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.MainGame;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.HideOut
{
    public class AccidentManager : MonoBehaviour
    {
        public List<Accident> initAccidentList = new List<Accident>();
        public Accident currentAccident;
        public UiManager uiManager;
        public CityAreaManager cityAreaManager;
        public CharacterManager characterManager;
        
        // public Transform EventButton;
        // public Transform MarketButton;
        // public Transform inventoryButton;

        public CityArea selectedCityArea;
        public GameObject selectedIcon;

        private void Awake()
        {
            if (uiManager == null) uiManager = FindObjectOfType<UiManager>();
            if (cityAreaManager == null) cityAreaManager = FindObjectOfType<CityAreaManager>();
            if (characterManager == null) characterManager = FindObjectOfType<CharacterManager>();
        }

        private void Start()
        {
            InitAccident();
            var testList = GameUtility.RandomExtraction(cityAreaManager.cityAreaList.Count, initAccidentList.Count);
            SetId(testList);
            InputCityArea();
        }

        public void SpendAccident()
        {
            var originCityArea = characterManager.WhereIam();
            
            if (originCityArea == null) return;
            if (originCityArea.assignedAccident == null) return;

            selectedCityArea = originCityArea;
            
            if (originCityArea.assignedAccident.accidentType == AccidentType.Event)
            {
                print(originCityArea.name + " : Event");
            }
            else if (originCityArea.assignedAccident.accidentType == AccidentType.Market) 
            {
                print(originCityArea.name + " : Market");
            }
        }

        public void removeSelectedAccident()
        {
            if (selectedCityArea == null) return;
            if (selectedIcon == null) return;
            
            selectedCityArea.assignedAccident = null;
            selectedIcon = null;
            uiManager.RenewalIcon();
        }

        public void SpendAllExpiry(List<CityArea> cityAreas)
        {
            foreach (var cityArea in cityAreas)
            {
                var assignedAccident = cityArea.assignedAccident;
                
                if (assignedAccident == null) continue;

                if (assignedAccident.expiry > 0)
                {
                    assignedAccident.expiry = assignedAccident.expiry - 1;
                    
                }
                else
                {
                    cityArea.assignedAccident = null;
                }
                
                // print(assignedAccident.expiry);
            }
            
            uiManager.RenewalIcon();
        }

        void SetId(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                initAccidentList[i].id = list[i];
            }
        }

        void InputCityArea()
        {
            foreach (var accident in initAccidentList)
            {
                var theCityArea = cityAreaManager.cityAreaList.Find(cityArea => cityArea.id == accident.id);
                if (theCityArea == null) continue;
                theCityArea.assignedAccident = accident;
            }
        }
        
        void InitAccident()
        {
            initAccidentList.Add(new Accident()
            {
                expiry = 5,
                accidentType = AccidentType.Event
            });
            initAccidentList.Add(new Accident()
            {
                expiry = 5,
                accidentType = AccidentType.Event
            });
            initAccidentList.Add(new Accident()
            {
                expiry = 5,
                accidentType = AccidentType.Event
            });
            initAccidentList.Add(new Accident()
            {
                expiry = 5,
                accidentType = AccidentType.Event
            });
            initAccidentList.Add(new Accident()
            {
                expiry = 5,
                accidentType = AccidentType.Market
            });
        }
    }

    public class Accident
    {
        public AccidentType accidentType = AccidentType.Non;
        public int id = 0;
        public int expiry = 0;
    }
    
    public enum AccidentType
    {
        Non, Event, Market,
    }
}
