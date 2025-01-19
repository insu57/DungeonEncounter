using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    [SerializeField] private Material originalMaterialColor;
    [SerializeField] private Renderer[] renderers;
    private List<Material> _materials = new List<Material>();
    
    private Renderer[] _renderers;
    private Color _originalColor;
    private Color _flashColor = Color.red;
    private float _flashDuration = 0.2f;
    //null???
   
    private void Awake()
    {
        foreach (var rend in renderers)
        {
            _materials.Add(rend.material);
        }
        _originalColor = originalMaterialColor.color;
    }

    private void OnEnable()
    {
        foreach (var mat in _materials)
        {
            mat.color = _originalColor;
        }
    }
    
    public void TriggerFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()//맞으면 깜빡임
    {
        foreach (var mat in _materials)
        {
            mat.color = _flashColor;
        }
        yield return new WaitForSeconds(_flashDuration);
        foreach (var mat in _materials)
        {
            mat.color = _originalColor;
        }
    }
    
}
