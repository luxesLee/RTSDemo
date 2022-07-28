using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据管理类，主要负责游戏开始时读取内置游戏数据
/// 是玩家不可改变的那一类数据
/// </summary>
public class DataManager
{
    private static DataManager instance;
    public static DataManager Instance {
        get {
            if(instance == null) {
                instance = new DataManager();
            }
            return instance;
        }
    }

    public DataManager() {

    }

    // model数据存储的集合，model依据此初始化

    // model数据存储的路径
    private string modelPath;



}
