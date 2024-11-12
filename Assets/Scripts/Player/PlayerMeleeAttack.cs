using Scriptable_Objects;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private PlayerManager _playerManager;
    private TrailRenderer _trailRenderer;
    private Collider _attackArea;
    private void Awake()
    {
        _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>(); //이펙트 수정 필요. 
        _attackArea = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerManager.IsAttack) //이펙트 시간...코루틴으로? 지금은 공격끝나면 바로 사라짐
        {
            _attackArea.enabled = true;
            _trailRenderer.enabled = true;
        }
        else
        {
            _attackArea.enabled = false;
            _trailRenderer.enabled = false;
        }
    }
}
