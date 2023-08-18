using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 对象
/// </summary>
public class Pool : MonoBehaviour
{
    public GameObject prefab;

    /// <summary>
    /// 游戏开始需要创建的数量
    /// </summary>
    public int preLoad = 0;

    public bool limit = false;

    public int maxCount;

    [HideInInspector] public List<GameObject> active = new List<GameObject>();

    [HideInInspector] public List<GameObject> inactive = new List<GameObject>();

    public void Awake()
    {
       
    }
    public void Init()
    {
        if (prefab == null) return;

        PoolMgr.Add(this);

        PreLoad();
    }
    public void PreLoad()
    {
        if (prefab == null)
        {
            Debug.LogWarning("预制体为空，请检查");
            return;
        }

        for (int i = totalCount; i < preLoad; i++)
        {
            GameObject obj = (GameObject)Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(transform);
            Rename(obj.transform);
            obj.SetActive(false);
            inactive.Add(obj);
        }
    }


    /// <summary>
    /// 激活一个对象来着对象池
    /// </summary>
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        Transform trans;

        if (inactive.Count > 0)
        {
            //激活一个则从 inactive 中进行移除
            obj = inactive[0];
            inactive.RemoveAt(0);
            trans = obj.transform;
        }
        else
        {
            //假如我们没有足够激活对象，则根据限制选择是否创建
            if (limit && active.Count >= maxCount)
                return null;

            obj = (GameObject)Object.Instantiate(prefab);
            trans = obj.transform;
            Rename(trans);
        }

        //设置位置信息
        trans.position = position;
        trans.rotation = rotation;
        //修改父物体
        if (trans.parent != transform)
            trans.parent = transform;

        //添加到active 队列
        active.Add(obj);
        obj.SetActive(true);
        //call the method OnSpawn() on every component and children of this object
        obj.BroadcastMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
        return obj;
    }


    public void Despawn(GameObject instance)
    {
        //search in active instances for this instance
        if (!active.Contains(instance))
        {
            Debug.LogWarning("Can't despawn - Instance not found: " + instance.name + " in Pool " + this.name);
            return;
        }

        //in case it was unparented during runtime, reparent it now
        if (instance.transform.parent != transform)
            instance.transform.parent = transform;

        //we want to deactivate it, remove it from the active list
        active.Remove(instance);
        //add object to the list of inactive instances instead
        inactive.Add(instance);
        //call the method OnDespawn() on every component and children of this object
        instance.BroadcastMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);
        //deactivate object including child objects
        instance.SetActive(false);
    }



    /// <summary>
    /// Timed deactivation of an instance of this pool for later use.
    /// </summary>
    public void Despawn(GameObject instance, float time)
    {
        //create new class PoolTimeObject to keep track of the instance
        PoolTimeObject timeObject = new PoolTimeObject();
        //assign time and instance variable of this class
        timeObject.instance = instance;
        timeObject.time = time;

        //start timed deactivation using the created properties
        StartCoroutine(DespawnInTime(timeObject));
    }


    //coroutine which waits for 'time' seconds before deactivating the instance
    IEnumerator DespawnInTime(PoolTimeObject timeObject)
    {
        //cache instance to deactivate
        GameObject instance = timeObject.instance;

        //wait for defined seconds
        float timer = Time.time + timeObject.time;
        while (instance.activeInHierarchy && Time.time < timer)
            yield return null;

        //the instance got deactivated in between already
        if (!instance.activeInHierarchy) yield break;
        //despawn it now
        Despawn(instance);
    }



    /// <summary>
    /// 销毁未使用的
    /// </summary>
    /// <param name="limitToPreLoad"></param>
    public void DestroyUnused(bool limitToPreLoad)
    {
        //only destroy instances above the limit amount
        if (limitToPreLoad)
        {
            //start from the last inactive instance and count down
            //until the index reached the limit amount
            for (int i = inactive.Count - 1; i >= preLoad; i--)
            {
                //destroy the object at 'i'
                Object.Destroy(inactive[i]);
            }
            //remove the range of destroyed objects (now null references) from the list
            if (inactive.Count > preLoad)
                inactive.RemoveRange(preLoad, inactive.Count - preLoad);
        }
        else
        {
            //limitToPreLoad is false, destroy all inactive instances
            for (int i = 0; i < inactive.Count; i++)
            {
                Object.Destroy(inactive[i]);
            }
            //reset the list
            inactive.Clear();
        }
    }


    public void DestroyCount(int count)
    {
        //the amount which was passed in exceeds the amount of inactive instances
        if (count > inactive.Count)
        {
            Debug.LogWarning("Destroy Count value: " + count + " is greater than inactive Count: " +
                            inactive.Count + ". Destroying all available inactive objects of type: " +
                            prefab.name + ". Use DestroyUnused(false) instead to achieve the same.");
            DestroyUnused(false);
            return;
        }

        //starting from the end, count down the index and destroy each inactive instance
        //until we destroyed the amount passed in
        for (int i = inactive.Count - 1; i >= inactive.Count - count; i--)
        {
            Object.Destroy(inactive[i]);
        }
        //remove the range of destroyed objects (now null references) from the list
        inactive.RemoveRange(inactive.Count - count, count);
    }

    private void Rename(Transform instance)
    {
        instance.name += (totalCount + 1).ToString("#000");
    }


    /// <summary>
    /// 总数量
    /// </summary>
    private int totalCount => active.Count + inactive.Count;

    private void OnDestroy()
    {
        active.Clear();
        inactive.Clear();
    }
}


[System.Serializable]
public class PoolTimeObject
{
    /// <summary>
    /// Instance to deactivate.
    /// </summary>
    public GameObject instance;

    /// <summary>
    /// Delay until deactivation.
    /// </summary>
    public float time;
}
