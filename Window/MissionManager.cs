using System;
using Game.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Window
{
    public class MissionManager : MonoBehaviour
    {
        public GameObject loadingScene;
        public DataManager dataManager;

        private void Awake()
        {
            if (dataManager == null) FindObjectOfType<DataManager>();
            if (dataManager.currentData == null) dataManager.currentData = FindObjectOfType<CurrentData>();
        }

        public void GoToNodeMap()
        {
            if (dataManager.currentData == null) dataManager.currentData = FindObjectOfType<CurrentData>();
            dataManager.currentData.currentMission = dataManager.currentData.missionList[0];
            
            var characters = dataManager.LoadCharacter();
            if (characters == null)
            {
                print("characters was not saved");
            }
            
            loadingScene.SetActive(true);
            SceneManager.LoadScene("Main");
        }
    }
}
