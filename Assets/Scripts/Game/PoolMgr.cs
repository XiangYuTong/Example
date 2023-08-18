using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class PoolMgr : MonoBehaviour
{
    private static PoolMgr _instance;//单例

    public static PoolMgr instance
    {
        get
        {
            if (_instance == null)
            {
                PoolMgr ins = FindObjectOfType<PoolMgr>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有PoolMgr组件，请添加");
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }
    private static Dictionary<GameObject, Pool> Pools = new Dictionary<GameObject, Pool>();

    public List<PoolData> poolDatas = new List<PoolData>();
    public void Init()
    {
        for (int i = 0; i < poolDatas.Count; i++)
        {
            CreatePool(poolDatas[i].prefab, poolDatas[i].preLoad, poolDatas[i].limit, poolDatas[i].maxCount);
        }
    }
    public static void Add(Pool pool)
    {
        //检查预制体对象
        if (pool.prefab == null)
        {
            Debug.LogError("Prefab of pool: " + pool.gameObject.name + " is empty! Can't add pool to Pools Dictionary.");
            return;
        }

        ///检查是否之前就存在预制体
        if (Pools.ContainsKey(pool.prefab))
        {
            Debug.LogError("Pool with prefab " + pool.prefab.name + " has already been added to Pools Dictionary.");
            return;
        }

        Pools.Add(pool.prefab, pool);
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="preLoad"></param>
    /// <param name="limit"></param>
    /// <param name="maxCount"></param>
    public static void CreatePool(GameObject prefab, int preLoad, bool limit, int maxCount)
    {
        //debug error if pool was already added before 
        if (Pools.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager already contains Pool for prefab: " + prefab.name);
            return;
        }

        //create new gameobject which will hold the new Pool component
        GameObject newPoolGO = new GameObject("Pool " + prefab.name);
        newPoolGO.transform.parent = instance.transform;
        //add Pool component to the new gameobject in the scene
        Pool newPool = newPoolGO.AddComponent<Pool>();
        //assign default parameters
        newPool.prefab = prefab;
        newPool.preLoad = preLoad;
        newPool.limit = limit;
        newPool.maxCount = maxCount;
        //let it initialize itself after assigning variables
        newPool.Init();
    }


    /// <summary>
    /// 创建对象池对象
    /// </summary>
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        //debug a Log entry in case the prefab was not found in a Pool
        //this is not critical as then we create a new Pool for it at runtime
        if (!Pools.ContainsKey(prefab))
        {
            Debug.Log("Prefab not found in existing pool: " + prefab.name + " New Pool has been created.");
            CreatePool(prefab, 0, false, 0);
        }

        //spawn instance in the corresponding Pool
        return Pools[prefab].Spawn(position, rotation);
    }


    public static void Despawn(GameObject instance, float time = 0f)
    {
        if (time > 0) GetPool(instance).Despawn(instance, time);
        else GetPool(instance).Despawn(instance);
    }

    /// <summary>
    /// 获取对象池
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static Pool GetPool(GameObject instance)
    {
        //go over Pools and find the instance
        foreach (GameObject prefab in Pools.Keys)
        {
            if (Pools[prefab].active.Contains(instance))
                return Pools[prefab];
        }

        //the instance could not be found in a Pool
        Debug.LogError("PoolManager couldn't find Pool for instance: " + instance.name);
        return null;
    }


    /// <summary>
    /// 删除Pool的所有实例，使它们可供以后使用。
    /// </summary>
    public static void DeactivatePool(GameObject prefab)
    {
        //debug error if Pool wasn't already added before
        if (!Pools.ContainsKey(prefab))
        {
            Debug.LogError("PoolManager couldn't find Pool for prefab to deactivate: " + prefab.name);
            return;
        }

        //cache active count
        int count = Pools[prefab].active.Count;
        //loop through each active instance
        for (int i = count - 1; i > 0; i--)
        {
            Pools[prefab].Despawn(Pools[prefab].active[i]);
        }
    }

    public static void DestroyAllInactive(bool limitToPreLoad)
    {
        foreach (GameObject prefab in Pools.Keys)
            Pools[prefab].DestroyUnused(limitToPreLoad);
    }


    /// <summary>
    /// 销毁对象池
    /// </summary>
    /// <param name="prefab"></param>
    public static void DestroyPool(GameObject prefab)
    {
        if (!Pools.ContainsKey(prefab))
        {
            Debug.LogError("PoolManager couldn't find Pool for prefab to destroy: " + prefab.name);
            return;
        }

        //先销毁子物体
        Destroy(Pools[prefab].gameObject);
        Pools.Remove(prefab);
    }

    /// <summary>
    /// 销毁所有池
    /// </summary>
    public static void DestroyAllPools()
    {
        foreach (GameObject prefab in Pools.Keys)
            DestroyPool(Pools[prefab].gameObject);
    }


    void OnDestroy()
    {
        Pools.Clear();
    }
}
[Serializable]
public class PoolData
{
    public GameObject prefab;
    public int preLoad;
    public bool limit;
    public int maxCount;
}
