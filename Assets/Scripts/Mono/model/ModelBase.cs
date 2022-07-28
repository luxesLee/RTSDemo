using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有Model的基类
/// 用于存放单位共性，例如基础攻击力，基础生命等
/// 这些内容对于单一兵种从文件中读取后保持不变，因此利用享元模式
/// 单例类，系统中只保留一份供读取
/// </summary>
public class ModelBase
{
    private static ModelBase instance;
    public static ModelBase Instance {
        get {
            if(instance == null) 
                instance = new ModelBase();
            return instance;
        }
    }

    public ModelBase() {

    }

    // 根据modelType类型赋予以下属性
    public ModelEnum modelType;

    // hp
    public float hp { get; }

    // mp
    public float mp { get; }

    // attack power
    public float attackPower { get; }

    // defense power
    public float defensePower { get; }

    // detect length
    public float detectLength { get; }








}