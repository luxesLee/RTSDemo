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

    public string name;
    public bool isHero; // 是否英雄单位
    public int hp;
    public int mp;
    public int attackPower;
    public int defensePower;
    public int detectLength;

    public int expGiven;  // 被杀经验值
    public int goldGiven;   // 被杀金币







}
