using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float destroyTimer = 2;
    [SerializeField] float explosionRadius = 2;
    [SerializeField] LayerMask layerMask;
    [SerializeField] AudioClip explosionAudio;
    [SerializeField] GameObject explosionPrefab;

    private Vector3 initialPosition;
    private Vector3 finalPosition = new Vector3(0, 20, -20);

    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        body.MoveRotation(Quaternion.LookRotation(body.velocity.normalized) * Quaternion.AngleAxis(90, Vector3.right));
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;


        Explode();
    }

    private void Explode()
    {
        ExplosionAffect();

        var collisions = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        foreach(Collider collider in collisions)
        {
            Vector3 direction = (collider.transform.position - transform.position).normalized;

            bool hit = Physics.Raycast(transform.position, direction);
            collider.TryGetComponent<Agent>(out Agent agent);

            if(hit && agent != null)
            {
                agent.Damage(100);
            }
            else if (hit && collider.CompareTag("Terrain"))
            {
                Bounds[] bounds = new Bounds[1]; 
                bounds[0] = new Bounds(transform.position, new Vector3(explosionRadius, explosionRadius/2, explosionRadius));
                TrenchManager.Instance.PTerrain.AdjustTerrain(bounds);
                TrenchManager.Instance.PTerrain.BuildNavMesh();
            }
        }

        StartCoroutine(Delete());
    }

    private void ExplosionAffect()
    {
        AudioManager.Instance.PlaySFX(explosionAudio);
        Destroy(Instantiate(explosionPrefab, transform.position, transform.rotation),destroyTimer);
    }

    IEnumerator Delete()
    {
        float timer = 0;

        while (timer < destroyTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(transform.root.gameObject);
    }
}
