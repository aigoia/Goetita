using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Game.Menu
{
    public class AudioManager : MonoBehaviour
    {
        public List<AudioClip> soundList = new List<AudioClip>();
        private AudioSource _source;

        public static AudioManager Instance;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            PlayNextSong();
        }

        void PlayNextSong(){
            _source.clip = soundList[Random.Range(0, soundList.Count)];
            _source.Play();
            Invoke(nameof(PlayNextSong), _source.clip.length);
        }
    }
}