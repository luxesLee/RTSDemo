using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PickupResources : Action
{
    private CharacterMono thisMono;
    private float beginTime;
    public GameObject resourceGO;

    public override void OnAwake()
    {
        thisMono = GetComponent<CharacterMono>();
        beginTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
        if(Time.time - beginTime < thisMono.PickupSpeed) return TaskStatus.Running;

        thisMono.SetResources(resourceGO.GetComponent<ResourceBase>().GetResource());
        
        return TaskStatus.Success;
    }


}
