using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMVVM.Sources.Infrastructure;

/// <summary>
/// 一个地区携带的资源数量
/// </summary>
public class ResourceBase : MonoBehaviour
{

    public OwnedResource resource;
    public BindableProperty<int> num = new BindableProperty<int>();
    public BindableProperty<int>.ValueChangedHandler numChangeHandler {
        get {
            return num.OnValueChanged;
        }
        set {
            num.OnValueChanged = value;
        }
    }


    private void Awake() {
        resource = new OwnedResource(50, Resource.gold);
        num.Value = resource.num;
        numChangeHandler += OnNumChange;
    }


    public OwnedResource GetResource() {
        if(resource.num > 10) {
            resource.num -= 10;
            return new OwnedResource(10, resource.resourceType);
        }
        int val = resource.num;
        resource.num = 0;
        return new OwnedResource(val, resource.resourceType);
    }

    private void OnNumChange(int oldvalue, int newvalue) {
        if(newvalue == 0) {
            Destroy(this.gameObject);
        }
    }

}
