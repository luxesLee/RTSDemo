using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMVVM.Sources.Infrastructure;

/// <summary>
/// Player作为中介者模式的中介
/// </summary>
public class Player : MonoBehaviour
{
    public string userName;
    public bool isHuman = true;
    public SelectManager selectManager;

    // 玩家所能控制的所有单位
    public List<MonoBase> monoOfPlayer;
    // 维护玩家的对象池，对象池最大数目与maxHumanPopulation相关
    private MonoPool pool;

    private BindableProperty<int> gold = new BindableProperty<int>();
    private BindableProperty<int> wood = new BindableProperty<int>();
    private BindableProperty<int> humanPopulation = new BindableProperty<int>();
    private BindableProperty<int> maxHumanPopulation = new BindableProperty<int>();
    private BindableProperty<int> curSelectHp = new BindableProperty<int>();


    // ---------------------------------------------------------------------------------
    public BindableProperty<int>.ValueChangedHandler GoldValueChangedHandler {
        get {
            return gold.OnValueChanged;
        }
        set {
            gold.OnValueChanged = value;
        }
    }

    public BindableProperty<int>.ValueChangedHandler WoodValueChangedHandler {
        get {
            return wood.OnValueChanged;
        }
        set {
            wood.OnValueChanged = value;
        }
    }

    public BindableProperty<int>.ValueChangedHandler HumanPopulationValueChangedHandler {
        get {
            return humanPopulation.OnValueChanged;
        }
        set {
            humanPopulation.OnValueChanged = value;
        }
    }

    public BindableProperty<int>.ValueChangedHandler MaxHumanPopulationValueChangedHandler {
        get {
            return maxHumanPopulation.OnValueChanged;
        }
        set {
            maxHumanPopulation.OnValueChanged = value;
        }
    }



    private void Awake() {

        // Player下已有的mono的准备
        MonoBase[] monos = GetComponentsInChildren<MonoBase>();
        foreach(var mono in monos) {
            monoOfPlayer.Add(mono);
        }
    }


    private void Start() {
        Init();
        pool = new MonoPool(maxHumanPopulation.Value);
    }



    private void Init() {

        selectManager = FindObjectOfType<SelectManager>();
        selectManager.player = this;

        gold.Value = 0;
        wood.Value = 0;
        humanPopulation.Value = 0;
        maxHumanPopulation.Value = 10;
    }

    /// <summary>
    /// 从对象池生产新的单位
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="qua"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public MonoBase AddNewUnit(Vector3 pos, Quaternion qua, GameObject template) {
        GameObject newUnit = pool.GetGameObjectInPool(pos, qua, template);
        if(newUnit != null) {
            newUnit.transform.SetParent(this.transform);
            MonoBase newMono = newUnit.GetComponent<MonoBase>();
            monoOfPlayer.Add(newMono);
            newMono.player = this;
            return newMono;
        }
        else return null;
    }

    /// <summary>
    /// 增加Player的资源，以ChacterMono背负的资源作为增加量
    /// </summary>
    public void AddResources(CharacterMono characterMono) {
        
        OwnedResource resource = characterMono.ItsResource;
        characterMono.RemoveResources();
        if(resource.resourceType == Resource.gold) {
            gold.Value += resource.num;
        }
        else if(resource.resourceType == Resource.wood) {
            wood.Value += resource.num;
        }

    }
}
