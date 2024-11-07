using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private float _damage;
    private string _name;
    private string _description;
    private string _type;
   
    public float Damage{set=>_damage=value; get=>_damage;}
    public string Name => _name;
    public string Description => _description;
    public string Type => _type;

    //저장된 데이터 불러오기

    private void Awake()
    {
        _damage = 20f;
    }
    
    
}
