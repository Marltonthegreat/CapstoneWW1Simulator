using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order
{
    protected Order(string name, GameObject markerPrefab, Vector3 location, int playerID)
    {
        MarkerPrefab = markerPrefab;
        Name = name;
        Location = location;
        PlayerID = playerID;

        GameObject.Instantiate(markerPrefab, location, Quaternion.AngleAxis(0, Vector3.up));
    }

    public delegate void OnChange();
    public static event OnChange OrderChange;
    
    public delegate void OnRemove();
    public static event OnRemove RemoveOrder;

    protected GameObject MarkerPrefab;

    public string Name { get; protected set; }
    public Vector3 Location { get; protected set; }

    public int PlayerID { get; private set; }

    public float PlayerWeight = 1;
    public float TrueWeight = 1;

}
