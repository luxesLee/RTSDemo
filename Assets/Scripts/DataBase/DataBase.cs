using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

/// <summary>
/// 数据库，主要负责游戏开始时读取内置游戏数据
/// 是玩家不可改变的那一类数据
/// </summary>
public class DataBase
{
    private static DataBase instance;
    public static DataBase Instance {
        get {
            if(instance == null) {
                instance = new DataBase();
            }
            return instance;
        }
    }

    // model数据存储的路径
    private string modelPath = "Data/Model";
    public readonly JSONObject modelDataBase;

    public DataBase() {
        TextAsset modelContent = Resources.Load(modelPath) as TextAsset;
        modelDataBase = new JSONObject(modelContent.text);
    }






}
