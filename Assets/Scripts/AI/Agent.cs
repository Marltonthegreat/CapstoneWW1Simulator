using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private struct Squad
    {
        public Agent squadLeader;
        public Agent[] subordinates;

        //public delegate void 

        public delegate void IssueOrder(Order order);
        public event IssueOrder issueOrder;
        
        public Order standingOrder;

        public int subCount;

        public Squad(Agent squadLeader)
        {
            this.squadLeader = squadLeader;
            subordinates = new Agent[4];
            issueOrder = null;
            standingOrder = null;
            subCount = 0;
        }

        public void IssueOrderToMembers(Order order)
        {
            standingOrder = order;
            issueOrder(order);
        }
    }

    [SerializeField] AudioClip clip;

    public AgentMovement movement;
    public Animator animator;

    public Perception perception;

    public PathFollower pathFollower;
    public StateMachine stateMachine = new StateMachine();

    public BoolRef enemySeen;
    public BoolRef atDestination;
    public BoolRef hasOrder;
    public FloatRef enemyDistance;
    public FloatRef health;
    public FloatRef criticalHealth;
    public FloatRef enemyHealth;
    public FloatRef timer;

    public float attackDistance = 5;
    public float attackDelay = 5;
    public float attackDamage = 5;

    public int PlayerID;
    public int TeamID;

    public GameObject enemy { get; set; }

    private Squad squad;
    public Order CurrentOrder;
    public List<Order> CompletedOrders = new();

    private void Start()
    {
        InstantiateAgent();
    }

    void Update()
    {
        // Update parameters

        // Enemy
        var enemies = perception.GetGameObjects();
        if (enemies.Length != 0) enemies = enemies.Where(e => e.GetComponent<Agent>().TeamID != TeamID).ToArray();

        enemySeen.value = enemies.Length != 0;
        enemy = (enemies.Length != 0) ? enemies[0] : null;
        enemyHealth.value = (enemy != null) ? (enemy.GetComponent<Agent>().health) : 0f;
        enemyDistance.value = (enemy != null) ? (Vector3.Distance(transform.position, enemy.transform.position)) : float.MaxValue;

        //get order
        if (!hasOrder && squad.squadLeader != null && squad.squadLeader.Equals(this)) GetOrder();
        else if (atDestination) CompleteOrder();

        timer.value -= Time.deltaTime;

        stateMachine.Update();

        animator.SetFloat("speed", movement.velocity.magnitude);
    }

    private void InstantiateAgent()
    {
        InstantiateStates();
        InstantiateTransitions();

        SetSquadLeader();
        stateMachine.SetState(stateMachine.StateFromName(typeof(IdleState).Name));
    }

    private void InstantiateStates()
    {
        stateMachine.AddState(new IdleState(this, typeof(IdleState).Name));
        stateMachine.AddState(new OrderState(this, typeof(OrderState).Name));
        //stateMachine.AddState(new PatrolState(this, typeof(PatrolState).Name));
        stateMachine.AddState(new ChaseState(this, typeof(ChaseState).Name));
        stateMachine.AddState(new DeathState(this, typeof(DeathState).Name));
        stateMachine.AddState(new AttackState(this, typeof(AttackState).Name));
        stateMachine.AddState(new RetreatState(this, typeof(RetreatState).Name));
    }

    private void InstantiateTransitions()
    {
        //IdleState transitions
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, criticalHealth) }), typeof(RetreatState).Name);
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.GREATER, criticalHealth) }), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new BoolCondition(hasOrder, true), new FloatCondition(health, Condition.Predicate.GREATER, criticalHealth) }), typeof(OrderState).Name);

        stateMachine.AddTransition(typeof(OrderState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS, criticalHealth) }), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(OrderState).Name, new Transition(new Condition[] { new BoolCondition(hasOrder, false) }), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(OrderState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.GREATER, criticalHealth) }), typeof(ChaseState).Name);
        //stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 30) }), typeof(RetreatState).Name);
        //stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition
        (
            new Condition[] { new BoolCondition(enemySeen, false), new BoolCondition(hasOrder, false) }
        ), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition
        (
            new Condition[] { new BoolCondition(enemySeen, false), new BoolCondition(hasOrder, true) }
        ), typeof(OrderState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition
        (
            new Condition[] { new FloatCondition(enemyDistance, Condition.Predicate.LESS_EQUAL, attackDistance), new FloatCondition(enemyHealth, Condition.Predicate.GREATER, 0) }
        ), typeof(AttackState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition
        (
            new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, criticalHealth) }
        ), typeof(RetreatState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition
        (
            new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }
        ), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(AttackState).Name, new Transition
        (
            new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS_EQUAL, 0) }
        ), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition
        (
            new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, criticalHealth) }
        ), typeof(RetreatState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition
        (
            new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }
        ), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(RetreatState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.GREATER, criticalHealth) }), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(RetreatState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);
    }

    public void GetOrder()
    {

        var player = GameManager.Instance.FindPlayerByID(PlayerID);
        if (player == null) return;

        var playerOrders = player.StandingOrders;

        playerOrders = playerOrders.Where(o => !CompletedOrders.Contains(o)).ToList();
        if (playerOrders.Count == 0) return;

        var order = playerOrders.Aggregate((so1, so2) => ((so1.Location - transform.position).sqrMagnitude < (so2.Location - transform.position).sqrMagnitude) ? so1 : so2);

        squad.IssueOrderToMembers(order);
    }

    public void SetOrder(Order order)
    {
        stateMachine.SetState(stateMachine.StateFromName(typeof(IdleState).Name));

        CurrentOrder = order;

        if (CurrentOrder != null)
        {
            hasOrder.value = true;
            stateMachine.StateFromType<OrderState>().destination = CurrentOrder.Location;
        }
    }

    public void CompleteOrder()
    {
        atDestination.value = false;
        hasOrder.value = false;
        stateMachine.StateFromType<OrderState>().destination = null;
        CompletedOrders.Add(CurrentOrder);
        CurrentOrder = null;
    }

    public void SetSquadLeader()
    {
        var units = GameManager.Instance.FindPlayerByID(PlayerID).Units;
        var leader = units.FirstOrDefault(u => u.squad.squadLeader != null && u.squad.squadLeader.Equals(u) && u.squad.subordinates[3] != null);

        if (leader == null)
        {
            squad = new Squad(this);
            squad.issueOrder += SetOrder;
        }
        else
        {
            leader.squad.subordinates[leader.squad.subCount++] = this;
            leader.squad.issueOrder += SetOrder;
            squad = leader.squad;

            SetOrder(squad.standingOrder);
        }
    }

    //private void OnGUI()
    //{
    //    Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);

    //    GUI.Label(new Rect(screen.x, Screen.height - screen.y, 300, 20), stateMachine.GetStateName());
    //    //    GUI.Label(new Rect(screen.x, Screen.height - screen.y - 10, 300, 20), $"Has Order: {hasOrder.value}");
    //    //    GUI.Label(new Rect(screen.x, Screen.height - screen.y - 20, 300, 20), $"health: {health.value}");
    //}

    public void Damage(float damage)
    {
        health.value -= damage;
    }

    public void PlayShootSFX()
    {
        AudioManager.Instance.PlaySFX(clip);
    }

    public static Agent[] GetAgents()
    {
        return FindObjectsOfType<Agent>();
    }
}
