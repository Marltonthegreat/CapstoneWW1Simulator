using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NeighborList
{
    public string Name;
    public List<TerrainSection> ValidNeighbors;

    public NeighborList(string Name)
    {
        this.Name = Name;
        ValidNeighbors = new();
    }
}
