using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMVVM.Sources.Infrastructure;

/// <summary>
/// 所有Mono的基类，用于存放一些通性
/// </summary>
public class MonoBase : MonoBehaviour
{
    // 与Player双向持有
    public Player player;
    public int MaxHp;
    public int curHP;
    // 每个Mono中保存一个UI
    public GameObject panel;

    // 表示每个mono的类型
    protected MonoEnum monotype;
    public MonoEnum MonoType {
        get {
            return monotype;
        }
    }

    public SelectableCharacter selectableCharacter;
    // 是否由玩家选中
    protected bool isOperateByPlayer;
    public bool IsOperateByPlayer {
        get {
            return isOperateByPlayer;
        }
    }

    public List<MonoBase> arroundEnemies;
    
    private float detectLength = 10f;
    
    public virtual void Awake() {
        selectableCharacter = GetComponentInChildren<SelectableCharacter>();
    }

    private void Update() {
        
    }


    /// <summary>
    /// 每120帧做一次周围敌人检测
    /// </summary>
    protected void DetectArroundEnemies() {
        if(Time.frameCount % 120 == 0) {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectLength, ~(1 << 7 | 1 << 8));
            foreach(var col in colliders) {
                MonoBase enemyMono = col.gameObject.GetComponent<MonoBase>();
                if(enemyMono.player == player) continue;
                if(enemyMono != this && !arroundEnemies.Contains(enemyMono)) {
                    arroundEnemies.Add(enemyMono);
                }
            }
        }
    }

    public virtual void TurnOnOperateByPlayer() {

    }

    public virtual void TurnOffOperateByPlayer() {

    }
}
