using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPoppy : MonoBehaviour
{
    [SerializeField] LayerMask mask;

    // Start is called before the first frame update
    void Awake()
    {
        transform.position = new Vector3(Random.Range(-3, 6f), 10, Random.Range(-2, 2f  ));

        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out RaycastHit hitInfo, mask);

        transform.position = hitInfo.point;
    }
}
