using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

/// <summary>
/// 
/// </summary>
public class CharacterMono : MonoBase
{
    private Animator animator;
    private NavMeshAgent agent;
    public NavMeshAgent Agent {
        get {
            return agent;
        }
    }
    private NavMeshObstacle obstacle;
    private BehaviorTree behaviorTree;
    public BehaviorTree BTree {
        get {
            return behaviorTree;
        }
    }
    
    // 收集到资源后返回的建筑
    public BuildingMono bindingBuild;

    // 该单位身上携带的资源数目
    private OwnedResource itsResource;
    public OwnedResource ItsResource {
        get {
            return itsResource;
        }
    }

    /// <summary>
    /// 基本属性将用享元模式进行优化
    /// </summary>
    #region 基本属性
    public ExternalBehavior solider;
    public ExternalBehavior pickup;


    private float turnArroundSpeed;
    private float pickupSpeed = 1.0f;
    public float PickupSpeed {
        get {
            return pickupSpeed;
        }
    }
    #endregion

    public override void Awake() {
        base.Awake();
        Init();
    }



    private void Update() {


        DetectArroundEnemies();


        if(agent.isActiveAndEnabled)
            EndMove();
    }

    private void Init() {
        monotype = MonoEnum.character;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        behaviorTree = GetComponent<BehaviorTree>();
        agent.enabled = false;
        obstacle.enabled = true;
        obstacle.carving = true;
        isOperateByPlayer = false;
    }

    /// <summary>
    /// 是否正对目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsForwardToTarget(GameObject target) {
        Vector3 dir = target.transform.position - transform.position;
        
        if(Vector3.Angle(dir, transform.forward) < 30f) return true;
        else return false;
    }

    /// <summary>
    /// 用于角色转向
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool ChangeDirection(GameObject target) {
        if(IsForwardToTarget(target)) return true;

        obstacle.carving = false;
        obstacle.enabled = false;
        transform.forward = Vector3.Slerp(transform.forward, target.transform.position - transform.position, turnArroundSpeed);

        return false;
    }

    public void Move(Vector3 pos) {
        /// <summary>
        /// 这里由于关闭obstacle和打开agent在同一帧，因此会造成角色瞬移
        /// </summary>
        obstacle.enabled = false;
        obstacle.carving = false;
        agent.enabled = true;
        agent.SetDestination(pos);
        animator.SetFloat("run", 1);
    }

    public bool EndMove() {
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            animator.SetFloat("run", 0);
            agent.enabled = false;
            obstacle.enabled = true;
            obstacle.carving = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 执行对敌方的攻击
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool Attack(CharacterMono target) {
        // 当前状态和下一状态均不为Attack
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetNextAnimatorStateInfo(0).IsName("Attack")) {
            animator.SetTrigger("attack");
        }

        

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f) {
            animator.ResetTrigger("attack");
            // 执行伤害逻辑

            return true;
        }

        return false;
    }

    public void DoDamge(MonoBase target) {

    }


    public override void TurnOnOperateByPlayer() {
        isOperateByPlayer = true;
        behaviorTree.enabled = false;
        agent.enabled = false;
        obstacle.enabled = false;
    }

    public override void TurnOffOperateByPlayer() {
        isOperateByPlayer = false;
        behaviorTree.enabled = true;
    }

    public void RemoveResources() {
        itsResource = new OwnedResource();
    }

    public void SetResources(OwnedResource resources) {
        itsResource = resources;
    }

    public void SwitchBehaviourScript() {
        behaviorTree.enabled = false;
        if(behaviorTree.ExternalBehavior == solider) behaviorTree.ExternalBehavior = pickup;
        else behaviorTree.ExternalBehavior = solider;
    }

}