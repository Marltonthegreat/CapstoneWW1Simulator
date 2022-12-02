using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class PerlinTerrain : Singleton<PerlinTerrain>
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    public Unity.AI.Navigation.NavMeshSurface surface;

    Vector3[] vertices;

    public float height = 20f;
    public int width = 256;
    public int length = 256;

    public float scale = 20f;

    private float offsetX = 100f;
    private float offsetY = 100f;

    private void Start()
    {
        offsetX = Random.Range(-1000f, 1000);
        offsetY = Random.Range(-1000f, 1000);

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        GenerateTerrain();
        BuildNavMesh();
    }

    public void AdjustTerrain(Bounds[] bounds)
    {
        var verts = meshFilter.mesh.vertices;


        for (int x = 0; x <= width; x++)
        {
            for (int z = 0; z <= length; z++)
            {
                foreach (Bounds bound in bounds)
                {
                    if (bound.Contains(meshFilter.transform.position + verts[x * (width + 1) + z]))
                    {
                        verts[x * (width + 1) + z] = new Vector3(verts[x * (width + 1) + z].x, bound.min.y, verts[x * (width + 1) + z].z);
                    }
                }
            }
        }

        meshFilter.mesh.vertices = verts;
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public void BuildNavMesh()
    {
        surface.BuildNavMesh();
    }

    private void GenerateTerrain()
    {
        vertices = new Vector3[(width + 1) * (length + 1)];
        int[] triangles = new int[width * length * 6];
        var heights = GenerateHeights();

        for (int x = 0; x <= width; x++)
        {
            for (int z = 0; z <= length; z++)
            {
                vertices[x * (width + 1) + z] = new Vector3(x, heights[x, z], z);
            }
        }

        for (int x = 0, triIndex = 0, vertIndex = 0; x < width; x++, vertIndex++)
        {
            for (int y = 0; y < length; y++, triIndex += 6, vertIndex++)
            {

                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = vertIndex + 1;
                triangles[triIndex + 2] = vertIndex + length + 1;

                triangles[triIndex + 3] = triangles[triIndex + 2];
                triangles[triIndex + 4] = triangles[triIndex + 1];
                triangles[triIndex + 5] = vertIndex + length + 2;
            }
        }


        Mesh mesh = new Mesh();

        meshFilter.mesh = mesh;
        mesh.name = "terrain";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width + 1, length + 1];
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

        return Mathf.PerlinNoise(xCoord, zCoord) * height;
    }
}