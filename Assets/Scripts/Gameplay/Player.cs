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

    [SerializeField] LayerMask layerMask;
    [HideInInspector] public List<Order> StandingOrders;
    [HideInInspector] public List<Agent> Units;

    public int PlayerID { get; }
    public int TeamID { get; }

    private void Start()
    {
        StandingOrders = new();
        Units = new();

        GameManager.Instance.players.Add(this);
    }

    public void OnIssueOrder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, layerMask))
        {
            GameManager.Instance.CreateOrder(hitInfo.point);
        }
    }

    public void AddUnit(Agent unit)
    {
        if (!Units.Contains(unit)) Units.Add(unit);
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
}