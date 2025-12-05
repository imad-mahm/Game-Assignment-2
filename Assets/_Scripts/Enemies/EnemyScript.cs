using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
    public EnemyData data;
    public Transform[] patrolPoints;
    public Transform player;

    private NavMeshAgent agent;
    private int patrolIndex = 0;
    private float attackTimer = 0f;
    private float searchTimer = 0f;

    private enum State { Patrol, Chase, Search, Attack }
    private State currentState = State.Patrol;

    private Vector3 lastKnownPlayerPos;
    private bool playerVisible = false;

    // Tunables
    [SerializeField] private float arriveThreshold = 0.25f;
    [SerializeField] private float rotationLerpSpeed = 10f; // used during movement
    [SerializeField] private float attackRotateSpeed = 360f; // degrees per second

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent missing!");
            enabled = false;
            return;
        }

        if (data == null)
        {
            Debug.LogError("EnemyData (data) not assigned!");
            enabled = false;
            return;
        }

        agent.speed = data.walkSpeed;
        agent.acceleration = 50f;
        agent.angularSpeed = 1200f; // high because we manually rotate
        agent.autoBraking = false;

        // We control rotation manually so the agent doesn't fight us.
        agent.updateRotation = false;
        agent.updatePosition = true;

        // Stopping distance small — shooter stops by logic, not by large stoppingDistance
        agent.stoppingDistance = data.enemyType == EnemyType.Shooter ? 0.5f : 0.5f;

        // Ensure patrolPoints is not null
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning($"{name}: No patrol points assigned.");
        }
        else
        {
            SetDestinationToCurrentPatrol(); // set first destination but don't advance index yet
        }
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        DetectPlayer();

        switch (currentState)
        {
            case State.Patrol:
                PatrolState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Search:
                SearchState();
                break;
            case State.Attack:
                AttackState();
                break;
        }
    }

    // ---------------------- DETECTION -------------------------
    void DetectPlayer()
    {
        if (player == null)
        {
            playerVisible = false;
            return;
        }

        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;

        // Distance check
        if (dist > data.sightRange)
        {
            playerVisible = false;
            return;
        }

        // Angle check (use half-angle)
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > data.sightAngle * 0.5f)
        {
            playerVisible = false;
            return;
        }

        // Raycast check (cast from eye height to player)
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 dirNorm = dir.normalized;
        if (Physics.Raycast(origin, dirNorm, out RaycastHit hit, data.sightRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                playerVisible = true;
                lastKnownPlayerPos = player.position;

                // If we see the player, go to chase (or attack will be decided in ChaseState)
                if (currentState != State.Attack)
                    currentState = State.Chase;

                return;
            }
        }

        playerVisible = false;
    }

    // ---------------------- PATROL -------------------------
    void PatrolState()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // Ensure agent moving to the current patrol point
        if (!agent.hasPath || agent.pathPending || agent.remainingDistance <= arriveThreshold)
        {
            // If we arrived (or no path), set next point
            if (!agent.pathPending && agent.remainingDistance <= arriveThreshold)
                AdvancePatrolIndexAndSetDestination();
            else if (!agent.hasPath)
                SetDestinationToCurrentPatrol();
        }

        // movement rotation: rotate toward movement direction every frame
        ApplyMovementRotation();

        // If we spotted the player, start chase logic
        if (playerVisible)
        {
            agent.isStopped = false;
            agent.speed = data.chaseSpeed;
            currentState = State.Chase;
        }
    }

    void SetDestinationToCurrentPatrol()
    {
        if (patrolPoints.Length == 0) return;
        agent.isStopped = false;
        agent.speed = data.walkSpeed;
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    void AdvancePatrolIndexAndSetDestination()
    {
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        SetDestinationToCurrentPatrol();
    }

    // ---------------------- CHASE -------------------------
    void ChaseState()
    {
        // If lost sight → go to Search (go to last known position)
        if (!playerVisible)
        {
            searchTimer = data.searchingDelay;
            currentState = State.Search;
            agent.isStopped = false;
            agent.SetDestination(lastKnownPlayerPos);
            return;
        }

        agent.speed = data.chaseSpeed;
        agent.isStopped = false;

        if (data.enemyType == EnemyType.MeleeCombatant)
        {
            // Melee: chase directly
            agent.SetDestination(player.position);

            // Rotate to face movement while moving
            ApplyMovementRotation();

            if (Vector3.Distance(transform.position, player.position) <= data.attackRange)
            {
                currentState = State.Attack;
            }
        }
        else // Shooter
        {
            float dist = Vector3.Distance(transform.position, player.position);

            // Keep distance: move if outside comfortable range; stop when near desired range
            float desiredDist = data.attackRange * 0.9f;
            if (dist > desiredDist + 0.1f) // a small hysteresis so it doesn't constantly toggle
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                ApplyMovementRotation();
            }
            else
            {
                // close enough — stop moving so we can aim & shoot
                agent.isStopped = true;
            }

            // If within attackRange (strict), enter Attack
            if (dist <= data.attackRange)
            {
                currentState = State.Attack;
            }
        }
    }

    // ---------------------- SEARCH -------------------------
    void SearchState()
    {
        if (playerVisible)
        {
            currentState = State.Chase;
            return;
        }

        // If agent still moving towards last known pos, allow it
        if (!agent.pathPending && agent.remainingDistance > arriveThreshold)
        {
            ApplyMovementRotation();
            return;
        }

        // We reached the last known position; rotate in place and countdown
        searchTimer -= Time.deltaTime;

        // rotate in place using scanSpeed (deg/sec)
        transform.Rotate(Vector3.up * data.scanSpeed * Time.deltaTime);

        if (searchTimer <= 0f)
        {
            currentState = State.Patrol;
            // set next patrol destination
            AdvancePatrolIndexAndSetDestination();
        }
    }

    // ---------------------- ATTACK -------------------------
    void AttackState()
    {
        if (!playerVisible)
        {
            // lost sight while attacking → search last known pos
            currentState = State.Search;
            searchTimer = data.searchingDelay;
            agent.isStopped = false;
            agent.SetDestination(lastKnownPlayerPos);
            return;
        }

        // distance check
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > data.attackRange)
        {
            // If player stepped out of strict attackRange, go back to chase
            currentState = State.Chase;
            agent.isStopped = false;
            return;
        }

        // Stop agent and rotate toward player smoothly
        agent.isStopped = true;
        RotateTowardsPlayer();

        if (attackTimer >= data.attackCooldown)
        {
            attackTimer = 0f;

            if (data.enemyType == EnemyType.MeleeCombatant)
            {
                // Hook melee attack here (animation & damage)
                Debug.Log("MELEE HIT!");
            }
            else if (data.enemyType == EnemyType.Shooter)
            {
                ShootProjectile();
            }
        }
    }

    void RotateTowardsPlayer()
    {
        if (player == null) return;
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        Quaternion target = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, attackRotateSpeed * Time.deltaTime);
    }

    void ApplyMovementRotation()
    {
        // Prefer agent.velocity, but if it's near zero (stuck/in corners), use desiredVelocity fallback
        Vector3 move = agent.velocity;
        if (move.sqrMagnitude < 0.0001f)
            move = agent.desiredVelocity;

        if (move.sqrMagnitude > 0.0001f)
        {
            move.y = 0f;
            Quaternion target = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * rotationLerpSpeed);
        }
    }

    void ShootProjectile()
    {
        if (data.projectilePrefab == null)
        {
            Debug.LogWarning("No projectile prefab assigned for shooter.");
            return;
        }

        // spawn a bit in front and a bit up
        Vector3 spawnPos = transform.position + transform.forward * 1f + Vector3.up * 1f;
        GameObject proj = Instantiate(data.projectilePrefab, spawnPos, Quaternion.identity);
        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // fire towards player's current position (flattened Y)
            Vector3 aim = player.position - spawnPos;
            aim.y = 0f;
            rb.velocity = aim.normalized * data.projectileSpeed;
        }

        if (data.gunshotSfx != null && data.gunshotSfx.Length > 0)
        {
            AudioSource.PlayClipAtPoint(data.gunshotSfx[Random.Range(0, data.gunshotSfx.Length)], transform.position);
        }
    }
}
