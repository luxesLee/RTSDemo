using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 建筑物单位的Mono
/// </summary>
public class BuildingMono : MonoBase
{
    public GameObject soliderPrefab;
    // 生产队列
    public Queue<GameObject> productQueue;
    private Transform birthPoint;
    private Transform assemblePoint;
    private float currentBuildProgress;
    private float maxBuildProgress;

    public override void Awake() {
        base.Awake();
        birthPoint = transform.Find("BirthPoint").transform;
        assemblePoint = transform.Find("AssemblePoint").transform;



        monotype = MonoEnum.building;
    }

    void Start()
    {
        // Init();
        curHP.Value = 5000;
        productQueue = new Queue<GameObject>();
        currentBuildProgress = 0.0f;
        maxBuildProgress = 5.0f;
    }

    private void Init() {

    }

    
    public override void Update()
    {
        base.Update();
        ProcessQueue();
    }


    /// <summary>
    /// 
    /// </summary>
    public void InsertGameObjectToQueue() {
        productQueue.Enqueue(soliderPrefab);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RemoveGameObjectFromQueue() {
        productQueue.Dequeue();
        currentBuildProgress = 0.0f;
    }

    /// <summary>
    /// 处理生产队列
    /// </summary>
    private void ProcessQueue() {
        if(productQueue.Count > 0) {
            currentBuildProgress += Time.deltaTime;
            if(currentBuildProgress > maxBuildProgress) {
                // Instantiate(soliderPrefab, birthPoint.position, Quaternion.identity);
                CharacterMono newMono = player.AddNewUnit(birthPoint.position, Quaternion.identity, soliderPrefab) as CharacterMono;
                if(newMono != null) {
                    newMono.Move(assemblePoint.position);
                    productQueue.Dequeue();
                    currentBuildProgress = 0.0f;
                }
            }
        }
    }
    
    public float GetProductPercentage() {
        return currentBuildProgress / maxBuildProgress;
    }

    public void SetAssemblePoint(Vector3 pos) {
        assemblePoint.transform.position = pos;
    }


    /// <summary>
    /// 用于提升Player的某些能力
    /// </summary>
    public void Improve() {

    }

}
