using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.MainGame
{
	public enum Sound
	{
		Sword, Gun, EnergyGun,
	}

	public enum SoundUi
	{
		Hover, Click
	}
	
	public class SoundManager : MonoBehaviour
	{
		[Header("Sound")]
		public AudioClip sword;
		public AudioClip gun;
		public AudioClip energyGun;
		private static AudioSource _audioSource;

		[Header("UI")] 
		public AudioClip hover;
		public AudioClip click;
		

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		public void PlaySound(Sound sound)
		{
			if (sound == Sound.Sword)
			{
				_audioSource.PlayOneShot(sword);
			}
			else if (sound == Sound.Gun)
			{
				_audioSource.PlayOneShot(gun);
			}
			else if (sound == Sound.EnergyGun)
			{
				_audioSource.PlayOneShot(energyGun);
			}
		}

		public void PlayUi(SoundUi sound)
		{
			if (sound == SoundUi.Hover)
			{
				_audioSource.PlayOneShot(hover);
			}
			else if (sound == SoundUi.Click)
			{
				_audioSource.PlayOneShot(click);
			}
		}
	}
}
