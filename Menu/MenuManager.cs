using System;
using UnityEngine;

namespace Game.Menu
{
    public class MenuManager : MonoBehaviour
    {
        private AudioManager _audioManager;
        public Options options;
       

        private void Awake()
        {
            if (_audioManager == null) _audioManager = FindObjectOfType<AudioManager>();
            if (options == null) options = FindObjectOfType<Options>();
        }

        void Start()
        {
            if (_audioManager != null) _audioManager.PlaySound();
            
            options.InitUi();
            options.InitSetting();
        }

    }
}
