using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Window
{
    public class MissionManager : MonoBehaviour
    {
        public GameObject loadingScene;
        
        public void GoToNodeMap()
        {
            loadingScene.SetActive(true);
            SceneManager.LoadScene("Main");
        }
    }
}
