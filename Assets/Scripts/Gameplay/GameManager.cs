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
    public List<Player> players = new();
    public List<Order> AllStandingOrders = new();

    public void CreateOrder(Vector3 location)
    {
        Order order = null;

        switch (CurrentOrderType)
        {
            case OrderType.Move:
                order = new MoveOrder("MoveOrder1", null, location, 0);
                players[0].AddOrder(order);
                break;
            case OrderType.Attack:
                order = new AttackOrder("AttackOrder1", null, location, 0);
                players[0].AddOrder(order);
                break;
            case OrderType.Retreat:
                order = new RetreatOrder("RetreatOrder1", null, location, 0);
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
}
