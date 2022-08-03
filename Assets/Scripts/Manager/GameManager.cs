using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    

    private DataBase dataBase;

    private GameObject playerPrefab;

    private void Awake() {
        playerPrefab = Resources.Load("Prefabs/Player") as GameObject;

        dataBase = new DataBase();
    }

    private void Start() {
        

        Instantiate(playerPrefab, new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100)), Quaternion.identity);
        
        
    }

    private void Update() {
        
    }

    
}
