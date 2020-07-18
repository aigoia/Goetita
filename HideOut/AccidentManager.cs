using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.MainGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.HideOut
{
    public class AccidentManager : MonoBehaviour
    {
        List<Accident> accidentList = new List<Accident>();
        public Accident currentAccident;
        public UiManager uiManager;
        public CityAreaManager cityAreaManager;
        
        private void Awake()
        {
            if (uiManager == null) uiManager = FindObjectOfType<UiManager>();
            if (cityAreaManager == null) cityAreaManager = FindObjectOfType<CityAreaManager>();
        }

        private void Start()
        {
            testAccident();

            var testList = GameUtility.RandomExtraction(25, 5);

            foreach (var i in testList)
            {
                print(i);
            }
        }
        

        void testAccident()
        {
            accidentList.Add(new Accident());
            accidentList.Add(new Accident());
            accidentList.Add(new Accident());
            accidentList.Add(new Accident());
            accidentList.Add(new Accident());
        }
    }

    public class Accident
    {
        public bool hasEvent;
        public bool hasMarket;
        public int id;
    }
}
