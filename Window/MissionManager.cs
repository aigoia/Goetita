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
        }

        public void GoToNodeMap()
        {
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
