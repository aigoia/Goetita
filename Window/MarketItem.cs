using System;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    public class MarketItem : MonoBehaviour
    {
        public DataManager dataManager;
        public List<Transform> marketItemList;
        private readonly Action<Transform> _hideAllItem = (i) => i.gameObject.SetActive(false);

        private void Awake()
        {
            if (dataManager == null) dataManager = FindObjectOfType<DataManager>();
            marketItemList.ForEach(_hideAllItem);
        }

        [ContextMenu("Setting")]
        public void MarketItemSet()
        {
            marketItemList = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                marketItemList.Add(transform.GetChild(i));
            }
        }

        [ContextMenu("SetBaseIcon")]
        public void SetBaseIcon()
        {
            foreach (var item in marketItemList)
            {
                item.GetComponent<ItemButtonManager>().icon.GetComponent<Image>().sprite =
                    dataManager.FindItemImage("Base").sprite;
            }
        }

        public void OffAll()
        {
            foreach (var sellIcon in marketItemList)
            {
                sellIcon.gameObject.SetActive(false);
            }
        }
    }
}
