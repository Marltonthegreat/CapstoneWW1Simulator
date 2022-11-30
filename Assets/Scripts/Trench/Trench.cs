using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trench : MonoBehaviour
{
    [SerializeField] Transform[] ConnectionNodes;

    public Transform[] GetConnectionNodes()
    { 
        return ConnectionNodes;
    }
    
    public Transform GetConnectionNode(int index)
    {
        if (ConnectionNodes == null || index > ConnectionNodes.Length || index < 0) return null;

        return ConnectionNodes[index];
    }
}
