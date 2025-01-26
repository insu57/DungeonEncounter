using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    private readonly Dictionary<PoolKeys, ObjectPool<GameObject>> _pools = new Dictionary<PoolKeys, ObjectPool<GameObject>>();
    private GameObject _temporaryPoolParent;
    public void InitStagePools(List<PoolingObject> poolingObjects)
    {
        
        _pools.Clear();

        
        if (_temporaryPoolParent != null)
        {
            Destroy(_temporaryPoolParent);
        }
        
        _temporaryPoolParent = new GameObject("TemporaryPoolParent");
        _temporaryPoolParent.transform.SetParent(null);
        
        Debug.Log("pools length: " + poolingObjects.Count);
        foreach (var poolingObject in poolingObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreatePoolItem(poolingObject.prefab),
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnToPool,
                actionOnDestroy: OnDestroyPoolObject,
                defaultCapacity: poolingObject.defaultCapacity,
                maxSize: poolingObject.maxSize
            );
            _pools.Add(poolingObject.poolKey, pool);
            
            var tempList = new List<GameObject>();
            for (int i = 0; i < poolingObject.defaultCapacity; i++)
            {
                var obj = pool.Get();
                tempList.Add(obj);
            }

            foreach (var obj in tempList)
            {
                pool.Release(obj);
            }
        }
    }
    
    private GameObject CreatePoolItem(GameObject prefab)
    {
        //GameObject item = Instantiate(prefab, _temporaryPoolParent.transform);
        GameObject item = Instantiate(prefab, transform);
        return item;
    }

    private static void OnTakeFromPool(GameObject prefab)
    {
        prefab.SetActive(true);
    }

    private static void OnReturnToPool(GameObject prefab)
    {
        prefab.SetActive(false);
    }

    private static void OnDestroyPoolObject(GameObject prefab)
    {
        Destroy(prefab);
    }

    public GameObject GetObjectFromPool(PoolKeys key, Vector3 position, Quaternion rotation)
    {
        if (_pools.TryGetValue(key, out ObjectPool<GameObject> pool))
        {
            GameObject obj = pool.Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }
        //null error
        Debug.LogError("Get Pool Error");
        return null;
    }

    public void ReturnToPool(PoolKeys key, GameObject obj)
    {
        if (_pools.TryGetValue(key, out ObjectPool<GameObject> pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogError("Return Pool ERROR");
        }
    }
    
    public override void Awake()
    {
        base.Awake();
        //InitPools();
    }
}
