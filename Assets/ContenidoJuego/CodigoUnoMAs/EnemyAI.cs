using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrulla")]
    public float speed = 2f;
    public float patrolRadius = 5f;
    public float waitTime = 2f;
    private Vector3 startPosition;
    private Vector3 targetPos;
    private float waitTimer;
    private bool isWaiting = false;

    [Header("Combate y Visión")]
    public float visionRange = 5f;
    public float attackRange = 1f;
    public float attackDamage = 10f;
    public float attackRate = 1f;
    private float nextAttackTime;

    private Transform targetEnemy;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        GetNewRandomPosition();
    }

    void Update()
    {
        // 1. Siempre buscamos al jugador primero
        DetectPlayer();

        if (targetEnemy != null)
        {
            // SI VEMOS AL JUGADOR: Perseguir y atacar
            float dist = Vector2.Distance(transform.position, targetEnemy.position);
            if (dist <= attackRange)
            {
                rb.linearVelocity = Vector2.zero; // Detenerse al atacar
                Attack();
            }
            else
            {
                Vector3 dir = (targetEnemy.position - transform.position).normalized;
                rb.linearVelocity = dir * speed;
            }
        }
        else
        {
            // SI NO VEMOS AL JUGADOR: Patrullar
            Patrol();
        }
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, LayerMask.GetMask("Player"));
        targetEnemy = (hit != null) ? hit.transform : null;
    }

    void Patrol()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0) { GetNewRandomPosition(); isWaiting = false; }
        }
        else
        {
            rb.linearVelocity = (targetPos - transform.position).normalized * speed;
            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                rb.linearVelocity = Vector2.zero;
                isWaiting = true;
                waitTimer = waitTime;
            }
        }
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            targetEnemy.GetComponent<HealthComponent>().TakeDamage(attackDamage);
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void GetNewRandomPosition()
    {
        targetPos = startPosition + new Vector3(Random.Range(-patrolRadius, patrolRadius), Random.Range(-patrolRadius, patrolRadius), 0);
    }
}