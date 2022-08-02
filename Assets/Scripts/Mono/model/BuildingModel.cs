using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModel : ModelBase
{
    private static BuildingModel instance;
    public new static BuildingModel Instance {
        get {
            if(instance == null) {
                instance = new BuildingModel();
            }
            return instance;
        }
    }

    public BuildingModel() {
        name = DataBase.Instance.modelDataBase["building"]["name"].stringValue;
        isHero = DataBase.Instance.modelDataBase["building"]["isHera"].boolValue;
        hp = DataBase.Instance.modelDataBase["building"]["hp"].intValue;
        mp = DataBase.Instance.modelDataBase["building"]["mp"].intValue;
        attackPower = DataBase.Instance.modelDataBase["building"]["attackPower"].intValue;
        defensePower = DataBase.Instance.modelDataBase["building"]["defensePower"].intValue;
        detectLength = DataBase.Instance.modelDataBase["building"]["detectLength"].intValue;
        expGiven = DataBase.Instance.modelDataBase["building"]["expGiven"].intValue;
        goldGiven = DataBase.Instance.modelDataBase["building"]["goldGiven"].intValue;
        productTime = DataBase.Instance.modelDataBase["building"]["productTime"].floatValue;
    }


    public float productTime;


}
