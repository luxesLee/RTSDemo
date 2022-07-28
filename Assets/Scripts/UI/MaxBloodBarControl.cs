using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxBloodBarControl : MonoBehaviour
{
    public RectTransform rectTransform;
    public MonoBase thisMono;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        thisMono = GetComponentInParent<MonoBase>();
    }

    private void Start() {
        SuiteBloodBar();
    }

    private void SuiteBloodBar() {
        float scale = Mathf.Log10(thisMono.MaxHp/100);

        rectTransform.localScale = new Vector3(rectTransform.localScale.x * scale, 
            rectTransform.localScale.y, rectTransform.localScale.z);
    }
}
