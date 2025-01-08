using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private Material material;
    [SerializeField] private bool inactiveParentAfterMerge = true;
    [SerializeField] private bool destroyParentAfterMerge = false;
    //private string _tag;
    
    [ContextMenu("Merge")]
    public void MergeMeshes()
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh != null)
            {
                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix,
                };
                combineInstances.Add(combineInstance);
                
            }
        }
        GameObject combineObject = new GameObject("CombinedMesh");
        combineObject.AddComponent<MeshFilter>();
        combineObject.AddComponent<MeshRenderer>();
        combineObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        combineObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combineInstances.ToArray(), true);
        combineObject.GetComponent<MeshRenderer>().material = material;
        
        if (inactiveParentAfterMerge)
        {
            parent.SetActive(false);
        }

        if (destroyParentAfterMerge)
        {
            Destroy(parent);
        }
    }
    
}
