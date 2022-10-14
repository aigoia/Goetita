using UnityEngine;

namespace Game.Window
{
    public class Information : MonoBehaviour
    {
        public bool menuOn = false;
        public GameObject gameMenu;
        public GameObject inventory;
        public GameObject mission;
        public GameObject market;
        public GameObject hospital;
        public GameObject recruit;
        public GameObject levelUp;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (inventory.activeSelf) return;
                if (mission.activeSelf) return;
                if (market.activeSelf) return;
                if (hospital.activeSelf) return;
                if (recruit.activeSelf) return;
                if (levelUp.activeSelf) return;

                if (menuOn == false)
                {
                    gameMenu.SetActive(true);
                    menuOn = true;
                }
                else
                {
                    gameMenu.SetActive(false);
                    menuOn = false;
                }    
            }
            
        }
    }
}
