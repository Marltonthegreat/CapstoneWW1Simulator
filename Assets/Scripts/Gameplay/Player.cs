using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Player(int playerID, int teamID)
    {
        PlayerID = playerID;
        TeamID = teamID;
    }

    [Header("British Units")]
    [SerializeField] GameObject britishPrefab;
    [SerializeField] Transform britishSpawnTransform;

    [Header("German Units")]
    [SerializeField] GameObject germanPrefab;
    [SerializeField] Transform germanSpawnTransform;

    [Header("Projectile")]
    [SerializeField] GameObject ProjectilePrefab;


    [Header("Other")]
    [SerializeField] LayerMask layerMask;

    private int rotation;

    [HideInInspector] public List<Order> StandingOrders;
    [HideInInspector] public List<Agent> Units;

    private Vector2 camMovement;

    public int PlayerID { get; set; }
    public int TeamID { get; set; }

    private bool BuildMode = false;

    private bool ArtilleryMode = false;

    private void Start()
    {
        StandingOrders = new();
        Units = new();

        GameManager.Instance.AddPlayer(this);
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + camMovement.x, transform.position.y, transform.position.z + camMovement.y);
    }

    public void OnMovement(InputValue value)
    {
        camMovement = value.Get<Vector2>();
    }

    public void OnFire()
    {
        if (ArtilleryMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100))
            {
                Instantiate(ProjectilePrefab, hitInfo.point + new Vector3(0, transform.position.y), transform.rotation);
            }
        }
    }

    public void OnIssueCommand()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!BuildMode)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, layerMask))
            {
                GameManager.Instance.CreateOrder(hitInfo.point);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, layerMask))
            {
                TrenchManager.Instance.SpawnTrench(hitInfo, rotation);
                TrenchManager.Instance.PTerrain.BuildNavMesh();
            }
        }
    }

    public void OnSpawnBritishUnit()
    {
        var go = Instantiate(britishPrefab, britishSpawnTransform.position, britishSpawnTransform.rotation);
        AddUnit(go.GetComponent<Agent>());
    }

    public void OnSpawnGermanUnit()
    {
        var go = Instantiate(germanPrefab, germanSpawnTransform.position, germanSpawnTransform.rotation);
        var otherPlayer = GameManager.Instance.FindPlayerByID((PlayerID == 0) ? 1 : 0);

        otherPlayer.AddUnit(go.GetComponent<Agent>(), otherPlayer.TeamID, otherPlayer.PlayerID);
    }

    public void OnRotateTrench()
    {
        rotation = ++rotation % 4;
    }

    public void AddUnit(Agent unit)
    {
        if (!Units.Contains(unit)) Units.Add(unit);
        unit.TeamID = TeamID;
        unit.PlayerID = PlayerID;
    }

    public void AddUnit(Agent unit, int teamID, int playerID)
    {
        if (!Units.Contains(unit)) Units.Add(unit);
        unit.TeamID = teamID;
        unit.PlayerID = playerID;
    }

    public void AddUnits(List<Agent> units)
    {
        Units.AddRange(units.Where(u => !Units.Contains(u)));
    }

    public void RemoveUnit(Agent unit)
    {
        Units.Remove(unit);
    }

    public void RemoveUnits(List<Agent> units)
    {
        Units = Units.Except(units).ToList();
    }

    public void AddOrder(Order order)
    {
        if (!StandingOrders.Contains(order)) StandingOrders.Add(order);
    }

    public void RemoveOrder(Order order)
    {
        StandingOrders.Remove(order);
    }

    public void OnToggleOrderBuild()
    {
        BuildMode = !BuildMode;
    }

    public void OnToggleArtillery()
    {
        ArtilleryMode = !ArtilleryMode;
    }
}
