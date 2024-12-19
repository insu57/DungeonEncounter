using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        private string _projectileKey;
        private Vector3 _targetPos;
        private Vector3 _startPos;
        private Rigidbody _rigidbody;
        private float _speed;
        private EnemyRangedAttack _enemyRangedAttack;
    
        public float Damage { get; private set; }

        public void InitEnemyProjectile(EnemyRangedAttack enemyRangedAttack, Vector3 targetPos)
        {
            _enemyRangedAttack = enemyRangedAttack;
            _projectileKey = _enemyRangedAttack.GetProjectileKey();
            _speed = _enemyRangedAttack.ProjectileSpeed;
            Damage = _enemyRangedAttack.Damage;
            _targetPos = targetPos;
            _targetPos.y += 1f;
            _startPos = _enemyRangedAttack.transform.position;
            Debug.Log("StartPos: " + _startPos + " TargetPos: " + _targetPos);
        }

        private void Awake()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _rigidbody.isKinematic = false;
        }

        private void Start()
        {
            //transform.SetParent(null);
            //_rigidbody.AddForce((_targetPos - _startPos).normalized * _speed ,ForceMode.Impulse);//
        }

        private void Update()
        {
            _rigidbody.AddForce((_targetPos - _startPos).normalized * _speed ,ForceMode.Impulse);//
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ObjectPoolingManager.Instance.ReturnToPool(_projectileKey,gameObject);
                //Destroy(gameObject); //추후 오브젝트 풀링으로 관리로 수정 예정
            }
            else if(other.CompareTag("Floor"))
            {
                // _rigidbody.isKinematic = true;
                ObjectPoolingManager.Instance.ReturnToPool(_projectileKey,gameObject);
                //Destroy(gameObject, 1f);
            }
        }
    }
}
