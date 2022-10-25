using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatOrder : Order
{
    public RetreatOrder(GameObject markerPrefab, string name, Vector3 location, int teamID) : base(markerPrefab, name, location, teamID)
    {
    }
}
