using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public AttackState(Agent owner, string name) : base(owner, name)
    {

    }

    public override void OnEnter()
    {
        owner.movement.Stop();
        owner.animator.SetTrigger("attack");
        owner.timer.value = owner.attackDelay;

        //owner.GetComponent<AgentDamage>().Damage();
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}
