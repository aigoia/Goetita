using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.HideOut
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
