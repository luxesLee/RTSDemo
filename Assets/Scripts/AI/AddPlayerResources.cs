using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


public class AddPlayerResources : Action
{
    private CharacterMono thisMono;

    public override void OnAwake()
    {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate()
    {
        thisMono.player.AddResources(thisMono);
        
        return TaskStatus.Success;
    }



}
