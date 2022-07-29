using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PickupResources : Action
{
    private CharacterMono thisMono;
    public SharedGameObject resourceGO;

    public override void OnAwake()
    {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate()
    {
        thisMono.SetResources(resourceGO.Value.GetComponent<ResourceBase>().GetResource());
        
        return TaskStatus.Success;
    }

}
