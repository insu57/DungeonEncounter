using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject chestTop;
    private GameObject _dropItem;
    private bool _isOpen;
    public void SetItem(GameObject item)
    {
        _dropItem = item;
        Debug.Log(_dropItem.name);
    }
    public void OpenChest()
    {
        if(_isOpen) return;
        Vector3 targetRotation = new Vector3(-90f, 0f, 0f);
        chestTop.transform.DOLocalRotate(targetRotation, 0.5f);
        Instantiate(_dropItem, chestTop.transform.position+Vector3.back, Quaternion.identity);
        _isOpen = true;
        StartCoroutine(DisappearChest());
    }

    private IEnumerator DisappearChest()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    private void Awake()
    {
        _isOpen = false;
    }
}
