using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsNearBuilding : Conditional
{
    private CharacterMono thisMono;
    public SharedGameObject target;

    public override void OnAwake()
    {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate()
    {
        if(thisMono.bindingBuild == null) return TaskStatus.Failure;
        target.Value = thisMono.bindingBuild.gameObject;
        return TaskStatus.Success;
    }


}
