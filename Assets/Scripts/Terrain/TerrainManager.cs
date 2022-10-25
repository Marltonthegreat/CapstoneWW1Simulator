using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class TerrainManager : MonoBehaviour
{
    private class Tile
    {

        public bool IsChecked = false;
        public bool Collapsed = false;
        public List<int> Entropy;

        public NeighborList[] NeighborLists;

        public Tile()
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

        public void UpdateNeighborList(TileSet set)
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

            for (int i = 0; i < NeighborLists.Count(); i++)
            {
                var list = NeighborLists[i];

                for (int j = 0; j < Entropy.Count; j++)
                {
                    var validNeighbors = set.Terrain[Entropy[j]].NeighborLists.First(l => l.Name.Equals(list.Name)).ValidNeighbors;
                    list.ValidNeighbors.AddRange(validNeighbors);
                }

                list.ValidNeighbors = list.ValidNeighbors.Distinct().ToList();
            }
        }

        public override string ToString()
        {
            return $"Collapsed: {Collapsed}\nEntropy: {Entropy.Count}";
        }
    }

    private struct TileIndex
    {
        public int x, y, z;

        public TileIndex(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [Serializable]
    private struct Dimensions
    {
        public int x;
        public int y;
        public int z;
    }

    [SerializeField] float DelayTime = 0;
    [SerializeField] TileSet tileSet;

    [SerializeField] int tileDim;
    [SerializeField] Dimensions chunkDim;

    [SerializeField] GameObject world;

    private Tile[] tiles;

    private bool initialized = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        UnityEngine.Random.InitState(29);

        tileSet.GenerateValidNeighbors();

        SetupTiles();

        StartCoroutine(GenerateTerrain());
    }

    private void SetupTiles()
    {
        // Create number of tiles to fill the grid
        tiles = new Tile[chunkDim.x * chunkDim.y * chunkDim.z];

        for (int i = 0; i < tiles.Length; i++)
        {
            // Create blank tiles
            tiles[i] = new Tile();
            tiles[i].Entropy = new();
            for (int j = 0; j < tileSet.Terrain.Length; j++)
            {
                tiles[i].Entropy.Add(j);
            }
            tiles[i].UpdateNeighborList(tileSet);
        }
    }

    private IEnumerator GenerateTerrain()
    {
        bool generating = true, skip = false;
        do
        {
            // Find the lowest entropy
            Tile[] ordered = tiles.OrderBy(t => t.Entropy.Count).ToArray();
            Tile[] orderedNotCollapses = ordered.Where(t => !t.Collapsed).ToArray();

            Tile[] lowestEntropy = orderedNotCollapses.Where(t => t.Entropy.Count == orderedNotCollapses[0].Entropy.Count).ToArray();

            // Pick from lowest entropy and collapse it
            var index = UnityEngine.Random.Range(0, lowestEntropy.Length);
            var chosenTile = lowestEntropy[index];

            index = UnityEngine.Random.Range(0, chosenTile.Entropy.Count);
            var chosenState = chosenTile.Entropy[index];
            chosenTile.Entropy = new List<int> { chosenState };
            chosenTile.Collapsed = true;

            chosenTile.UpdateNeighborList(tileSet);

            generating = false;

            foreach (Tile tile in tiles)
            {
                if (!tile.Collapsed)
                {
                    generating = true;
                    UpdateTiles(chosenTile, ref skip);
                    break;
                }
            }
            RenderTerrain();
            yield return new WaitForSeconds(DelayTime);
        } while (generating && !skip);

        world.GetComponent<MeshCombiner>().CombineMeshes();

    }

    private void UpdateTiles(Tile currentTile, ref bool skip)
    {
        for (int y = 0; y < chunkDim.y; y++)
        {
            for (int z = 0; z < chunkDim.z; z++)
            {
                for (int x = 0; x < chunkDim.x; x++)
                {
                    int index = x + z * chunkDim.x + y * chunkDim.x * chunkDim.z;
                    if (tiles[index].Equals(currentTile))
                    {
                        AnalyzeSurroundings(null, currentTile, new TileIndex(x, y, z), ref skip);
                    }
                }
            }
        }
    }

    private void AnalyzeSurroundings(Tile previousTile, Tile currentTile, TileIndex tileIndex, ref bool skip)
    {
        int index;
        currentTile.IsChecked = true;

        if (skip) return;
        //Look Forward
        if (tileIndex.z < chunkDim.z - 1)
        {
            index = tileIndex.x + (tileIndex.z + 1) * chunkDim.x + tileIndex.y * chunkDim.x * chunkDim.z;
            if (!tiles[index].Collapsed && !tiles[index].IsChecked)
            {
                var prevEntropy = tiles[index].Entropy;
                UpdateEntropy(tiles[index], currentTile.NeighborLists.First(l => l.Name.Equals("posZ")));
                if (prevEntropy.Count != tiles[index].Entropy.Count) AnalyzeSurroundings(currentTile, tiles[index], new TileIndex(tileIndex.x, tileIndex.y, tileIndex.z + 1), ref skip);
                tiles[index].IsChecked = false;
            }
        }
        if (skip) return;
        // Look Backward
        if (tileIndex.z > 0)
        {
            index = tileIndex.x + (tileIndex.z - 1) * chunkDim.x + tileIndex.y * chunkDim.x * chunkDim.z;
            if (!tiles[index].Collapsed && !tiles[index].IsChecked)
            {
                var prevEntropy = tiles[index].Entropy;
                UpdateEntropy(tiles[index], currentTile.NeighborLists.First(l => l.Name.Equals("negZ")));
                if (prevEntropy.Count != tiles[index].Entropy.Count) AnalyzeSurroundings(currentTile, tiles[index], new TileIndex(tileIndex.x, tileIndex.y, tileIndex.z - 1), ref skip);
                tiles[index].IsChecked = false;
            }
        }
        if (skip) return;
        // Look Right
        if (tileIndex.x < chunkDim.x - 1)
        {
            index = tileIndex.x + 1 + tileIndex.z * chunkDim.x + tileIndex.y * chunkDim.x * chunkDim.z;
            if (!tiles[index].Collapsed && !tiles[index].IsChecked)
            {
                var prevEntropy = tiles[index].Entropy;
                UpdateEntropy(tiles[index], currentTile.NeighborLists.First(l => l.Name.Equals("posX")));
                if (prevEntropy.Count != tiles[index].Entropy.Count) AnalyzeSurroundings(currentTile, tiles[index], new TileIndex(tileIndex.x + 1, tileIndex.y, tileIndex.z), ref skip);
                tiles[index].IsChecked = false;
            }
        }
        if (skip) return;
        // Look Left
        if (tileIndex.x > 0)
        {
            index = tileIndex.x - 1 + tileIndex.z * chunkDim.x + tileIndex.y * chunkDim.x * chunkDim.z;
            if (!tiles[index].Collapsed && !tiles[index].IsChecked)
            {
                var prevEntropy = tiles[index].Entropy;
                UpdateEntropy(tiles[index], currentTile.NeighborLists.First(l => l.Name.Equals("negX")));
                if (prevEntropy.Count != tiles[index].Entropy.Count) AnalyzeSurroundings(currentTile, tiles[index], new TileIndex(tileIndex.x - 1, tileIndex.y, tileIndex.z), ref skip);
                tiles[index].IsChecked = false;
            }
        }
        if (skip) return;
        // Look Up
        if (tileIndex.y < chunkDim.y - 1)
        {
            index = tileIndex.x + tileIndex.z * chunkDim.x + (tileIndex.y + 1) * chunkDim.x * chunkDim.z;
            if (!tiles[index].Collapsed && !tiles[index].IsChecked)
            {
                var prevEntropy = tiles[index].Entropy;
                UpdateEntropy(tiles[index], currentTile.NeighborLists.First(l => l.Name.Equals("posY")));
                if (prevEntropy.Count != tiles[index].Entropy.Count) AnalyzeSurroundings(currentTile, tiles[index], new TileIndex(tileIndex.x, tileIndex.y + 1, tileIndex.z), ref skip);
                tiles[index].IsChecked = false;
            }
        }
        if (skip) return;
        // Look Down
        if (tileIndex.y > 0)
        {
            index = tileIndex.x + tileIndex.z * chunkDim.x + (tileIndex.y - 1) * chunkDim.x * chunkDim.z;
            if (!tiles[index].Collapsed && !tiles[index].IsChecked)
            {
                var prevEntropy = tiles[index].Entropy;
                UpdateEntropy(tiles[index], currentTile.NeighborLists.First(l => l.Name.Equals("negY")));
                if (prevEntropy.Count != tiles[index].Entropy.Count) AnalyzeSurroundings(currentTile, tiles[index], new TileIndex(tileIndex.x, tileIndex.y - 1, tileIndex.z), ref skip);
                tiles[index].IsChecked = false;
            }
        }
    }

    private void UpdateEntropy(Tile otherTile, NeighborList list)
    {
        //var terrain = tileSet.Terrain[currTile.Entropy[0]];
        //var test = terrain.Sockets.First(s => s.Equals(socket));
        int[] temp = otherTile.Entropy.ToArray();


        for (int i = 0; i < otherTile.Entropy.Count; i++)
        {
            bool valid = false;
            foreach (TerrainSection section in list.ValidNeighbors)
            {
                if (tileSet.Terrain[otherTile.Entropy[i]].Equals(section))
                {
                    valid = true;
                    break;
                }
            }

            if (!valid) temp[i] = -1;
        }

        var tempList = new List<int>(temp).Where(i => i != -1).ToList();

        otherTile.Entropy = tempList;

        if (otherTile.Entropy.Count == 1) otherTile.Collapsed = true;

        otherTile.UpdateNeighborList(tileSet);

    }

    private void RenderTerrain()
    {

        foreach (Transform child in world.transform)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < chunkDim.y; y++)
        {
            for (int z = 0; z < chunkDim.z; z++)
            {
                for (int x = 0; x < chunkDim.x; x++)
                {
                    Tile tile = tiles[y * chunkDim.x * chunkDim.z + z * chunkDim.x + x];
                    if (tile.Collapsed)
                    {
                        var terrain = tileSet.Terrain[tile.Entropy[0]];
                        var position = new Vector3(x * tileDim + terrain.xOffset, y * tileDim + terrain.yOffset, z * tileDim + terrain.zOffset);
                        if (terrain != null) Instantiate(terrain, position + transform.position, terrain.transform.rotation, world.transform);
                    }
                }
            }
        }
    }

}
