using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class ItemDataAssign : MonoBehaviour
{
    //[SerializeField] protected GameObject itemPrefab;
    //public GameObject ItemPrefab => itemPrefab;
    private ParticleSystem[] _particleSystems;
    public abstract IItemData GetItemData();
    
    protected void SetParticleArray()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    protected void SetParticleColor(IItemData data)
    {
        foreach (var particle in _particleSystems)
        {
            var particleMain = particle.main;
            particleMain.startColor = EnumManager.RarityToColor(data.GetRarity());
        }
        
    }
    
}
