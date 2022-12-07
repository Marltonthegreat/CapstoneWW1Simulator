using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastPerception : Perception
{
    [SerializeField] Transform sphereCastTransform;
    [SerializeField] [Range(2, 50)] public int numRaycast = 2;
    [SerializeField] [Range(0, 5)] public float radius = 2;
    [SerializeField] LayerMask layerMask;

    public override GameObject[] GetGameObjects()
    {
        List<GameObject> result = new List<GameObject>();

        float angleOffset = (angle * 2) / (numRaycast - 1);
        for (int i = 0; i < numRaycast; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(-angle + (angleOffset * i), Vector3.up);
            Vector3 direction = rotation * sphereCastTransform.forward;
            Ray ray = new Ray(sphereCastTransform.position, direction);
            if (Physics.SphereCast(ray, radius, out RaycastHit raycastHit, distance, layerMask))
            {
                if (tagName == "" || raycastHit.collider.CompareTag(tagName))
                {
                    Debug.DrawRay(ray.origin, ray.direction * raycastHit.distance, Color.red);
                    if (!result.Contains(raycastHit.collider.gameObject)) result.Add(raycastHit.collider.gameObject);
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.white);
            }
        }

        return result.ToArray();
    }
}
