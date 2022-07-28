using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackAction : Action
{
    private CharacterMono thisMono;
    public SharedGameObject Enemy;

    public override void OnAwake()
    {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate()
    {
        if(thisMono.IsOperateByPlayer) return TaskStatus.Failure;

        if(thisMono.Attack(Enemy.Value.GetComponent<CharacterMono>())) {
            return TaskStatus.Success;
        }
        else {
            return TaskStatus.Running;
        }

    }


}
