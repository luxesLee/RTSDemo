using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsNearResource : Conditional
{
    private CharacterMono thisMono;
    public SharedGameObject target;


    public override void OnAwake() {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate() {
        if(thisMono.resourceBase == null) {
            return TaskStatus.Failure;
        }
        target.Value = thisMono.resourceBase.gameObject;
        return TaskStatus.Success;
    }
    
}
