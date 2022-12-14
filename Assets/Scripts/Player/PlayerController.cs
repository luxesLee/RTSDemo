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

    private SelectManager selectManager;
    public List<Vector3> offsets = new List<Vector3>(16);

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
                else if(hit.collider.gameObject.layer == 9) {
                    DoAttack(hit.collider.gameObject);
                }

            }

        }
    }

    private void DoAttack(GameObject hitGO) {
        MonoBase hitMono = hitGO.GetComponent<MonoBase>();
        if(player != hitMono.player) {
            foreach (var selectmono in selectManager.selectMono) {
                if(selectmono.MonoType == MonoEnum.character) {
                    CharacterMono characterMono = selectmono as CharacterMono;
                    characterMono.arroundEnemies.Clear();
                    characterMono.arroundEnemies.Add(hitMono);

                    // 切换charactermono中AI
                    if(characterMono.BTree.ExternalBehavior != characterMono.solider)
                        characterMono.SwitchBehaviourScript();
                }
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    private void MoveChacterOrAssemblePoint(Vector3 point) {
        int i = 0;
        foreach(var selectmono in selectManager.selectMono) {
            if(selectmono.MonoType == MonoEnum.character) {
                CharacterMono characterMono = selectmono as CharacterMono;

                if(characterMono.BTree.ExternalBehavior != characterMono.solider)
                    characterMono.SwitchBehaviourScript();

                Vector3 pos = point + new Vector3(0, 1, 0) + offsets[i++];
                characterMono.Move(pos);

                
                if(Server.instance != null) {
                    Server.instance.SendPos(pos.x, pos.z, player.monoOfPlayer.IndexOf(selectmono));
                }
                
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
                
                characterMono.resourceBase = hitGO.GetComponent<ResourceBase>();

                // 切换charactermono中AI
                if(characterMono.BTree.ExternalBehavior != characterMono.pickup)
                    characterMono.SwitchBehaviourScript();
            }
        }
    }

}
