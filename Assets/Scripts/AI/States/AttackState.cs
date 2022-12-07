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
        owner.animator.SetTrigger("Fire");
        owner.timer.value = owner.attackDelay;

        owner.transform.LookAt(owner.enemy.transform);

        owner.enemy.GetComponent<Agent>().Damage(owner.attackDamage);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}
