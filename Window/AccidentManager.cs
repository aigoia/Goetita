using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.MainGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Window
{
    public class AccidentManager : MonoBehaviour
    {
        public UiManager uiManager;
        public CityAreaManager cityAreaManager;
        public CharacterManager characterManager;
        public CurrentData currentData;
        public DataManager dataManager;
        public int defaultExpiry = 5;
        public int spendExpiry = 1;
        private readonly int zeroExpiry = 0;
        public int initEventCount = 4;
        public int initMarketCount = 2;
        public int initHospitalCount = 2;
        public int initRecruitCount = 1;
        public int initMission = 2;
        
        
        private void Awake()
        {
            if (uiManager == null) uiManager = FindObjectOfType<UiManager>();
            if (cityAreaManager == null) cityAreaManager = FindObjectOfType<CityAreaManager>();
            if (characterManager == null) characterManager = FindObjectOfType<CharacterManager>();
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
        }

        private void Start()
        {
            dataManager.SettingData();
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

            if (intList == null)
            {
                return;
            }

            if (where == null)
            {
                for (int i = 0; i < accidents.Count; i++)
                {
                    accidents[i].id = intList[i];
                }
            }
            else
            {
                for (int i = 0; i < accidents.Count; i++)
                {
                    if (intList[i] != where.id)
                    {
                        accidents[i].id = intList[i];    
                    }
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

            foreach (var accident in accidents)
            {
                if (accident.mission != null) print(accident.id + " : " + accident.mission.mapName + "(" + accident.mission.difficultLevel + ")");
            }
            
            // accidents.ForEach(print);
            
            SetId(accidents, IntList());
            
            // accidents.ForEach(i => print(i.id));

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

        DifficultLevel ReturnDifficultLevel(int index)
        {
            DifficultLevel newDifficultLiLevel;

            int number = Random.Range(0, dataManager.frequencyList[index].easy + dataManager.frequencyList[index].normal + dataManager.frequencyList[index].hard);

            if (number < dataManager.frequencyList[index].easy)
            {
                newDifficultLiLevel = DifficultLevel.Easy;
            }
            else if (number < dataManager.frequencyList[index].easy + dataManager.frequencyList[index].normal)
            {
                newDifficultLiLevel = DifficultLevel.Normal;
            }
            else if (number < dataManager.frequencyList[index].easy + dataManager.frequencyList[index].normal + dataManager.frequencyList[index].hard)
            {
                newDifficultLiLevel = DifficultLevel.Hard;
            }
            else
            {
                print("Difficult Non");
                newDifficultLiLevel = DifficultLevel.Non;
            }
            
            return newDifficultLiLevel;
        }
        
        Mission ReturnMission()
        {
            Mission newMission;
            // what difficult
            var difficult = DifficultLevel.Non;
            
            // if (dataManager.baseData.turnDate < initMission || dataManager.baseData.initClear == false)
            // {
            //     int index = Random.Range(0, currentData.initMissionList.Count);
            //     return currentData.initMissionList[index];
            // }
            
            if (dataManager.baseData.turnDate < dataManager.frequencyTiming[0])
            {
                difficult = ReturnDifficultLevel(0);
            }
            else if (dataManager.baseData.turnDate < dataManager.frequencyTiming[1])
            {
                difficult = ReturnDifficultLevel(1);
            }
            else if (dataManager.baseData.turnDate < dataManager.frequencyTiming[2])
            {
                difficult = ReturnDifficultLevel(2);
            }
            else if (dataManager.baseData.turnDate < dataManager.frequencyTiming[3])
            {
                difficult = ReturnDifficultLevel(3);
            }
            else if (dataManager.baseData.turnDate < dataManager.frequencyTiming[4])
            {
                difficult = ReturnDifficultLevel(4);
            }
            else if (dataManager.baseData.turnDate < dataManager.frequencyTiming[5])
            {
                difficult = ReturnDifficultLevel(5);
            }
            else
            {
                difficult = DifficultLevel.Non;
            }
            
            // print("Ok");
            
            if (difficult == DifficultLevel.Easy)
            {
                var missionList = new List<Mission>();
                foreach (var mission in currentData.missionList)
                {
                    if (mission.difficultLevel == DifficultLevel.Easy)
                    {
                        missionList.Add(mission);
                    }
                }
                
                newMission = missionList[Random.Range(0, missionList.Count)];
            }
            else if (difficult == DifficultLevel.Normal)
            {
                var missionList = new List<Mission>();
                foreach (var mission in currentData.missionList)
                {
                    if (mission.difficultLevel == DifficultLevel.Normal)
                    {
                        missionList.Add(mission);
                    }
                }
                
                newMission = missionList[Random.Range(0, missionList.Count)];
            }
            else if (difficult == DifficultLevel.Hard)
            {
                var missionList = new List<Mission>();
                foreach (var mission in currentData.missionList)
                {
                    if (mission.difficultLevel == DifficultLevel.Hard)
                    {
                        missionList.Add(mission);
                    }
                }
                
                newMission = missionList[Random.Range(0, missionList.Count)];
            }
            else
            {
                newMission = currentData.missionList[Random.Range(0, currentData.missionList.Count)];
            }

            return newMission;
        }

        Accident NewAccident(AccidentType type)
        {
            var newAccident = new Accident();
            
            if (type == AccidentType.Event)
            {
                
                newAccident = new Accident()
                {
                    expiry = defaultExpiry,
                    accidentType = AccidentType.Event,
                    mission = ReturnMission()
                };

            }
            else if (type == AccidentType.Market)
            {
                newAccident = new Accident()
                {
                    expiry = defaultExpiry,
                    accidentType = AccidentType.Market,
                    
                    // make random Item
                    // marketItem = dataManager.marketManager.defaultBuyList,
                    marketItem = dataManager.marketManager.MakeBuyList()
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
        public Mission mission;
        public List<Item> marketItem;
    }
    
    public enum AccidentType
    {
        Non, Event, Market, Hospital, Recruit,
    }
}
