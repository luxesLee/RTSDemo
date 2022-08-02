using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uMVVM.Sources.Infrastructure;

/// <summary>
/// 所有Mono的基类，用于存放一些通性
/// </summary>
public class MonoBase : MonoBehaviour
{
    // 与Player双向持有
    public Player player;
    public int MaxHp;
    public BindableProperty<int> curHP = new BindableProperty<int>();
    public BindableProperty<bool> isDying = new BindableProperty<bool>();

    // 表示每个mono的类型
    protected MonoEnum monotype;
    public MonoEnum MonoType {
        get {
            return monotype;
        }
    }

    private Image curHPImage;   // 血条

    public SelectableCharacter selectableCharacter;
    // 是否由玩家选中
    protected bool isOperateByPlayer;
    public bool IsOperateByPlayer {
        get {
            return isOperateByPlayer;
        }
    }

    public List<MonoBase> arroundEnemies;

    public BindableProperty<int>.ValueChangedHandler CurHpChangedHandler {
        get {
            return curHP.OnValueChanged;
        }
        set {
            curHP.OnValueChanged = value;
        }
    }
    
    public BindableProperty<bool>.ValueChangedHandler IsDiedHandler {
        get {
            return isDying.OnValueChanged;
        }
        set {
            isDying.OnValueChanged = value;
        }
    }







    public virtual void Awake() {
        selectableCharacter = GetComponentInChildren<SelectableCharacter>();

        curHPImage = transform.Find("HP/HPHandle/curHP").GetComponent<Image>();
        CurHpChangedHandler += OnCurHpChanged;
        IsDiedHandler += OnisDyingChanged;
    }

    public virtual void Update() {

    }


    /// <summary>
    /// 每120帧做一次周围敌人检测
    /// </summary>
    protected void DetectArroundEnemies() {
        if(Time.frameCount % 120 == 0) {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5, ~(1 << 7 | 1 << 8));
            foreach(var col in colliders) {
                MonoBase enemyMono = col.gameObject.GetComponent<MonoBase>();
                if(enemyMono.player == player) continue;
                if(enemyMono != this && !arroundEnemies.Contains(enemyMono)) {
                    arroundEnemies.Add(enemyMono);
                }
            }
        }
    }

    private void OnCurHpChanged(int oldVal, int newVal) {
        if(newVal < 0) curHP.Value = 0;
        curHPImage.fillAmount = (float)curHP.Value / MaxHp;
        if(curHP.Value == 0) {
            isDying.Value = true;
        }
    }

    protected virtual void OnisDyingChanged(bool oldVal, bool newVal) {
        Debug.Log("dieeeeeeeeeee");
    }

    public virtual void TurnOnOperateByPlayer() {

    }

    public virtual void TurnOffOperateByPlayer() {

    }
}
