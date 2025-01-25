using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class TraderNPC : MonoBehaviour
{
   private PlayerManager _playerManager;
   private float _distance;
   //NPC
   
   private void Awake()
   {
      _playerManager = FindObjectOfType<PlayerManager>();
   }

   private void Update()
   {
      _distance = Vector3.Distance(_playerManager.transform.position, transform.position);
      if (_distance < 1f)
      {
         //pop up
      }
   }
}
