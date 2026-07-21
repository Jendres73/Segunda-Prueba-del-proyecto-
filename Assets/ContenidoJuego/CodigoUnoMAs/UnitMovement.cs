using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    public float speed = 5f;
    public bool moving = false;
    public bool isSelected = false; 
    
    private NavMeshAgent agent;
    private Animator anim;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        // CORRECCIÓN: Eliminamos la condición que detenía la unidad al perder selección.
        // Ahora la unidad sigue moviéndose hasta llegar al destino.

        // Detección de llegada al destino
        if (moving && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StopMovement();
        }

        // Actualizar animaciones y dirección visual
        if (anim != null) anim.SetBool("isWalking", moving);
        if (moving) FlipSprite();
    }

    void FlipSprite()
    {
        if (agent.velocity.x > 0.1f) sr.flipX = false;
        else if (agent.velocity.x < -0.1f) sr.flipX = true;
    }

    public void StopMovement()
    {
        moving = false;
        if (agent.enabled) 
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        // Al detenerse, devolvemos el control al controlador de combate
        UnitController controller = GetComponent<UnitController>();
        if (controller != null)
        {
            controller.ResumeCombatControl();
        }
    }

    public void SetNewDestination(Vector3 newPos)
    {
        // Solo moverse si la unidad está seleccionada al recibir la orden
        if (isSelected && agent.enabled)
        {
            UnitController controller = GetComponent<UnitController>();
            if (controller != null)
            {
                // Tomamos control y forzamos el agente a moverse
                controller.isPlayerCommanding = true;
                agent.isStopped = false; 
            }

            agent.SetDestination(newPos);
            moving = true;
        }
    }
}