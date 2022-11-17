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
        //terrainCollider = GetComponent<TerrainCollider>();
        BuildNavMesh();
    }

    private void Update()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UpdateHeight(position, -0.2f);
    }

    private void UpdateHeight(Vector3 position, float delta)
    {
        // get vertices at position

    }


    public void BuildNavMesh()
    {
        surface.BuildNavMesh();
    }

    public void AdjustTerrain(GameObject go)
    {
        
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, length);

        terrainData.SetHeights(0, 0, GenerateHeights());
        //terrainData.hei

        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                heights[x, y] = CalcHeight(x, y);
            }
        }

        return heights;
    }

    float CalcHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / length * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
