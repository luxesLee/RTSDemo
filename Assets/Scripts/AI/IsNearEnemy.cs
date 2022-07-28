using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsNearEnemy : Conditional
{
    private CharacterMono thisMono;
    public SharedGameObject Enemy;
    
    public override void OnAwake() {
        thisMono = GetComponent<CharacterMono>();
    }

    public override TaskStatus OnUpdate()
    {
        if(thisMono.arroundEnemies.Count > 0) {
            Enemy.SetValue(SelectEnemy());
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    private GameObject SelectEnemy() {
        MonoBase enemy = thisMono.arroundEnemies[0];
        float mindistance = float.MaxValue;
        foreach(MonoBase target in thisMono.arroundEnemies) {
            float distance = Vector3.Distance(thisMono.transform.position, target.transform.position);
            if(distance < mindistance) {
                mindistance = distance;
                enemy = target;
            }
        }
        return enemy.gameObject;
    }


}

