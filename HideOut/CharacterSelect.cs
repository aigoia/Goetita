using System.Collections.Generic;
using UnityEngine;

namespace Game.HideOut
{
    public class CharacterSelect : MonoBehaviour {

        public List<GameObject> activeList = new List<GameObject>();
        public List<GameObject> buttonList = new List<GameObject>();

        public void ReSetting()
        {
            activeList.ForEach(i => i.SetActive(false));
        }
    }
}
