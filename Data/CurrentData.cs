using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class CurrentData : MonoBehaviour
    {
        public static CurrentData Instance;

        public List<Mission> missionList;
        public Mission currentMission;

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
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            MakeMission();;
        }

        void MakeMission()
        {
            missionList = new List<Mission>()
            {
                new Mission("TestMap", MapType.Test, 600, 9),
            };
        }
    }
    
    [System.Serializable]
    public class Mission
    {
        public string mapName;
        public MapType mapType;
        public int goldReward;
        public int expReward;

        public Mission(string mapName, MapType mapType, int goldReward, int expReward)
        {
            this.mapName = mapName;
            this.mapType = mapType;
            this.goldReward = goldReward;
            this.expReward = expReward;
        }
    }
}
