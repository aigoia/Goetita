using System;
using System.Collections.Generic;
using Game.MainGame;
using UnityEngine;

namespace Game.HideOut
{
    public class UiManager : MonoBehaviour
    {
        private Canvas _canvas;
        public GameObject iconObject;
        readonly Vector3 posCorrection = new Vector3(0f, 0f, 0f);
        public List<GameObject> iconList;

        private void Awake()
        {
            if (_canvas == null) _canvas = FindObjectOfType<Canvas>();
        }

        [ContextMenu("Make CityAreaIcon")]
        void MakeCityAreaIcon(CityArea cityArea)
        {
            if (Camera.main == null) return;
			
            var newClone = Instantiate 
            (
                iconObject,
                Camera.main.WorldToScreenPoint(cityArea.transform.position + posCorrection),
                Quaternion.identity, _canvas.transform.Find("IconManager")
            );
            
            iconList.Add(newClone);
        }
    }
}
