using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBarControl : MonoBehaviour
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

    private void Update() {
        UpdateBloodBar();
    }


    /// <summary>
    /// 更新血条
    /// 后面应该写成回调函数
    /// </summary>
    private void UpdateBloodBar() {
        float x = (thisMono.curHP / thisMono.MaxHp - 1) * 5;
        rectTransform.offsetMax = new Vector2(x, rectTransform.offsetMax.y);
    }

    /// <summary>
    /// 初始化血条长度与最大生命值有关
    /// </summary>
    private void SuiteBloodBar() {
        float scale = Mathf.Log10(thisMono.MaxHp/100);

        rectTransform.localScale = new Vector3(rectTransform.localScale.x * scale, 
            rectTransform.localScale.y, rectTransform.localScale.z);
    }

}
