using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制类
/// 用于控制玩家的操作逻辑
/// </summary>
public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<PlayerController>();
            }
            return instance;
        }
    }
    private Player player;
    public GameObject panelOfSystem;
    private float nextPressEsc = 0;

    private SelectManager selectManager;

    // Start is called before the first frame update
    void Start()
    {
        selectManager = FindObjectOfType<SelectManager>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isHuman) {
            RightMouseControl();
        }

        // 可以换成Unity的InputSystem
        if(Input.GetKey(KeyCode.Escape) && Time.time - nextPressEsc > 0.1f) {
            panelOfSystem.SetActive(!panelOfSystem.activeInHierarchy);
            nextPressEsc = Time.time;
        }


    }


    /// <summary>
    /// 鼠标右键的相关逻辑
    /// </summary>
    private void RightMouseControl() {
        if(Input.GetMouseButtonDown(1) && selectManager.holdonCharacter.Count > 0) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 200f)) {

                // 这里应该根据hit目标的layer进行分类

                if(hit.collider.gameObject.layer == 8) {
                    PickUpInitial(hit.collider.gameObject);
                }
                else if(hit.collider.gameObject.layer == 7) {
                    MoveChacterOrAssemblePoint(hit.point);
                }
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    private void MoveChacterOrAssemblePoint(Vector3 point) {
        foreach(var selectmono in selectManager.selectMono) {
            if(selectmono.MonoType == MonoEnum.character) {
                CharacterMono characterMono = selectmono as CharacterMono;

                if(characterMono.BTree.ExternalBehavior != characterMono.solider)
                    characterMono.SwitchBehaviourScript();

                characterMono.Move(point + new Vector3(0, 1, 0));
            }
            else if(selectmono.MonoType == MonoEnum.building){
                BuildingMono buildingMono = selectmono as BuildingMono;
                buildingMono.SetAssemblePoint(point);
            }
        }
    }


    /// <summary>
    /// 切换chactermono至采集模式
    /// </summary>
    /// <param name="hitGO"></param>
    private void PickUpInitial(GameObject hitGO) {
        foreach(var selectmono in selectManager.selectMono) {
            if(selectmono.MonoType == MonoEnum.character) {
                CharacterMono characterMono = selectmono as CharacterMono;
                // 先设置pickup脚本所需的参数
                

                // 切换charactermono中AI
                if(characterMono.BTree.ExternalBehavior != characterMono.pickup)
                    characterMono.SwitchBehaviourScript();
            }
        }
    }

}
