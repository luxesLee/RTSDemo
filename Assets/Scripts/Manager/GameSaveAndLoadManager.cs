using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameSaveAndLoadManager : MonoBehaviour
{
    public void SaveData() {

        Debug.Log(Application.persistentDataPath);

        if(!Directory.Exists(Application.persistentDataPath + "/SaveData")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveData");
        }

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream fs = File.Create(Application.persistentDataPath + "/SaveData/" + Time.time + ".txt");


        Player[] players = Transform.FindObjectsOfType<Player>();
        foreach(var player in players) {
            // 保存每个player中的数据

            
        }

        // 传入数组会失效
        // string json = JsonUtility.ToJson();

        // Debug.Log(json);

        // formatter.Serialize(fs, json);

        fs.Close();
    }

    public void LoadData() {

    }

}
