using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    [SerializeField] Material material;
    //[SerializeField] Gradient color;

    public void CombineMeshes()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>().Where(mF => !mF.gameObject.transform.parent.name.Contains("TerrainEmpty")).ToArray();
        


        Debug.Log(name + " is combining " + filters.Length + " meshes");    

        CombineInstance[] combine = new CombineInstance[filters.Length];
        for (int i = 0; i < filters.Length; i ++)
        {
            combine[i].subMeshIndex = 0;
            combine[i].mesh = filters[i].sharedMesh;
            combine[i].transform = filters[i].transform.localToWorldMatrix;
            filters[i].gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponent<MeshRenderer>().material = material;

        transform.gameObject.SetActive(true);

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
