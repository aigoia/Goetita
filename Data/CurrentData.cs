using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Data
{
    public class CurrentData : MonoBehaviour
    {
        public static CurrentData Instance;

        public List<Mission> missionList;
        public List<Mission> initMissionList;
        public Mission currentMission;
        public int itemRewardPercent = 2;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            MakeMission();
            MakeInitMission();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            // GetMissions();
            // GetInitMissions();
        }

        [ContextMenu("Make Mission")]
        void MakeMission()
        {
            missionList = new List<Mission>()
            {
                new Mission("TestMap", "Test", DifficultLevel.Hard, MapType.Test, 2100, 45),
                new Mission("KeyMap", "Key", DifficultLevel.Hard, MapType.Test, 2100, 45),
                
                new Mission("SignMap", "Sign", DifficultLevel.Hard, MapType.Basic, 2600, 50),
                new Mission("BackMap", "Back", DifficultLevel.Hard, MapType.Basic, 2600, 50),
                
                new Mission("HouseMap", "House", DifficultLevel.Hard, MapType.Test, 3100, 55),
                new Mission("DreamMap", "Dream", DifficultLevel.Hard, MapType.Basic, 3100, 55),
                
            
                new Mission("RunMap", "Run", DifficultLevel.Easy, MapType.Basic, 400, 12),
                new Mission("WalkMap", "Walk", DifficultLevel.Easy, MapType.Basic, 400, 12),
                new Mission("VisionMap", "Vision", DifficultLevel.Easy, MapType.Basic, 400, 12),
                new Mission("CrossMap", "Cross", DifficultLevel.Easy, MapType.Basic, 400, 12),
                new Mission("UnityMap", "Unity", DifficultLevel.Easy, MapType.Basic, 400, 12),
                new Mission("ReplayMap", "Replay", DifficultLevel.Easy, MapType.Basic, 400, 12),
                
                new Mission("GetOverItMap", "Get over it", DifficultLevel.Easy, MapType.Basic, 300, 9),
                new Mission("BeginnerMap", "Beginner", DifficultLevel.Easy, MapType.Basic, 300, 9),
                
                new Mission("CodeMap", "Code", DifficultLevel.Easy, MapType.Basic, 500, 15),
                new Mission("UnrealMap", "Unreal", DifficultLevel.Easy, MapType.Basic, 500, 15),
                new Mission("WorkHardMap", "Work hard", DifficultLevel.Easy, MapType.Basic, 500, 15),
                new Mission("EasyGuyMap", "Easy guy", DifficultLevel.Easy, MapType.Basic, 500, 15),
                
                new Mission("FishMap", "Fish", DifficultLevel.Easy, MapType.Basic, 600, 18),
                new Mission("SimulationMap", "Simulation", DifficultLevel.Easy, MapType.Basic, 600, 18),
                
                new Mission("ArcMap", "Arc age", DifficultLevel.Normal, MapType.Basic, 900, 27),
                new Mission("AngryMap", "Angry", DifficultLevel.Normal, MapType.Basic, 900, 27),
                new Mission("GodotMap", "Godot", DifficultLevel.Normal, MapType.Basic, 900, 27),
                new Mission("MakerMap", "Maker", DifficultLevel.Normal, MapType.Basic, 900, 27),
                
                new Mission("DungeonMap", "Dungeon age", DifficultLevel.Normal, MapType.Basic, 1100, 30),
                new Mission("WidthMap", "Width", DifficultLevel.Normal, MapType.Basic, 1100, 30),
                
                new Mission("LongLineMap", "Long line", DifficultLevel.Normal, MapType.Basic, 1300, 33),
                new Mission("GoodByeMap", "Good bye", DifficultLevel.Normal, MapType.Basic, 1300, 33),
            };
            
            SetMissions(missionList);
        }
        
        [ContextMenu("Make InitMission")]
        void MakeInitMission()
        {
            initMissionList = new List<Mission>()
            {
                // new Mission("OtherSizeMap", "Other side", DifficultLevel.Easy, MapType.Basic, 400, 12),
                // new Mission("CrossMap", "Cross", DifficultLevel.Easy, MapType.Basic, 400, 12),
                new Mission("GetOverItMap", "Get over it", DifficultLevel.Easy, MapType.Basic, 300, 9),
                new Mission("BeginnerMap", "Beginner", DifficultLevel.Easy, MapType.Basic, 300, 9),
            };
            
            SetInitMissions(initMissionList);
        }
        
        public List<Mission> GetMissions()
        {
        	string path = Path.Combine(Application.dataPath + "/StreamingAssets/MissionList.Json");
        	string jsonData = File.ReadAllText(path);
        	var newMission = JsonUtility.FromJson<MissionList>(jsonData);
        	return newMission.Missions.ToList();
        }
        
        void SetMissions(List<Mission> missions)
        {
            var newMissionList = new MissionList() {Missions = missions.ToArray()};
            string jsonData = JsonUtility.ToJson(newMissionList, true);
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/MissionList.Json");
            File.WriteAllText(path, jsonData);
        }
        
        public List<Mission> GetInitMissions()
        {
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/InitMissionList.Json");
            string jsonData = File.ReadAllText(path);
            var newMission = JsonUtility.FromJson<MissionList>(jsonData);
            return newMission.Missions.ToList();
        }
        
        void SetInitMissions(List<Mission> missions)
        {
            var newMissionList = new MissionList() {Missions = missions.ToArray()};
            string jsonData = JsonUtility.ToJson(newMissionList, true);
            string path = Path.Combine(Application.dataPath + "/StreamingAssets/InitMissionList.Json");
            File.WriteAllText(path, jsonData);
        }
    }

    public class MissionList
    { 
        public Mission[] Missions;
    }
    
    [System.Serializable]
    public class Mission
    {
        public string mapName;
        public string missionName;
        public MapType mapType;
        public DifficultLevel difficultLevel;
        public int goldReward;
        public int expReward;

        public Mission(string mapName, string missionName, DifficultLevel difficultLevel, MapType mapType, int goldReward, int expReward)
        {
            this.mapName = mapName;
            this.difficultLevel = difficultLevel;
            this.missionName = missionName;
            this.mapType = mapType;
            this.goldReward = goldReward;
            this.expReward = expReward;
        }
    }
}
