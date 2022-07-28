using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uMVVM.Sources.Infrastructure;

public class AvatarView : MonoBehaviour
{
    public Text gold;
    public Text wood;
    public Text human;
    public Text maxHuman;
    public Player player;

    private void Start() {
        Bind();
    }

    public void Bind() {
        player.GoldValueChangedHandler += OnGoldChanged;
        player.WoodValueChangedHandler += OnWoodChanged;
        player.HumanPopulationValueChangedHandler += OnHumanChanged;
        player.MaxHumanPopulationValueChangedHandler += OnMaxHumanChanged;
    }

    public void OnGoldChanged(int oldValue, int newValue) {
        gold.text = "gold:" + newValue.ToString();
    }

    public void OnWoodChanged(int oldValue, int newValue) {
        wood.text = "wood:" + newValue.ToString();
    }

    public void OnHumanChanged(int oldValue, int newValue) {
        human.text = "human:" + newValue.ToString();
    }

    public void OnMaxHumanChanged(int oldValue, int newValue) {
        maxHuman.text = "maxHuman:" + newValue.ToString();
    }
}
