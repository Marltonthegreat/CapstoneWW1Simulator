using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTerrain : Singleton<PerlinTerrain>
{
    Terrain terrain;
    TerrainCollider terrainCollider;
    MeshFilter terrainMesh;
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
        terrainCollider = GetComponent<TerrainCollider>();
        terrainMesh = GetComponent<MeshFilter>();
        BuildNavMesh();
    }

    private void Update()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void AdjustTerrain(Bounds[] bounds)
    {
        // get all vertices
        var vertices = terrain.;
        for (int i = 0; i < vertices.Length; i++)
        {
            var pos = transform.TransformPoint(vertices[i]);

            foreach (Bounds bound in bounds)
            {
                if (bound.Contains(pos))
                {
                    pos = new Vector3(pos.x, bound.min.y, pos.z);
                }

                pos = terrainMesh.transform.worldToLocalMatrix.MultiplyPoint(pos);
            }

        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        /*ContactPoint[] contacts = new ContactPoint[0];
        collision.Get

        List<Vector3> points = new();
        List<Vector3> verts = new();

        foreach(ContactPoint contact in contacts)
        {
            terrainMesh.mesh.GetVertices(verts);
            points.Add(terrainCollider.ClosestPoint(contact.point));
        }*/
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
