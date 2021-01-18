using UnityEngine;

namespace Game.Data
{
    public enum MapType
    {
        Test,
    }
    
    
    public class Map : MonoBehaviour
    {
        public string mapName;
        public MapType mapType;
        public int goldReward;
        public int expReward;

        public Map(string mapName, MapType mapType, int goldReward, int expReward)
        {
            this.mapName = mapName;
            this.mapType = mapType;
            this.goldReward = goldReward;
            this.expReward = expReward;
        }
    }
}
