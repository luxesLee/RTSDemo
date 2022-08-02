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
        name = DataBase.Instance.modelDataBase["solider"]["name"].stringValue;
        isHero = DataBase.Instance.modelDataBase["solider"]["isHero"].boolValue;
        hp = DataBase.Instance.modelDataBase["solider"]["hp"].intValue;
        mp = DataBase.Instance.modelDataBase["solider"]["mp"].intValue;
        attackPower = DataBase.Instance.modelDataBase["solider"]["attackpower"].intValue;
        defensePower = DataBase.Instance.modelDataBase["solider"]["defensepower"].intValue;
        detectLength = DataBase.Instance.modelDataBase["solider"]["detectLength"].intValue;
        expGiven = DataBase.Instance.modelDataBase["solider"]["expGiven"].intValue;
        goldGiven = DataBase.Instance.modelDataBase["solider"]["goldGiven"].intValue;
        power = DataBase.Instance.modelDataBase["solider"]["power"].intValue;
        agile = DataBase.Instance.modelDataBase["solider"]["agile"].intValue;
        intelligence = DataBase.Instance.modelDataBase["solider"]["intelligence"].intValue;
        moveSpeed = DataBase.Instance.modelDataBase["solider"]["moveSpeed"].floatValue;
        turnAroundSpeed = DataBase.Instance.modelDataBase["solider"]["turnAroundSpeed"].floatValue;
    }

    public int power; // 力量
    public int agile; // 敏捷
    public int intelligence;  // 智力
    public float moveSpeed; // 移动速度
    public float turnAroundSpeed;
}
