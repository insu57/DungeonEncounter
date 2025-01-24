using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component //Singleton Generic 싱글톤 제네릭
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = (T)FindAnyObjectByType(typeof(T));
                if (!_instance)
                {
                    SetupInstance();

                }
            }
            return _instance;
        }
    }
    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static void SetupInstance()
    {
        _instance = (T)FindAnyObjectByType(typeof(T));

        if (!_instance)
        {
            GameObject gameObj = new GameObject
            {
                name = typeof(T).Name
            };
            _instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }
    }

    private void RemoveDuplicates()
    {
        if(!_instance)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log(gameObject.name);
            Destroy(gameObject);
        }
    }
}
