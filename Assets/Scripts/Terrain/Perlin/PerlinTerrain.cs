using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class PerlinTerrain : MonoBehaviour
{
    [SerializeField] GameObject[] ObjsToAwake;

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    public Unity.AI.Navigation.NavMeshSurface surface;

    public float height = 20f;

    public int width = 100;
    public int length = 100;

    private int xSize;
    private int zSize;
    [Min(1)] public int resolution = 1;

    public float scale = 20f;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        xSize = width * resolution + 1;
        zSize = length * resolution + 1;

        GenerateTerrain();
        BuildNavMesh();
        Awaken(ObjsToAwake);
    }

    private void GenerateTerrain()
    {
        var vertices = new Vector3[xSize * zSize];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[vertices.Length * 6];
        var heights = GenerateHeights();
        var colorVals = GenerateColors();

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                int index = x * (xSize) + z;
                vertices[index] = new Vector3(x / (float)resolution, heights[x, z], z / (float)resolution);
                colors[index] = new Color(0, colorVals[x, z], 0);
            }
        }

        for (int x = 0, triIndex = 0, vertIndex = 0; x < xSize - 1; x++, vertIndex++)
        {
            for (int y = 0; y < zSize - 1; y++, triIndex += 6, vertIndex++)
            {

                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = vertIndex + 1;
                triangles[triIndex + 2] = vertIndex + zSize;

                triangles[triIndex + 3] = triangles[triIndex + 2];
                triangles[triIndex + 4] = triangles[triIndex + 1];
                triangles[triIndex + 5] = vertIndex + zSize + 1;
            }
        }


        Mesh mesh = new Mesh();

        meshFilter.mesh = mesh;
        mesh.name = "terrain";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    public void AdjustTerrain(Bounds[] bounds)
    {
        var verts = meshFilter.mesh.vertices;
        var colors = meshFilter.mesh.colors;


        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                foreach (Bounds bound in bounds)
                {
                    int index = x * xSize + z;
                    if (bound.Contains(meshFilter.transform.position + verts[index]))
                    {
                        verts = ChangeHeights(verts, bound, index, x, z);

                        colors = ChangeColors(colors, index, x, z);
                    }
                }
            }
        }

        meshFilter.mesh.vertices = verts;
        meshFilter.mesh.colors = colors;
        meshCollider.sharedMesh = meshFilter.mesh;
        meshFilter.mesh.RecalculateNormals();
    }

    public void BuildNavMesh()
    {
        surface?.BuildNavMesh();
    }
    
    public void Awaken(GameObject[] objects)
    {
        foreach (var obj in objects)
        {
            obj.SetActive(true);
        }
    }

    float[,] GenerateHeights()
    {
        float offsetX = Random.Range(-1000f, 1000);
        float offsetZ = Random.Range(-1000f, 1000);

        float[,] heights = new float[xSize, zSize];
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int z = 0; z < zSize - 1; z++)
            {
                heights[x, z] = CalcHeight(x, z, offsetX, offsetZ);
            }
        }

        return heights;
    }

    float[,] GenerateColors()
    {

        float offsetX = Random.Range(-1000f, 1000);
        float offsetZ = Random.Range(-1000f, 1000);

        float[,] colors = new float[xSize, zSize];
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int z = 0; z < zSize - 1; z++)
            {
                colors[x, z] = CalcColor(x, z, offsetX, offsetZ);
            }
        }

        return colors;
    }

    float CalcHeight(int x, int z, float offsetX, float offsetZ)
    {
        float xCoord = (float)x / xSize * scale + offsetX;
        float zCoord = (float)z / zSize * scale + offsetZ;

        return Mathf.PerlinNoise(xCoord, zCoord) * height;
    }

    float CalcColor(int x, int z, float offsetX, float offsetZ)
    {
        float xCoord = (float)x / xSize * scale * 2 + offsetX;
        float zCoord = (float)z / zSize * scale * 2 + offsetZ;

        return Mathf.Clamp(Mathf.PerlinNoise(xCoord, zCoord) / 4 + .25f, .25f, .5f);
    }

    Vector3[] ChangeHeights(Vector3[] verts, Bounds bound, int index, int x, int z)
    {
        var tempIndex = (x - 1) * (xSize) + z;
        if (tempIndex > 0 && tempIndex < verts.Length && verts[tempIndex].y <= bound.min.y)
            verts[tempIndex] = new Vector3(verts[tempIndex].x, bound.min.y + bound.min.y / 2, verts[tempIndex].z);

        tempIndex = (x + 1) * (xSize) + z;
        if (tempIndex > 0 && tempIndex < verts.Length && verts[tempIndex].y <= bound.min.y) 
            verts[tempIndex] = new Vector3(verts[tempIndex].x, bound.min.y + bound.min.y / 2, verts[tempIndex].z);

        tempIndex = (x - 1) * (xSize) + z + 1;
        if (tempIndex > 0 && tempIndex < verts.Length && verts[tempIndex].y <= bound.min.y) 
            verts[tempIndex] = new Vector3(verts[tempIndex].x, bound.min.y + bound.min.y / 2, verts[tempIndex].z);

        tempIndex = (x + 1) * (xSize) + z + 1;
        if (tempIndex > 0 && tempIndex < verts.Length && verts[tempIndex].y <= bound.min.y) 
            verts[tempIndex] = new Vector3(verts[tempIndex].x, bound.min.y + bound.min.y / 2, verts[tempIndex].z);

        tempIndex = (x - 1) * (xSize) + z - 1;
        if (tempIndex > 0 && tempIndex < verts.Length && verts[tempIndex].y <= bound.min.y) 
            verts[tempIndex] = new Vector3(verts[tempIndex].x, bound.min.y + bound.min.y / 2, verts[tempIndex].z);

        tempIndex = (x + 1) * (xSize) + z - 1;
        if (tempIndex > 0 && tempIndex < verts.Length && verts[tempIndex].y <= bound.min.y) 
            verts[tempIndex] = new Vector3(verts[tempIndex].x, bound.min.y + bound.min.y / 2, verts[tempIndex].z);

        verts[index] = new Vector3(verts[index].x, bound.min.y, verts[index].z);
        return verts;
    }



    Color[] ChangeColors(Color[] colors, int index, int x, int z)
    {
        float r = 38 / 255f;
        float g = 21 / 255f;
        float b = 6 / 255f;

        Color newColor = new Color(r, g, b);

        var tempIndex = (x - 1) * (xSize) + z;
        if (tempIndex > 0 && tempIndex < colors.Length && !colors[tempIndex].Equals(newColor)) colors[tempIndex] = newColor;

        tempIndex = (x + 1) * (xSize) + z;
        if (tempIndex > 0 && tempIndex < colors.Length && !colors[tempIndex].Equals(newColor)) colors[tempIndex] = newColor;

        tempIndex = (x - 1) * (xSize) + z + 1;
        if (tempIndex > 0 && tempIndex < colors.Length && !colors[tempIndex].Equals(newColor)) colors[tempIndex] = newColor;

        tempIndex = (x + 1) * (xSize) + z + 1;
        if (tempIndex > 0 && tempIndex < colors.Length && !colors[tempIndex].Equals(newColor)) colors[tempIndex] = newColor;

        tempIndex = (x - 1) * (xSize) + z - 1;
        if (tempIndex > 0 && tempIndex < colors.Length && !colors[tempIndex].Equals(newColor)) colors[tempIndex] = newColor;

        tempIndex = (x + 1) * (xSize) + z - 1;
        if (tempIndex > 0 && tempIndex < colors.Length && !colors[tempIndex].Equals(newColor)) colors[tempIndex] = newColor;

        colors[index] = newColor;
        return colors;
    }
}