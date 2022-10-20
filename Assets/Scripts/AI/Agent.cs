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
    public FloatRef enemyDistance;
    public FloatRef health;
    public FloatRef enemyHealth;
    public FloatRef timer;

    public float attackDistance = 5;
    public float attackDelay = 5;

    public GameObject enemy { get; set; }

    private void Start()
    {
        stateMachine.AddState(new IdleState(this, typeof(IdleState).Name));
        stateMachine.AddState(new PatrolState(this, typeof(PatrolState).Name));
    }

    public static Agent[] GetAgents()
    {
        return FindObjectsOfType<Agent>();
    }
}
