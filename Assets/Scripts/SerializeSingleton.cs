using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SerializeSingleton<T> : SerializedMonoBehaviour where T : Component
{
    //Odin Inspector의 OdinSerialize 가능한 싱글톤 클래스
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
            Destroy(gameObject);
        }
    }
}
