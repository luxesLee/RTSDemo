using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance {
        get {
            if(instance == null) {
                instance = new UIManager();
            }
            return instance;
        }
    }
    public Player player;

    private Text Gold;  // 金币
    private Text Wood;  // 木头
    private Text Food;  // 人口

    private void Awake() {
        Gold = transform.Find("top/Gold").GetComponent<Text>();
        Wood = transform.Find("top/Wood").GetComponent<Text>();
        Food = transform.Find("top/Food").GetComponent<Text>();

        player.GoldValueChangedHandler += OnGoldChanged;

        
    }

    private void OnGoldChanged(int oldVal, int newVal) {
        Gold.text = newVal.ToString();
    }

    private void OnWoodChanged(int oldVal, int newVal) {
        Wood.text = newVal.ToString();
    }

    private void OnFoodChanged(int oldVal, int newVal) {
        
    }

}
