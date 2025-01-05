using UnityEngine;

namespace Player
{
    public class PlayerMeleeAttack : MonoBehaviour
    {
        //private PlayerManager _playerManager;
        private PlayerControl _playerControl;
        private TrailRenderer _trailRenderer;
        private CapsuleCollider _attackArea;
        private void Awake()
        {
            _playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
            //_trailRenderer = GetComponentInChildren<TrailRenderer>(); //이펙트 수정 필요. 
            //_attackArea = GetComponent<CapsuleCollider>(); - 공격 Collider 별개로...
            
        }

        /*
         *
         private void Update()
        {
            if (_playerControl.IsAttack) 
            {
                //_attackArea.enabled = true;
                //_trailRenderer.enabled = true;
            }
            else
            {
                //_attackArea.enabled = false;
                //_trailRenderer.enabled = false;
            }
        }
         */
        
    }
}
