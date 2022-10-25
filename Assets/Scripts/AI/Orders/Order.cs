using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order : MonoBehaviour
{
    protected Order(GameObject markerPrefab, string name, Vector3 location, int teamID)
    {
        MarkerPrefab = markerPrefab;
        Name = name;
        Location = location;
        TeamID = teamID;
    }

    protected GameObject MarkerPrefab;

    public string Name { get; protected set; }
    public Vector3 Location { get; protected set; }

    public int TeamID { get; private set; }

}
