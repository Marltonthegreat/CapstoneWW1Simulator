using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatState : State
{
    float angle;
    float distance;

    public RetreatState(Agent owner, string name) : base(owner, name)
    {

    }

    public override void OnEnter()
    {
        angle = owner.perception.angle;
        distance = owner.perception.distance;

        owner.perception.angle = 180;
        owner.perception.distance = 10;
    }

    public override void OnUpdate()
    {
        //Vector3 direction = (owner.transform.position - owner.enemy.transform.position).normalized; 

        //owner.movement.MoveTowards(owner.transform.position + direction);
    }

    public override void OnExit()
    {
        owner.perception.angle = angle;
        owner.perception.distance = distance;
    }
}
