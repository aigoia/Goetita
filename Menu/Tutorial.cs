using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu
{
    public class Tutorial : MonoBehaviour
    {
        public List<GameObject> objectList;
        public Button button;
        public int index = 0;

        public void ShowProjects()
        {
            index = index + 1;
            index = index % objectList.Count; 

            foreach (var item in objectList)
            {
                item.SetActive(false);
            }
            
            objectList[index].SetActive(true);
            
        }
    }
}
