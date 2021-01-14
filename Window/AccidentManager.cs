using System;
using System.Collections.Generic;
using Game.MainGame;
using UnityEngine;

namespace Game.Window
{
    public class AccidentManager : MonoBehaviour
    {
        public UiManager uiManager;
        public CityAreaManager cityAreaManager;
        public CharacterManager characterManager;
        public int defaultExpiry = 5;
        public int spendExpiry = 1;
        private readonly int zeroExpiry = 0;
        public int initEventCount = 4;
        public int initMarketCount = 2;
        public int initHospitalCount = 3;
        public int initRecruitCount = 1;

        private void Awake()
        {
            if (uiManager == null) uiManager = FindObjectOfType<UiManager>();
            if (cityAreaManager == null) cityAreaManager = FindObjectOfType<CityAreaManager>();
            if (characterManager == null) characterManager = FindObjectOfType<CharacterManager>();
        }

        private void Start()
        {
            InputAccident(initEventCount, initMarketCount, initHospitalCount, initRecruitCount);
        }

        public void SpendAccident()
        {
            var originCityArea = characterManager.WhereIam();
            
            if (originCityArea == null) return;
            if (originCityArea.assignedAccident == null) return;

            if (originCityArea.assignedAccident.accidentType == AccidentType.Event)
            {
                uiManager.eventButton.SetActive(false);
            }
            else if (originCityArea.assignedAccident.accidentType == AccidentType.Market) 
            {
                uiManager.marketButton.SetActive(false);
            }
            else if (originCityArea.assignedAccident.accidentType == AccidentType.Hospital)
            {
                uiManager.hospitalButton.SetActive(false);
            }
            else if (originCityArea.assignedAccident.accidentType == AccidentType.Recruit)
            {
                uiManager.recruitButton.SetActive(false);
            }
            
            originCityArea.assignedAccident = null;
            uiManager.RenewalIcon();
        }
        
        public void SpendAllExpiry(List<CityArea> cityAreas)
        {
            foreach (var cityArea in cityAreas)
            {
                var assignedAccident = cityArea.assignedAccident;
                
                if (assignedAccident == null) continue;

                if (assignedAccident.expiry > zeroExpiry)
                {
                    assignedAccident.expiry = assignedAccident.expiry - spendExpiry;
                }
                else
                {
                    cityArea.assignedAccident = null;
                }
                
            }
            
            uiManager.RenewalIcon();
        }

        void SetId(List<Accident> accidents, List<int> intList)
        {
            var where = characterManager.WhereForm(cityAreaManager.dataManager.GetPosition());

            if (accidents.Count > intList.Count)
            {
                intList = GameUtility.RandomExtraction(cityAreaManager.cityAreaList.Count, accidents.Count);
            }

            for (int i = 0; i < accidents.Count; i++)
            {
                if (intList[i] != where.id)
                {
                    accidents[i].id = intList[i];    
                }
            }
        }

        List<int> IntList()
        {
            var exceptList = cityAreaManager.cityAreaList.FindAll(area => area.assignedAccident == null);
            var intList = new List<int>();

            foreach (var area in exceptList)
            {
                intList.Add(area.id);
            }
            
            GameUtility.ShuffleList(intList);
            
            return intList;
        }

        public void InputAccident(int eventCount = 0, int marketCount = 0, int hospitalCount = 0, int recruitCount = 0)
        {
            var accidents = MakeAccident(eventCount, marketCount,  hospitalCount, recruitCount);
            
            SetId(accidents, IntList());

            foreach (var accident in accidents)
            {
                var theCityArea = cityAreaManager.cityAreaList.Find(cityArea => cityArea.id == accident.id);
                if (theCityArea == null) continue;
                theCityArea.assignedAccident = accident;
            }
        }

        List<Accident> MakeAccident(int eventCount = 0, int marketCount = 0, int hospitalCount = 0, int recruitCount = 0)
        {
            var newAccidents = new List<Accident>();

            // print(eventCount + ", "+ marketCount);
            
            for (int i = 0; i < eventCount; i++)
            {
                newAccidents.Add(NewAccident(AccidentType.Event));
            }
            for (int i = 0; i < marketCount; i++)
            {
                newAccidents.Add(NewAccident(AccidentType.Market));
            }
            for (int i = 0; i < hospitalCount; i++)
            {
                newAccidents.Add(NewAccident(AccidentType.Hospital));
            }
            for (int i = 0; i < recruitCount; i++)
            {
                newAccidents.Add(NewAccident(AccidentType.Recruit));
            }

            return newAccidents;
        }

        Accident NewAccident(AccidentType type)
        {
            var newAccident = new Accident();

            if (type == AccidentType.Event)
            {
                newAccident = new Accident()
                {
                    expiry = defaultExpiry,
                    accidentType = AccidentType.Event
                };
            }
            else if (type == AccidentType.Market)
            {
                newAccident = new Accident()
                {
                    expiry = defaultExpiry,
                    accidentType = AccidentType.Market
                };
            }
            else if (type == AccidentType.Hospital)
            {
                newAccident = new Accident()
                {
                    expiry = defaultExpiry,
                    accidentType = AccidentType.Hospital
                };
            }
            else if (type == AccidentType.Recruit)
            {
                newAccident = new Accident()
                {
                    expiry = defaultExpiry,
                    accidentType = AccidentType.Recruit
                };
            }

            return newAccident;
        }
    }

    [Serializable]
    public class Accident
    {
        public AccidentType accidentType = AccidentType.Non;
        public int id = 0;
        public int expiry = 0;
    }
    
    public enum AccidentType
    {
        Non, Event, Market, Hospital, Recruit,
    }
}
