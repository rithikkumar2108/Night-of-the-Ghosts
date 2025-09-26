using SingularityGroup.HotReload;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    public enum EnemyState { Patrolling, Chasing, Shooting, Searching }
    private EnemyState currentState;
    public float Health = 100;
    public float Damage = 20f;
    private float startHealth;
    public Slider HealthSlider;

    [Header("References")]
    private Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Patrol")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    private float patrolTimer = 0f;
    private Vector3 startPos;
    public float patrolSpeed = 1.5f;    

    [Header("Sight")]
    public float fieldOfView = 100f;
    public float viewDistance = 25f;

    [Header("Movement")]
    public float chaseRange = 20f;
    public float stopDistance = 5f;
    public float moveSpeed = 3.5f;       
    public float rotationSpeed = 20f;

    [Header("Shooting")]
    public float shootingRange = 10f;
    public float fireRate = 1f;
    [Range(0, 0.3f)]
    public float SpreadRange;

    [Header("Search")]
    public float searchSpeed = 100f;
    public float searchDuration = 3f;
    private float searchTimer = 0f;
    private float nextFireTime = 0f;
    private NavMeshAgent agent;
    private Vector3 lastKnownPos;
    private bool hasLastKnownPos = false;

    private float pathUpdateRate = 0.2f;
    private float pathUpdateTimer = 0f;
    void Awake()
    {
        Debug.Log("Awake called!");

        SetStats();
    }
    public void SetStats()
    {
        if (EnemyStats.Instance != null)
        {
            Health = EnemyStats.Instance.baseHealth;
            Damage = EnemyStats.Instance.baseBulletDamage;
            fireRate = EnemyStats.Instance.baseFireRate;
            SpreadRange = EnemyStats.Instance.baseBulletSpread;
            moveSpeed = EnemyStats.Instance.baseSpeed;
        }
        else
        {
            Debug.LogWarning("No EnemyStats.Instance found! Using default inspector values.");
        }
    }
  
    void Start()
    {
        player = FindAnyObjectByType<EasyPeasyFirstPersonController.FirstPersonController>().transform;
        startHealth = Health;
        HealthSlider.transform.localScale = new Vector3(HealthSlider.transform.localScale.x, HealthSlider.transform.localScale.y / 100 * Health, HealthSlider.transform.localScale.z);
        HealthSlider.value = 1;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed; 
        startPos = transform.position;
        ChangeState(EnemyState.Patrolling);
    }
   
    void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CanSeePlayer();
        float distance = Vector3.Distance(transform.position, player.position);
        if (canSeePlayer)
        {
            lastKnownPos = player.position;
            hasLastKnownPos = true;

            if (distance <= shootingRange)
            {
                ChangeState(EnemyState.Shooting);
                ShootAtPlayer();
            }
            else if (distance <= chaseRange)
            {
                ChangeState(EnemyState.Chasing);
                ChasePlayer();
            }
        }
        else if (hasLastKnownPos)
        {
            if (Vector3.Distance(transform.position, lastKnownPos) > 1f)
            {
                ChangeState(EnemyState.Chasing);
                ChaseLastKnownPosition();
            }
            else
            {
                ChangeState(EnemyState.Searching);
                Search();
            }
        }
        else
        {
            ChangeState(EnemyState.Patrolling);
            Patrol();
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                Vector3 randomPoint;
                if (RandomNavmeshLocation(patrolRadius, out randomPoint))
                {
                    agent.isStopped = false;
                    agent.SetDestination(randomPoint);
                }
                patrolTimer = 0f;
            }
        }
    }

    bool RandomNavmeshLocation(float radius, out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += startPos;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    void ChasePlayer()
    {
        agent.speed = moveSpeed; 
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            pathUpdateTimer = pathUpdateRate;
        }
    }

    void ChaseLastKnownPosition()
    {
        agent.speed = moveSpeed;
        agent.isStopped = false;
        agent.SetDestination(lastKnownPos);
    }

    void ShootAtPlayer()
    {
        agent.isStopped = true;
        LookAtPlayer();

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Search()
    {
        agent.isStopped = true;
        searchTimer += Time.deltaTime;

        transform.Rotate(Vector3.up * searchSpeed * Time.deltaTime);

        if (searchTimer >= searchDuration)
        {
            hasLastKnownPos = false;
            searchTimer = 0f;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfView * 0.5f) return false;

        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewDistance))
        {
            if (hit.transform == player) return true;
        }
        return false;
    }

    void LookAtPlayer()
    {
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    void Shoot()
    {
        if (firePoint != null && projectilePrefab != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = proj.GetComponent<Bullet>();
            bullet.Damage = Damage;
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir;
                dir.x = (player.position - firePoint.position).normalized.x + Random.Range(-SpreadRange, SpreadRange);
                dir.y = (player.position - firePoint.position).normalized.y + Random.Range(-SpreadRange, SpreadRange);
                dir.z = (player.position - firePoint.position).normalized.z;
                rb.linearVelocity = dir * 15f;
            }
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log(name + " state changed → " + currentState);
    }

    public void TakeDamage(float d) {


        Debug.Log("Enemy Hit");
        Health -= d;
        HealthSlider.value = Health / startHealth;
        if (Health > 0)
        {
            lastKnownPos = player.position;
            hasLastKnownPos = true;
            ChangeState(EnemyState.Chasing);
            ChasePlayer();
        }
        else
        {
            Die();
        }
    }
    void Die() {
        WaveGenerator.potentialEnemies -= 1;
        Destroy(gameObject);

    }
}
