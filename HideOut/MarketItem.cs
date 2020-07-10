using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.HideOut
{
    public class MarketItem : MonoBehaviour
    {

        public List<Transform> marketItemList;
        private readonly Action<Transform> _hideAllItem = (i) => i.gameObject.SetActive(false);

        private void Awake()
        {
            marketItemList = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                marketItemList.Add(transform.GetChild(i));
            }

            marketItemList.ForEach(_hideAllItem);
        }
    }


}
