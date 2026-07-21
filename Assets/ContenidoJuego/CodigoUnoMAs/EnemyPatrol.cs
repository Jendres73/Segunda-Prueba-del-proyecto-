using UnityEngine;
using UnityEngine.AI; // NECESARIO para NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float patrolRadius = 5f;
    public float waitTime = 2f;
    public float timeToReturn = 10f; 
    
    private float returnTimer;
    private Vector3 startPosition;
    private Vector3 targetPos;
    private float waitTimer;
    private bool isWaiting = false;
    private bool returningHome = false;
    
    public bool estaPatrullando = true; 
    
    private NavMeshAgent agent; // Sustituye a la lógica manual
    private SpriteRenderer sr;
    private Animator anim; 

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        startPosition = transform.position; 
        returnTimer = timeToReturn;
        
        // Configuración del Agente
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false; // Mantiene el sprite estable
        agent.updateUpAxis = false;
        
        GetNewRandomPosition();
    }

    void Update()
    {
        if (!estaPatrullando) 
        {
            if (agent.enabled) agent.isStopped = true;
            if (anim != null) anim.SetBool("isWalking", false);
            return;
        }

        returnTimer -= Time.deltaTime;

        if (returnTimer <= 0 && !returningHome)
        {
            returningHome = true;
            targetPos = startPosition; 
            agent.SetDestination(targetPos);
        }

        if (isWaiting)
        {
            if (anim != null) anim.SetBool("isWalking", false);
            waitTimer -= Time.deltaTime;
            
            if (waitTimer <= 0)
            {
                if (returningHome) 
                {
                    returningHome = false;
                    returnTimer = timeToReturn;
                }
                GetNewRandomPosition();
                isWaiting = false;
            }
        }
        else
        {
            if (anim != null) anim.SetBool("isWalking", true);
            FlipSprite();

            // Comprobamos si el agente ha llegado al destino de forma segura
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }
        }
    }

    void FlipSprite()
    {
        if (agent.velocity.x > 0.1f) sr.flipX = false;
        else if (agent.velocity.x < -0.1f) sr.flipX = true;
    }

    void GetNewRandomPosition()
    {
        if (!returningHome)
        {
            float randomX = Random.Range(-patrolRadius, patrolRadius);
            float randomY = Random.Range(-patrolRadius, patrolRadius);
            targetPos = startPosition + new Vector3(randomX, randomY, 0);
            
            // Validamos que el punto sea alcanzable por el NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            agent.SetDestination(targetPos);
        }
    }
}