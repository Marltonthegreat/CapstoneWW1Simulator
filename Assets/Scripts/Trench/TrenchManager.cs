using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenchManager : Singleton<TrenchManager>
{
    public enum TrenchType
    {
        ThreeWay,
        FourWay,
        Bunker,
        Curve,
        End,
        Exit,
        Turn,
        Zag,
        Straight
    }

    private List<Trench> SpawnedTrenches;

    [SerializeField] TrenchType SelectedTrenchType = TrenchType.Straight;
    [SerializeField] GameObject[] Trenches;
    private GameObject SelectedTrench;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSelectedTrenchType(string type)
    {
        switch (type)
        {
            case "3Way":
                SelectedTrenchType = TrenchType.ThreeWay;
                break;
            case "4Way":
                SelectedTrenchType = TrenchType.FourWay;
                break;
            case "Bunker":
                SelectedTrenchType = TrenchType.Bunker;
                break;
            case "Curve":
                SelectedTrenchType = TrenchType.Curve;
                break;
            case "End":
                SelectedTrenchType = TrenchType.End;
                break;
            case "Exit":
                SelectedTrenchType = TrenchType.Exit;
                break;
            case "Turn":
                SelectedTrenchType = TrenchType.Turn;
                break;
            case "Zag":
                SelectedTrenchType = TrenchType.Zag;
                break;
            case "Straight":
                SelectedTrenchType = TrenchType.Straight;
                break;
            default:
                break;
        }
        SetSelectedTrench();
    }

    private void SetSelectedTrench()
    {
        switch (SelectedTrenchType)
        {
            case TrenchType.ThreeWay:
                SelectedTrench = Trenches[0];
                break;
            case TrenchType.FourWay:
                SelectedTrench = Trenches[1];
                break;
            case TrenchType.Bunker:
                SelectedTrench = Trenches[2];
                break;
            case TrenchType.Curve:
                SelectedTrench = Trenches[3];
                break;
            case TrenchType.End:
                SelectedTrench = Trenches[4];
                break;
            case TrenchType.Exit:
                SelectedTrench = Trenches[5];
                break;
            case TrenchType.Turn:
                SelectedTrench = Trenches[6];
                break;
            case TrenchType.Zag:
                SelectedTrench = Trenches[7];
                break;
            case TrenchType.Straight:
                SelectedTrench = Trenches[8];
                break;
            default:
                break;
        }
    }

    public void SpawnTrench(RaycastHit raycastHit, int rotation)
    {
        GameObject trench;

        if (raycastHit.collider != null && raycastHit.collider.CompareTag("Connection"))
        {
            var trenchComp = SelectedTrench.GetComponent<Trench>();

            int rotationMod = rotation % trenchComp.ConnectionLength;

            var node = trenchComp.GetConnectionNode(rotationMod);

            trench = Instantiate(SelectedTrench, raycastHit.collider.transform.position - Quaternion.AngleAxis(90 * rotationMod, Vector3.up) * node.position, raycastHit.collider.transform.parent.rotation * node.rotation);
        }
        else
        {
            trench = Instantiate(SelectedTrench, raycastHit.point /*+ new Vector3(0, 1, 0)*/, Quaternion.AngleAxis(90 * rotation, Vector3.up));
        }

        Bounds[] bounds = GetBounds(trench);

        PerlinTerrain.Instance.AdjustTerrain(bounds);
    }

    private Bounds[] GetBounds(GameObject go)
    {
        List<Bounds> bounds = new();
        List<BoxCollider> colliders = new();
        colliders.AddRange(go.GetComponentsInChildren<BoxCollider>());

        foreach (var collider in colliders)
        {
            bounds.Add(collider.bounds);
        }

        return bounds.ToArray();
    }
}
