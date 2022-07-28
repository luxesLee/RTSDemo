using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMVVM.Sources.Infrastructure;

/// <summary>
/// 对象池，用于实例化CharacterMono
/// </summary>
public class MonoPool
{
    private class PoolData {
        public GameObject obj;
        public bool isUsed;
    }


    // 对象池的数量应该与最大人口有关系
    // 因此保证pools的PoolData数目不会超过capacity
    public BindableProperty<int> capacity = new BindableProperty<int>();
    private List<PoolData> pools;


    public MonoPool(int value) {
        capacity.Value = value;
        pools = new List<PoolData>(value);
    }

    public GameObject GetGameObjectInPool(Vector3 pos, Quaternion qua, GameObject template) {
        MonoBase templateMono = template.GetComponent<MonoBase>();
        // 首先遍历对象池，寻找是否有未使用但已经存在的对象

        lock (pools) {
            foreach(PoolData poolData in pools) {
                if(!poolData.isUsed) {
                    poolData.isUsed = true;

                    poolData.obj.transform.position = pos;
                    poolData.obj.transform.rotation = qua;
                    return poolData.obj;
                }
            } 
        }


        // 检查容量是否==占用，若==则清理对象池
        if(pools.Count == capacity.Value) {
            int cleanNum = CleanPools();
            if(cleanNum == 0) return null;
        }
        
        // 未满足上述条件，根据模板创建
        GameObject go = GameObject.Instantiate(template, pos, qua);
        PoolData p = new PoolData { isUsed = true, obj = go };
        if(PutPoolDataToPools(p)) return p.obj;
        return null;
    }

    private bool PutPoolDataToPools(PoolData p) {
        lock (pools) {
            if(pools.Count < capacity.Value) {
                pools.Add(p);
            }
            else {
                GameObject.Destroy(p.obj);
                return false;
            }
        }
        return true;
    }

    private int CleanPools() {
        int count = 0;
        lock (pools) {
            foreach(PoolData p in pools) {
                if(!p.isUsed) {
                    pools.Remove(p);
                    count++;
                }
            }
        }
        return count;
    }

    public void ReleaseGameObject(GameObject obj) {
        PoolData p = pools.Find(delegate(PoolData pd) {
            return pd.obj == obj;
        });
        lock (pools) {
            p.isUsed = false;
        }
    }

    /// <summary>
    /// 扩展或收缩对象池
    /// 扩展收缩大小应该与building有关
    /// 应注意这个方法调用时机
    /// 调用该函数的前提条件是当前对象池中维护的被使用的PoolData数目小于value
    /// </summary>
    private void ExpandPoolsCapacity(int value) {
        List<PoolData> newPool = new List<PoolData>(value);
        foreach(var pool in pools) {
            if(pool.isUsed) {
                newPool.Add(pool);
            }
        }
        lock (pools) {
            pools = newPool;
        }
        capacity.Value = value;
        newPool = null;
    }

}
