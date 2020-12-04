using UnityEngine;

namespace Game.MainGame
{
	public class EffectManager : MonoBehaviour {

		public GameObject hitParticles;
		public GameObject hitLine;
		public GameObject hitBlood;
		public Vector3 particlePos = new Vector3 (0f, 1.5f, 0f);
		public float particleRotation;
		public Vector3 hitLineRotation = new Vector3(0, 0, 0);


		public void SwordEffectEnemy(Player player, Enemy enemy)
		{
			// effects
			if (hitParticles == null) return;
			if (hitLine == null) return;
			if (hitBlood == null) return;
			
			var targetTransform = player.transform;
			var position = targetTransform.position; 
			Instantiate(hitLine, position +  particlePos, Quaternion.Euler(hitLineRotation));
			Instantiate(hitParticles, position + particlePos, Quaternion.Euler(0, 0, particleRotation));
			Instantiate(hitBlood, position + particlePos, targetTransform.localRotation, targetTransform);
		}

		public void RangeEffectEnemy(Player player, Enemy enemy)
		{
			// effects
			if (hitParticles == null) return;
			if (hitLine == null) return;
			if (hitBlood == null) return;
            			
			var targetTransform = player.transform;
			var position = targetTransform.position;
			Instantiate(hitBlood, position + particlePos, targetTransform.localRotation, targetTransform);
		}
		
		public void SwordEffectPlayer(Enemy enemy, Player player)
		{
			// effects
			if (hitParticles == null) return;
			if (hitLine == null) return;
			if (hitBlood == null) return;
			
			var targetTransform = enemy.transform;
			var position = targetTransform.position; 
			Instantiate(hitLine, position +  particlePos, Quaternion.Euler(hitLineRotation));
			Instantiate(hitParticles, position + particlePos, Quaternion.Euler(0, 0, particleRotation));
			Instantiate(hitBlood, position + particlePos, targetTransform.localRotation, targetTransform);
		}
        
		public void RangeEffectPlayer(Enemy enemy, Player player)
		{
			if (hitParticles == null) return;
			if (hitLine == null) return;
			if (hitBlood == null) return;
			
			var targetTransform = enemy.transform;
			var position = targetTransform.position;
			Instantiate(hitBlood, position + particlePos, targetTransform.localRotation, targetTransform);
		}
	}
}
