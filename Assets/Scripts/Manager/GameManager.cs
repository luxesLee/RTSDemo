using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private DataBase dataBase;

    private void Awake() {
        
    }

    private void Start() {
        dataBase = new DataBase();
    }

    
}
