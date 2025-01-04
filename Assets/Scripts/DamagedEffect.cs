using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedEffect : MonoBehaviour
{
    private Material[] _materials;
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;

    private void Awake()
    {
        _materials = GetComponent<Renderer>().materials;
    }

    public void TriggerFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        foreach (var mat in _materials)
        {
            mat.SetColor("_FlashColor", flashColor);
            mat.SetFloat("_FlashAmount", 1.0f);
        }
        
        yield return new WaitForSeconds(flashDuration);

        foreach (var mat in _materials)
        {
            mat.SetFloat("_FlashAmount", 1.0f);
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
