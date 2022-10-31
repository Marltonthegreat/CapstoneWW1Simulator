using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum OrderType
    {
        Move,
        Attack,
        Retreat
    }

    public OrderType CurrentOrderType { get; private set; }
    [HideInInspector] public List<Player> players = new();
    [HideInInspector] public List<Order> AllStandingOrders = new();

    [SerializeField] GameObject MoveMarkerPrefab;
    [SerializeField] GameObject AttackMarkerPrefab;
    [SerializeField] GameObject RetreatMarkerPrefab;

    public void CreateOrder(Vector3 location)
    {
        Order order = null;

        switch (CurrentOrderType)
        {
            case OrderType.Move:
                order = new MoveOrder("MoveOrder1", MoveMarkerPrefab, location, 0);
                players[0].AddOrder(order);
                break;
            case OrderType.Attack:
                order = new AttackOrder("AttackOrder1", AttackMarkerPrefab, location, 0);
                players[0].AddOrder(order);
                break;
            case OrderType.Retreat:
                order = new RetreatOrder("RetreatOrder1", RetreatMarkerPrefab, location, 0);
                players[0].AddOrder(order);
                break;
            default:
                break;
        }

        if (order != null) AllStandingOrders.Add(order);
    }

    public void ChangeOrderType(OrderType type)
    {
        CurrentOrderType = type;
    }

    public Player FindPlayerByID(int ID)
    {
        return players.FirstOrDefault(p => p.PlayerID == ID);
    }
}
