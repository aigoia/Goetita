using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Game.MainGame {
    
    public class MotionManager : MonoBehaviour {
        
        public GameManager gameManager;

        // Base Data
        public WaitForSeconds wait = new WaitForSeconds (0.5f);
        public float swordEndTime = 0.9f;
        // public float swordEndTimeCard = 0.75f; 
        public float swordEndTimeEnemy = 0.6f;
        public float swordHit = 0.4f;
        public float swordHitPowerFull = 0.5f;
        public float swordHitCard = 0.3f;
        
        public float swordHitEnemy = 0.3f;
        public float swordMin = 0.15f;
        public float swordMax = 0.3f;
        
        public float rangeEndTime = 0.9f;
        public float rangeHit = 0.55f;
        public float rangeHitShadow = 0.55f;
        public float rangeHitCard = 0.55f;
        
        public float rangeMin = 0.15f;
        public float rangeMax = 0.3f;
        
        
        public float swordIn = 0.5f;
        public float swordOut = 0.4f;
        public float rangeIn = 0.6f;
        public float rangeOut = 0.7f;
        
        public float swordThrowIn = 0.6f;
        public float swordThrowReady = 0.85f;
        public float swordThrowOut = 0.6f;
        public float swordThrowFly = 0.25f;
        public float rangeThrowIn = 0.6f;
        public float rangeThrowReady = 0.85f;
        public float rangeThrowOut = 0.6f;
        public float rangeThrowFly = 0.25f;

        
        
        // newMotions
        public AnimationClip swordBase;
        public AnimationClip swordCard;
        public AnimationClip swordPowerFull;
        public AnimationClip rangeBase;
        public AnimationClip rangeCard;
        public AnimationClip rangeShadow;

        private void Awake () 
        {
            if (gameManager == null) gameManager = FindObjectOfType<GameManager> ();
        }
    }
}