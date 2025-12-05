using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemies
{
    public class EnemyScript : MonoBehaviour
    {
        [SerializeField] private EnemyData enemySO;

        private enum States
        {
            Patrol,
            Chase,
            Attack,
            Scan
        }
    
        private States currState = States.Patrol;


        private NavMeshAgent _agent;
        private Animator _anim;
        private AudioSource _audioSource;
        private Transform _player;
        [CanBeNull] public Transform projectileSpawn;


        public Transform patrolPointsParent;
        private Transform[] _patrolPoints;
        private int _currentPatrolIndex;
        
        private bool _alreadyAttacked;

        public float scanSpeed;   // degrees per second
        public float scanAngle;   // max angle left/right
        private float baseY;
        
        void Awake()
        { 
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _agent = GetComponent<NavMeshAgent>();
            _audioSource = GetComponent<AudioSource>();
            _anim = GetComponent<Animator>();
        }

        // Start is called before the first frame update

        void Start()
        {
            if (enemySO == null)
            {
                Debug.LogError($"EnemyData not assigned to {name}");
                enabled = false;
                return;
            }

            if (enemySO.enemyType == EnemyType.MeleeCombatant)
            {
                scanSpeed = 30f;
                scanAngle = 90f;
            }
            else if (enemySO.enemyType == EnemyType.Shooter)
            {
                scanSpeed = 15f;
                scanAngle = 50f;
            }
            baseY = transform.eulerAngles.y;
            
            _agent.speed = enemySO.walkSpeed;
        
            if (patrolPointsParent != null)
            {
                 _patrolPoints = patrolPointsParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointsParent) .ToArray();

                if (_patrolPoints.Length > 0)
                {
                    SetClosestPatrolPoint();
                }
            }

        }
    
        // Update is called once per frame
        private void Update()
        {
            float angle = Mathf.Sin(Time.time * scanSpeed * Mathf.Deg2Rad) * scanAngle;
            transform.rotation = Quaternion.Euler(0, baseY + angle, 0);
            
            PlayerInSight();
            PlayerInAttackRange();

            switch (currState)
            {
                case States.Patrol:
                    Patrol();
                    if (PlayerInSight() && enemySO.enemyType == EnemyType.MeleeCombatant)
                    {
                        currState = States.Chase;
                    }
                    else if (PlayerInSight() && enemySO.enemyType == EnemyType.Shooter)
                    {
                        currState = States.Attack;
                    }

                    break;
                case States.Chase:
                    Chase();
                    if (PlayerInAttackRange() && enemySO.enemyType == EnemyType.MeleeCombatant)
                    {
                        currState = States.Attack;
                    }
                    else if (!PlayerInSight() && enemySO.enemyType == EnemyType.MeleeCombatant)
                    {
                        currState = States.Scan;
                        Thread.Sleep((int)(enemySO.searchingDelay * 1000));
                        currState = States.Patrol;
                    }
                    break;
                case States.Attack:
                    Attack();
                    if (!PlayerInAttackRange() && PlayerInSight() && enemySO.enemyType == EnemyType.MeleeCombatant)
                    {
                        currState = States.Chase;
                    }
                    else if (!PlayerInSight())
                    {
                        currState = States.Scan;
                        Thread.Sleep((int)(enemySO.searchingDelay * 1000));
                        currState = States.Patrol;
                    }

                    break;
                case States.Scan:
                    Scan();
                    if (PlayerInSight())
                    {
                        currState = States.Attack;
                    }
                    break;
                
            }
        }
    
        #region Enemy Checks

        private bool PlayerInSight()
        {
            var directionToPlayer = (_player.position - transform.position).normalized;
            var distance = Vector3.Distance(transform.position, _player.position);
            var angle = Vector3.Angle(transform.forward, directionToPlayer);

            return distance <= enemySO.sightRange && angle <= enemySO.sightAngle / 2;
        }

        private bool PlayerInAttackRange()
        {
            var directionToPlayer = (_player.position - transform.position).normalized;
            var distance = Vector3.Distance(transform.position, _player.position);
            var angle = Vector3.Angle(transform.forward, directionToPlayer);

            return distance <= enemySO.attackRange && angle <= enemySO.sightAngle / 2;
        }

        #endregion

    
        #region Patrol Logic

        // ReSharper disable Unity.PerformanceAnalysis
        private void Patrol()
        {
            Debug.Log("Entered Patrol State...");

            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                Debug.LogWarning("Patrol points do not exist.");
                return;
            }

            _agent.speed = enemySO.walkSpeed;
            _anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);

            var targetPoint = _patrolPoints[_currentPatrolIndex];
            _agent.SetDestination(targetPoint.position);
            if (Vector3.Distance(transform.position, targetPoint.position) < 1)
            {
                GoToNextPatrolPoint();
            }
        }

        private void SetClosestPatrolPoint()
        {
            var closestDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                var distance = Vector3.Distance(transform.position, _patrolPoints[i].position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            _currentPatrolIndex = closestIndex;
            _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
        }

        private void GoToNextPatrolPoint()
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
        }

        #endregion

        private void Chase()
        {
            Debug.Log("Entered Chase State...");
            _agent.speed = enemySO.chaseSpeed;
            _agent.SetDestination(_player.position);

            _anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);

            var direction = (_player.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                var lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Attack()
        {
            Debug.Log("Entered Attack State...");
            _agent.SetDestination(transform.position);
            transform.LookAt(_player);
        
            _anim.SetTrigger("Attack");

            if (!_alreadyAttacked)
            {
                switch (enemySO.enemyType)
                {
                    case EnemyType.Shooter:
                        PerformRangedAttack();
                        break;
                    case EnemyType.MeleeCombatant:
                        PerformMeleeAttack();
                        break;
                }

                _alreadyAttacked = true;
                Invoke(nameof(ResetAttack), enemySO.attackCooldown);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void PerformRangedAttack()
        {
            if (!enemySO.projectilePrefab)
            {
                Debug.LogWarning($"{enemySO.name} is a shooter but has no projectile prefab assigned!");
                return;
            }

            if (projectileSpawn)
            {
                var projectile = ObjectPooler.GetFromPool("bullet", projectileSpawn.transform.position, Quaternion.identity);

                if (enemySO.gunshotSfx != null)
                {
                    var gunshot = enemySO.gunshotSfx[Random.Range(0, enemySO.gunshotSfx.Length)];
                    _audioSource.pitch = Random.Range(0.9f, 1.2f);
                    _audioSource.volume = Random.Range(0.8f, 1f);
                    _audioSource.PlayOneShot(gunshot);
                }

                var rb = projectile.GetComponent<Rigidbody>();

                if (rb)
                {
                    var direction = (_player.position - transform.position).normalized;
                    rb.velocity = direction * enemySO.projectileSpeed;
                }
                else
                {
                    Debug.LogWarning("Bullet does not have a rigidbody.");
                }
                
                ObjectPooler.ReleaseFromPool(projectile, 2);
            }
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }

        private void PerformMeleeAttack()
        {
            // TODO: Melee Attack
        }

        private void Scan()
        {
            if (enemySO.enemyType == EnemyType.MeleeCombatant)
            {
                // rotate left and right slowly
                float angle = Mathf.Sin(Time.time * scanSpeed * Mathf.Deg2Rad) * scanAngle;
                transform.rotation = Quaternion.Euler(0, baseY + angle, 0);
            }
        }

    }
}