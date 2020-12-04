using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.MainGame
{
    public class GameUtility : MonoBehaviour {

        public static readonly Vector3 ZeroClockRotation = new Vector3(0f, 0f, 0f);
        public static readonly Vector3 ThreeClockRotation = new Vector3(0f, 90f, 0f);
        public static readonly Vector3 SixClockRotation = new Vector3(0f, 180f, 0f);
        public static readonly Vector3 NineClockRotation = new Vector3(0f, 270f, 0f);

        public static readonly float interval = 2f;
        public static readonly Vector3 Box = new Vector3 (1f, 1f, 1f);
        public static readonly float Disappear = -2f;
        public static readonly float DisappearTime = 20f;
        public static readonly float Alpha = 1.5f;

        public static void ResetObjects(Transform node, GameObject holder)
        {
            var child = node.GetChild(0);
            DestroyImmediate(child.gameObject);
            Instantiate(holder, Vector3.zero, Quaternion.identity, node);
        }

        public static readonly Vector3[] Directions = 
        {
            Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
            // new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0),
        };
    
        public static readonly Vector3[] EightDirections =
        {
            // Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
            new Vector3(0, 0, 1), 
            new Vector3(0, 0, -1), 
            new Vector3(1, 0, 0), 
            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 1), 
            new Vector3(1, 0, -1), 
            new Vector3(-1, 0, 1), 
            new Vector3(-1, 0, -1),
        };

        static Vector2 Vector2Round (Vector2 inputVector) {

            return new Vector2(Mathf.Round (inputVector.x), Mathf.Round (inputVector.y));
        }

        static Vector3 Vector3Round (Vector3 inputVector) {

            return new Vector3(Mathf.Round (inputVector.x), Mathf.Round (inputVector.y), Mathf.Round(inputVector.z));
        }

        public static void ShuffleArray<T>(T[] array)
        {
            for (int index = 0; index < array.Length; ++index)
            {
                var random1 = UnityEngine.Random.Range (0, array.Length);
                var random2 = UnityEngine.Random.Range (0, array.Length);
 
                var tmp = array[random1];
                array[random1] = array[random2];
                array[random2] = tmp;
            }
        }

        public static Vector2 Coordinate(Vector3 pos) {

            return Vector2Round(new Vector2(pos.x / 2f, pos.z / 2f));
        }
    
        public static Vector3 CoordinateToTransform(Vector2 pos) {

            return Vector3Round(new Vector3(pos.x *2f, 0, pos.y * 2f));
        }
 
        public static void ShuffleList<T> (List<T> list)
        {
            for (int index = 0; index < list.Count; ++index)
            {
                var random1 = UnityEngine.Random.Range(0, list.Count);
                var random2 = UnityEngine.Random.Range(0, list.Count);
 
                var tmp = list[random1];
                list[random1] = list[random2];
                list[random2] = tmp;
            }
        }

        public static void PrintList(List<GameObject> list)
        {
            foreach (var item in list)
            {
                print(item.name);
            }
        }
        
        public static void PrintList(List<Transform> list)
        {
            foreach (var item in list)
            {
                print(item.name);
            }
        }
        
        public static void PrintList(List<int> list)
        {
            foreach (var item in list)
            {
                print(item);
            }
        }
        
        public static void PrintList(List<string> list)
        {
            foreach (var item in list)
            {
                print(item);
            }
        }
        
        public static void PrintList(List<Character> list)
        {
            foreach (var item in list)
            {
                print(item.CharacterName + "(" + item.CharacterId + ")");
            }
        }
        
        public static List<int> RandomExtraction(int count, int cut)
        {
            var newList = new List<int>();
            
            for (int i = 0; i < count; i++)
            {
                newList.Add(i);
            }
                        
            GameUtility.ShuffleList(newList);
                        
            var returnList = newList.GetRange(0, cut);
            
            return returnList;
        }
    }
}