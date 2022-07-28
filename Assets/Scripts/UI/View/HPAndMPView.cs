using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uMVVM.Sources.Infrastructure;

public class HPAndMPView : UnityGuiView<HPAndMPViewModel>
{
    public Text HP;
    public Text MP;
    public Text Name;



    private void Start() {
        BindingContext = new HPAndMPViewModel();
    }



    protected override void OnInitialize()
    {
        base.OnInitialize();
        Binder.Add<int>("HP", OnHPChanged);
        Binder.Add<int>("MP", OnMPChanged);
        Binder.Add<string>("Name", OnNameChanged);
    }

    public void OnHPChanged(int oldValue, int newValue) {
        HP.text = newValue.ToString();
    }

    public void OnMPChanged(int oldValue, int newValue) {
        MP.text = newValue.ToString();
    }

    public void OnNameChanged(string oldValue, string newValue) {
        Name.text = newValue;
    }
    

}
