using UnityEngine;

namespace Game.Data
{
    public enum MapType
    {
        Test, Basic,
    }

    public enum DifficultLevel
    {
        Non, Easy, Normal, Hard,
    }
    
    public class Map : MonoBehaviour
    {
        public string mapName;
        public Vector2 mapSize;
        public MapType mapType;
        public DifficultLevel difficultLevel;
        public int goldReward;
        public int expReward;

        public Map(string mapName, DifficultLevel difficultLevel, Vector2 mapSize, MapType mapType, int goldReward, int expReward)
        {
            this.mapName = mapName;
            this.difficultLevel = difficultLevel;
            this.mapSize = mapSize;
            this.mapType = mapType;
            this.goldReward = goldReward;
            this.expReward = expReward;
        }
    }
}
