using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource {
    uninitial,
    gold,
    wood
}

public class OwnedResource
{
    public int num;
    public Resource resourceType;
    public OwnedResource() {
        num = 0;
        resourceType = Resource.uninitial;
    }
    public OwnedResource(int num, Resource type) {
        this.num = num;
        this.resourceType = type;
    }
}
