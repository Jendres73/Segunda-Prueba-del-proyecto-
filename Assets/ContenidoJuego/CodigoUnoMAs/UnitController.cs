using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    [Header("Atributos")]
    public float moveSpeed = 3f;
    public float visionRange = 5f;
    public float attackRange = 1f;
    public float attackDamage = 10f;
    public float attackRate = 1f;

    [Header("Equipos")]
    public bool isPlayerUnit;
    public bool isPlayerCommanding = false;

    private Transform targetToPursue;
    private float nextAttackTime;
    
    private NavMeshAgent agent;
    private HealthComponent myHealth;
    private Animator anim;

    void Start()
    {
        myHealth = GetComponent<HealthComponent>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        agent.speed = moveSpeed;
        agent.acceleration = 100f;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        // Si el jugador está mandando, el combate está totalmente pausado
        if (isPlayerCommanding) return;

        if (myHealth != null && myHealth.currentHealth <= 0) 
        {
            if (agent.enabled) agent.isStopped = true;
            return;
        }

        DetectEnemy();

        if (targetToPursue != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetToPursue.position);

            if (distanceToTarget <= attackRange)
            {
                StopMoving();
                CombatBehavior();
            }
            else
            {
                PursueTarget();
            }
        }
    }

    public void ResumeCombatControl()
    {
        isPlayerCommanding = false;
    }

    void DetectEnemy()
    {
        string enemyLayerName = isPlayerUnit ? "Enemy" : "Player";
        LayerMask enemyLayer = LayerMask.GetMask(enemyLayerName);
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, enemyLayer);
        targetToPursue = (hit != null) ? hit.transform : null;
    }

    void PursueTarget()
    {
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(targetToPursue.position);
        }
    }

    void StopMoving()
    {
        if (agent.enabled) agent.isStopped = true;
    }

    void CombatBehavior()
    {
        if (targetToPursue == null) return;
        
        if (Time.time >= nextAttackTime)
        {
            if (anim != null) anim.SetTrigger("isAttacking");

            HealthComponent enemyHealth = targetToPursue.GetComponent<HealthComponent>();
            if (enemyHealth != null) enemyHealth.TakeDamage(attackDamage);

            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}