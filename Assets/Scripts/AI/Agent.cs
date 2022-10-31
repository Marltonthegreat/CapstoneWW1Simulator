using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
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

    public int PlayerID { get; }
    public int TeamID { get; }

    public GameObject enemy { get; set; }

    public Order CurrentOrder;

    private void Start()
    {
        stateMachine.AddState(new IdleState(this, typeof(IdleState).Name));
        stateMachine.AddState(new OrderState(this, typeof(OrderState).Name));
        //stateMachine.AddState(new PatrolState(this, typeof(PatrolState).Name));
        stateMachine.AddState(new ChaseState(this, typeof(ChaseState).Name));
        stateMachine.AddState(new DeathState(this, typeof(DeathState).Name));
        stateMachine.AddState(new AttackState(this, typeof(AttackState).Name));
        stateMachine.AddState(new RetreatState(this, typeof(RetreatState).Name));

        //IdleState transitions
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, criticalHealth) }), typeof(RetreatState).Name);
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.GREATER, criticalHealth) }), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new BoolCondition(hasOrder, true), new FloatCondition(health, Condition.Predicate.GREATER, criticalHealth) }), typeof(OrderState).Name);

        //stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.GREATER, 30) }), typeof(ChaseState).Name);
        //stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 30) }), typeof(RetreatState).Name);
        //stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, false) }), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(enemyDistance, Condition.Predicate.LESS_EQUAL, attackDistance), new FloatCondition(enemyHealth, Condition.Predicate.GREATER, 0) }), typeof(AttackState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, criticalHealth) }), typeof(RetreatState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS_EQUAL, 0) }), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, criticalHealth) }), typeof(RetreatState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(RetreatState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(RetreatState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);

        stateMachine.SetState(stateMachine.StateFromName(typeof(IdleState).Name));
    }

    void Update()
    {
        // Update parameters

        // Enemy
        var enemies = perception.GetGameObjects();
        enemySeen.value = enemies.Length != 0;
        enemy = (enemies.Length != 0) ? enemies[0] : null;
        enemyHealth.value = (enemy != null) ? (enemy.GetComponent<Agent>().health) : 0f;
        enemyDistance.value = (enemy != null) ? (Vector3.Distance(transform.position, enemy.transform.position)) : float.MaxValue;

        if (!hasOrder) GetOrder();

        timer.value -= Time.deltaTime;

        stateMachine.Update();

        animator.SetFloat("speed", movement.velocity.magnitude);
    }

    public void GetOrder()
    {
        CurrentOrder = GameManager.Instance.FindPlayerByID(PlayerID).StandingOrders.Aggregate((so1, so2) => ((so1.Location - transform.position).sqrMagnitude > (so2.Location - transform.position).sqrMagnitude) ? so1 : so2);

        if (CurrentOrder != null) hasOrder.value = true;
    }

    private void OnGUI()
    {
        Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);

        GUI.Label(new Rect(screen.x, Screen.height - screen.y, 300, 20), stateMachine.GetStateName());
        GUI.Label(new Rect(screen.x, Screen.height - screen.y - 10, 300, 20), $"health: {health.value}");
    }

    public static Agent[] GetAgents()
    {
        return FindObjectsOfType<Agent>();
    }
}
