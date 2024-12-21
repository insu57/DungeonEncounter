using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    [Serializable]
    public class PoolPrefab
    {
        public PoolKeys key;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxSize;
    }
    
    //추후 개선-(Data 받아와서...)//현재: Inspector에서 받아오기(SerializeField). -> ScriptableObject?(Xml등에서 받아옴)
    [SerializeField] private List<PoolPrefab> poolPrefabs = new List<PoolPrefab>();
    private readonly Dictionary<PoolKeys, ObjectPool<GameObject>> _pools = new Dictionary<PoolKeys, ObjectPool<GameObject>>();
   
    
    private void InitPools()
    {
        foreach (var poolPrefab in poolPrefabs)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreatePoolItem(poolPrefab.prefab),
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnToPool,
                actionOnDestroy: OnDestroyPoolObject,
                defaultCapacity: poolPrefab.defaultCapacity,
                maxSize: poolPrefab.maxSize
                );
            _pools.Add(poolPrefab.key, pool);
            
            var tempList = new List<GameObject>();
            for (int i = 0; i < poolPrefab.defaultCapacity; i++)
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
        InitPools();
    }
}
