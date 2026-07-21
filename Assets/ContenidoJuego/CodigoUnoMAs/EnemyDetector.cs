using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public float visionRange = 5f;
    
    // Referencias
    private MonoBehaviour scriptPatrulla; // Aquí guardaremos tu script de patrulla
    private bool estaEnCombate = false;

    void Start()
    {
        // Esto busca automáticamente el script de patrulla en este mismo objeto
        scriptPatrulla = GetComponent<EnemyPatrol>(); 
    }

    void Update()
    {
        // 1. Detectar al jugador
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            // Hay un enemigo, PAUSA la patrulla
            if (scriptPatrulla != null) scriptPatrulla.enabled = false;
            
            // Aquí puedes activar tu lógica de persecución o ataque
            // ...
        }
        else
        {
            // No hay enemigo, REANUDA la patrulla
            if (scriptPatrulla != null) scriptPatrulla.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}