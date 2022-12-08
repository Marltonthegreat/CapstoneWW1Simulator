using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [System.Serializable]
    public enum OrderType
    {
        Move,
        Attack,
        Retreat
    }

    public OrderType CurrentOrderType { get; private set; }
    private List<Player> players = new();
    [HideInInspector] public List<Order> AllStandingOrders = new();

    [SerializeField] SceneLoader sceneLoader;

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
                players[1].AddOrder(order);
                break;
            case OrderType.Attack:
                order = new AttackOrder("AttackOrder1", AttackMarkerPrefab, location, 0);
                players[1].AddOrder(order);
                break;
            case OrderType.Retreat:
                order = new RetreatOrder("RetreatOrder1", RetreatMarkerPrefab, location, 0);
                players[1].AddOrder(order);
                break;
            default:
                break;
        }

        if (order != null) AllStandingOrders.Add(order);
    }

    public void ChangeOrderType(string name)
    {
        if (CurrentOrderType.ToString().Equals(name)) return;

        OrderType type = CurrentOrderType;
        switch (name)
        {
            case "Attack":
                type = OrderType.Attack;
                break;
            case "Move":
                type = OrderType.Move;
                break;
            case "Retreat":
                type = OrderType.Retreat;
                break;
        }
        
        CurrentOrderType = type;
    }

    public void OnLoadScene(string sceneName)
    {
        sceneLoader.Load(sceneName);
    }

    public Player FindPlayerByID(int ID)
    {
        return players.FirstOrDefault(p => p.PlayerID == ID);
    }

    public void AddPlayer(Player player)
    {
        player.TeamID = player.PlayerID = players.Count();
        players.Add(player);
    }
}
