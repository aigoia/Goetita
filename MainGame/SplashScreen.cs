using System;
using UnityEngine;

namespace Game.MainGame
{
    public class SplashScreen : MonoBehaviour {
        
        [Header("RESOURCES")]
        public GameObject splashScreen;
        private Animator _splashScreenAnimator;
        public float removeTime = 0.8f;

        [Header("SETTINGS")]
        public bool disableSplashScreen;

        private void Awake()
        {
            if (splashScreen == null) splashScreen = transform.Find("SplashScreen").gameObject;
        }

        void Start()
        {
            if (disableSplashScreen)
            {
                splashScreen.SetActive(true);
                _splashScreenAnimator = splashScreen.GetComponent<Animator>();
                _splashScreenAnimator.Play("Splash Out");
            }

            else
            {
                splashScreen.SetActive(true);

            }
            
            Invoke("RemoveSplashScreen", removeTime);
        }

        void RemoveSplashScreen()
        {
            Destroy(splashScreen);
        }
        
    }
}
