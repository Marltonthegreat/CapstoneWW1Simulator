using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshSelector : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, layerMask))
            {
                Agent[] agents = Agent.GetAgents();
                foreach (var agent in agents)
                {
                    if (agent.TryGetComponent(out AgentMovement movement))
                    {
                        movement.MoveTowards(hitInfo.point);
                    }
                }
            }
        }
    }
}
