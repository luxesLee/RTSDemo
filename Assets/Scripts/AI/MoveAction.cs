using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MoveAction : Action
{
    private CharacterMono thisMono;
    public SharedGameObject target;

    public override void OnAwake()
    {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate()
    {
        if(thisMono.IsOperateByPlayer) return TaskStatus.Failure;

        thisMono.Move(target.Value.gameObject.transform.position);
        thisMono.ChangeDirection(target.Value);
        if(thisMono.EndMove() && thisMono.IsForwardToTarget(target.Value)) {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

}
