using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private ConsumableItemData data;
    [SerializeField] private ParticleSystem particleMainCircle;
    [SerializeField] private ParticleSystem particleLight;
    public ConsumableItemData Data => data;

    private void Awake()
    {
        var particleMain = particleMainCircle.main;
        particleMain.startColor = EnumManager.RarityToColor(data.Rarity);
        particleMain = particleLight.main;
        particleMain.startColor = EnumManager.RarityToColor(data.Rarity);
    }
}
