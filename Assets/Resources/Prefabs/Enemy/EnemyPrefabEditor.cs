using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyPrefabEditor : EditorWindow
{
    public static void ApplyCommonComponents()
    {
        GameObject enemyPrefab = Selection.activeGameObject;
        if (enemyPrefab == null)
        {
            Debug.LogError("No GameObject selected");
            return;
        }
        
        string[] commonComponents = {"NavMeshAgent", "Animator", "Rigidbody"};
    }
        
    
}
