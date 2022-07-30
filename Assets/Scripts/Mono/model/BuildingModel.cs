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

    public float productTime;


}
