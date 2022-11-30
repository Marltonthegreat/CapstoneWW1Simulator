using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTerrain : Singleton<PerlinTerrain>
{
    Terrain terrain;
    public Unity.AI.Navigation.NavMeshSurface surface;

    public float height = 20f;

    public int width = 256;
    public int length = 256;

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    private void Start()
    {
        offsetX = Random.Range(-1000f, 1000);
        offsetY = Random.Range(-1000f, 1000);

        terrain = GetComponent<Terrain>();

        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        BuildNavMesh();
    }

    public void AdjustTerrain(Bounds[] bounds)
    {
        TerrainData data = terrain.terrainData;

        // get all vertices
        float[,] heights = data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution);
        Vector3[,] worldPos = new Vector3[data.heightmapResolution, data.heightmapResolution]; ;

        GetWorldPos(data, heights, ref worldPos);

        int initX = -1, initZ = -1;
        int finalX = -1, finalZ = -1;

        for (int x = 0; x < data.heightmapResolution; x++)
        {
            for (int z = 0; z < data.heightmapResolution; z++)
            {
                foreach (Bounds bound in bounds)
                {
                    if (bound.Contains(worldPos[x, z]))
                    {
                        if (initX == -1 && initZ == -1)
                        {
                            initX = x;
                            initZ = z;
                        }

                        worldPos[x, z] = new Vector3(worldPos[x, z].x, bound.min.y, worldPos[x, z].z);
                        finalX = x;
                        finalZ = z;
                    }
                }
            }
        }

        SetLocalHeights(data, ref heights, worldPos, initX, initZ, finalX, finalZ);

        data.SetHeights(0, 0, heights);
    }

    private void GetWorldPos(TerrainData data, float[,] heights, ref Vector3[,] worldPos)
    {
        for (int x = 0; x < data.heightmapResolution; x++)
        {
            float relativePosX = x / (float)data.heightmapResolution;
            float worldPosX = relativePosX * data.size.x + transform.position.x;

            for (int z = 0; z < data.heightmapResolution; z++)
            {
                float relativePosZ = z / (float)data.heightmapResolution;

                float worldPosZ = relativePosZ * data.size.z + transform.position.z;

                float worldPosY = heights[x, z] * data.size.y + transform.position.y;

                worldPos[x, z] = new Vector3(worldPosZ, worldPosY, worldPosX);
            }
        }
    }

    private void SetLocalHeights(TerrainData data, ref float[,] heights, Vector3[,] worldPos, int initX = 0, int initZ = 0, int finalX = -1, int finalZ = -1)
    {
        finalX = (finalX < 0) ? data.heightmapResolution : finalX;
        finalZ = (finalZ < 0) ? data.heightmapResolution : finalZ;

        for (int x = initX; x < data.heightmapResolution && x <= finalX; x++)
        {
            for (int z = initZ; z < data.heightmapResolution && x <= finalZ; z++)
            {
                float localHeight = (worldPos[x, z].y - transform.position.y) / data.size.y;
                if (localHeight != heights[x, z])
                {
                    Debug.Log($"\nCoords: {x}, {z}\nWorldPos: {worldPos[x, z]}\nHeight Normalized: {heights[x, z]}\nHeight: {heights[x, z] * data.size.y + transform.position.y}");
                    Debug.Log($"Height Normalized: {heights[x, z]}\nHeight: {heights[x, z] * data.size.y + transform.position.y}");
                }
                heights[x, z] = localHeight;
            }
        }
    }

    public void BuildNavMesh()
    {
        surface.BuildNavMesh();
    }


    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, length);

        terrainData.SetHeights(0, 0, GenerateHeights());


        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                heights[x, z] = CalcHeight(x, z);
            }
        }

        return heights;
    }

    float CalcHeight(int x, int z)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float zCoord = (float)z / length * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, zCoord);
    }
}
