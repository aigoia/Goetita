using Game.Data;
using UnityEngine;

namespace Game.Window
{
    public class Chapter : MonoBehaviour
    {
        public MissionManager missionManager;
        public DataManager dataManager;
        
        private void Awake()
        {
            if (dataManager == null) FindObjectOfType<DataManager>();
            if (missionManager == null) FindObjectOfType<MissionManager>();
        }

    }
}
