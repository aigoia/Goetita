using System;
using UnityEngine;

namespace Game.MainGame
{
	public enum Sound
	{
		Sword, Gun, EnergyGun,
	}
	
	public class SoundManager : MonoBehaviour
	{
		private static AudioClip _sword, _gun, _energyGun;
		private static AudioSource _audioSource;

		private void Awake()
		{
			_sword = Resources.Load<AudioClip>("Sword");
			_gun = Resources.Load<AudioClip>("Gun");
			_energyGun = Resources.Load<AudioClip>("EnergyGun");
			
			_audioSource = GetComponent<AudioSource>();
		}

		public void PlaySound(Sound sound)
		{
			if (sound == MainGame.Sound.Sword)
			{
				_audioSource.PlayOneShot(_sword);
			}
			else if (sound == MainGame.Sound.Gun)
			{
				_audioSource.PlayOneShot(_gun);
			}
			else if (sound == MainGame.Sound.EnergyGun)
			{
				_audioSource.PlayOneShot(_energyGun);
			}
		}
	}
}
