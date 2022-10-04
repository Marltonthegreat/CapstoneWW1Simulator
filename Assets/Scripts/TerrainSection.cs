using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSection : MonoBehaviour
{
    [Serializable]
    public struct Socket
    {
        public string Name;
        public string OppositeName;
        public string Type;

        public Socket(string Name, string oppositeName,string Type)
        {
            this.Name = Name;
            this.OppositeName = oppositeName;
            this.Type = Type;
        }
    }

    public Socket[] Sockets;
    public NeighborList[] NeighborLists;

    [Header("Offests")]
    public int xOffset;
    public int yOffset;
    public int zOffset;

    public TerrainSection()
    {

        NeighborLists = new NeighborList[6]
        {
            new NeighborList("posX"),
            new NeighborList("posZ"),
            new NeighborList("negX"),
            new NeighborList("negZ"),
            new NeighborList("posY"),
            new NeighborList("negY")
        };
    }
}
