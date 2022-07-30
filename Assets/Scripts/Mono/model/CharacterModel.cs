using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : ModelBase
{
    private static CharacterModel instance;
    public static new CharacterModel Instance {
        get {
            if(instance == null) {
                instance = new CharacterModel();
            }
            return instance;
        }
    }

    public CharacterModel() {
        // name = DataBase.Instance.modelDataBase[""]
    }

    public float power; // 力量
    public float agile; // 敏捷
    public float intelligence;  // 智力
    public float moveSpeed; // 移动速度
    public float turnAroundSpeed;
}
