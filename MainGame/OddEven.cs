using System.Collections.Generic;
using UnityEngine;

public class OddEven : MonoBehaviour
{
    [ContextMenu("Odd")]
    public void CheckOdd()
    {
        var allTransforms = transform.GetComponentsInChildren<Transform>();
        var list = new List<Transform>();
        
        foreach (var item in allTransforms)
        {
            if (item.transform.position.x % 2 == 0)
            {
                print(item.name + " is not odd");
                list.Add(item);
            }
        }

        if (list.Count == 0)
        {
            print("Perfect!");
        }
    }

    [ContextMenu(("Even"))]
    public void CheckEven()
    {
        var allTransforms = transform.GetComponentsInChildren<Transform>();
        var list = new List<Transform>();
        
        foreach (var item in allTransforms)
        {
            if (item.transform.position.x % 2 == 1)
            {
                print(item.name + " is not even");   
                list.Add(item);
            }
        }
        
        if (list.Count == 0)
        {
            print("Perfect!");
        }
    }
    
}
