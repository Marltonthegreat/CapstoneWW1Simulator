using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TileSet : MonoBehaviour
{
    public TerrainSection[] Terrain;

    private void ResetValidNeighbors()
    {
        foreach (TerrainSection section in Terrain)
        {
            section.NeighborLists = new NeighborList[6]
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

    public void GenerateValidNeighbors()
    {
        ResetValidNeighbors();

        GenerateEmptyNeighbors();

        for (int i = 1; i < Terrain.Length; i++)
        {
            if (Terrain[i] == null) continue;
            for (int j = 1; j < Terrain.Length; j++)
            {
                if (Terrain[j] == null) continue;

                foreach (TerrainSection.Socket SocketA in Terrain[i].Sockets)
                {
                    var SocketB = Terrain[j].Sockets.First(s => s.Name.Equals(SocketA.OppositeName));

                    if (SocketA.Type.Contains('-'))
                    {
                        AddNeighbor(Terrain[i], Terrain[0], SocketA);
                    }
                    else if (SocketA.Type.Contains('s') && SocketB.Type.Contains('s') && SocketA.Type.Trim().Equals(SocketB.Type.Trim()))
                    {
                        AddNeighbor(Terrain[i], Terrain[j], SocketA);
                    }
                    else if ((SocketA.Type.Trim() + "f").Equals(SocketB.Type) || (SocketB.Type.Trim() + "f").Equals(SocketA.Type))
                    {
                        AddNeighbor(Terrain[i], Terrain[j], SocketA);
                    }

                }
            }
        }
    }

    private void GenerateEmptyNeighbors()
    {
        var soc = Terrain[0].Sockets.First(s => s.Name.Equals("negY"));

        foreach (TerrainSection.Socket socket in Terrain[0].Sockets)
        {
            AddNeighbor(Terrain[0], Terrain[0], socket);
        }

        for (int i = 1; i < Terrain.Length; i++)
        {
            AddNeighbor(Terrain[0], Terrain[i], soc);
        }
    }

    private void AddNeighbor(TerrainSection Terrain1, TerrainSection Terrain2, TerrainSection.Socket Socket)
    {
        var neighborList = Terrain1.NeighborLists.First(t => t.Name.Trim().Equals(Socket.Name));

        if (!neighborList.ValidNeighbors.Contains(Terrain2) || Terrain2.Equals(Terrain[1]))
            neighborList.ValidNeighbors.Add(Terrain2);
    }
}
