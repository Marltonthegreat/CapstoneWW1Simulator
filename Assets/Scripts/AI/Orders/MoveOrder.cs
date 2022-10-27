using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrder : Order
{
    public MoveOrder(string name, GameObject markerPrefab, Vector3 location, int playerID) : base(name, markerPrefab, location, playerID)
    {
    }
}
