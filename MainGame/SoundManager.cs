using Game.Menu;
using UnityEngine;

namespace Game.MainGame
{
	public enum Sound
	{
		Sword, Gun, EnergyGun, Impact, OneShot,
	}

	public enum SoundSystem
	{
		Hover, Click, Open,
	}
	
	public class SoundManager : MonoBehaviour
	{
		public AudioManager audioManager;
		
		[Header("Sound")]
		public AudioClip sword;
		public AudioClip impact;
		public AudioClip gun;
		public AudioClip gunOneShot;
		public AudioClip energyGun;
		private static AudioSource _audioSource;

		[Header("System")] 
		public AudioClip hover;
		public AudioClip click;
		public AudioClip open;
		

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
			if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
		}
		
		void Start()
		{
			if (audioManager != null) audioManager.PlaySound();
		}

		public void PlaySound(Sound sound)
		{
			if (sound == Sound.Sword)
			{
				_audioSource.PlayOneShot(sword);
			}

			if (sound == Sound.Impact)
			{
				_audioSource.PlayOneShot(impact);
			}
			else if (sound == Sound.Gun)
			{
				_audioSource.PlayOneShot(gun);
			}
			else if (sound == Sound.OneShot)
			{
				_audioSource.PlayOneShot(gunOneShot);
			}
			else if (sound == Sound.EnergyGun)
			{
				_audioSource.PlayOneShot(energyGun);
			}
		}

		// public void PlaySystem(SoundSystem sound)
		// {
		// 	if (sound == SoundSystem.Hover)
		// 	{
		// 		_audioSource.PlayOneShot(hover);
		// 	}
		// 	else if (sound == SoundSystem.Click)
		// 	{
		// 		_audioSource.PlayOneShot(click);
		// 	}
		// 	else if (sound == SoundSystem.Open)
		// 	{
		// 		_audioSource.PlayOneShot(open);
		// 	}
		// }
	}
}
