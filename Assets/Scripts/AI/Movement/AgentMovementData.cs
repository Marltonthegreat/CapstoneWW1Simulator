using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentMovementData", menuName = "AI/MovementData")]
public class AgentMovementData : ScriptableObject
{
    [Range(0,20)] public float maxSpeed = 12;
    [Range(0,20)] public float minSpeed = 6;
    [Range(0,20)] public float maxForce = 8;
    [Range(0,20)] public float turnRate = 360;

    public bool orientToMovement = true;
}
