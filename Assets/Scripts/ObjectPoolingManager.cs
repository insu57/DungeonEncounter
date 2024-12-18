using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
   //ObjectPool<>
    private List<ObjectPool<GameObject>> _objectPools = new List<ObjectPool<GameObject>>();
    public ObjectPool<GameObject> ArrowPool{get; private set;}
    [SerializeField] private GameObject arrowPrefab;
    
    
    
    public override void Awake()
    {
        base.Awake();
        ArrowPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(arrowPrefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: Destroy,
            defaultCapacity: 10,
            maxSize: 20
        );
    }
}
