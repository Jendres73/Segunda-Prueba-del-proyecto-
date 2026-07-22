using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// V2_MovementController.cs - Controla el movimiento de unidades
/// Usa NavMeshAgent o Rigidbody2D según disponibilidad
/// </summary>
public class V2_MovementController : MonoBehaviour
{
    // ==================== REFERENCIAS ====================
    private Rigidbody2D rigidbodyComponentV2;
    private NavMeshAgent navMeshAgentV2;
    private V2_Unit ownerUnitV2;

    [Header("Movement Settings")]
    [SerializeField] private bool useNavMeshV2 = true;  // Si es true, usa NavMeshAgent; si es false, usa Rigidbody2D
    [SerializeField] private float moveSpeedV2 = 4f;
    [SerializeField] private float stoppingDistanceV2 = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = false;

    // ==================== ESTADO ====================
    private Vector3 moveDestinationV2;
    private bool isMovingV2 = false;

    // ==================== INICIALIZACIÓN ====================
    private void AwakeInitializeV2()
    {
        ownerUnitV2 = GetComponent<V2_Unit>();
        rigidbodyComponentV2 = GetComponent<Rigidbody2D>();
        navMeshAgentV2 = GetComponent<NavMeshAgent>();

        // Configurar NavMeshAgent si existe
        if (navMeshAgentV2 != null && useNavMeshV2)
        {
            navMeshAgentV2.speed = moveSpeedV2;
            navMeshAgentV2.stoppingDistance = stoppingDistanceV2;
            LogDebugV2("Using NavMeshAgent for movement");
        }
        else if (rigidbodyComponentV2 != null && !useNavMeshV2)
        {
            LogDebugV2("Using Rigidbody2D for movement");
        }
        else
        {
            LogDebugV2("No movement component found!", LogType.Warning);
        }
    }

    private void Awake()
    {
        AwakeInitializeV2();
    }

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Mueve la unidad hacia una posición
    /// </summary>
    public void MoveToPositionV2(Vector3 targetPositionV2)
    {
        moveDestinationV2 = targetPositionV2;
        isMovingV2 = true;

        if (useNavMeshV2 && navMeshAgentV2 != null && navMeshAgentV2.isOnNavMesh)
        {
            navMeshAgentV2.isStopped = false;
            navMeshAgentV2.SetDestination(targetPositionV2);
            LogDebugV2($"Moving to {targetPositionV2} using NavMeshAgent");
        }
        else if (!useNavMeshV2 && rigidbodyComponentV2 != null)
        {
            Vector3 directionV2 = (targetPositionV2 - transform.position).normalized;
            rigidbodyComponentV2.linearVelocity = (Vector2)directionV2 * moveSpeedV2;
            LogDebugV2($"Moving to {targetPositionV2} using Rigidbody2D");
        }
    }

    /// <summary>
    /// Detiene el movimiento
    /// </summary>
    public void StopMovementV2()
    {
        isMovingV2 = false;

        if (useNavMeshV2 && navMeshAgentV2 != null && navMeshAgentV2.isOnNavMesh)
        {
            navMeshAgentV2.isStopped = true;
            navMeshAgentV2.velocity = Vector3.zero;
        }
        else if (!useNavMeshV2 && rigidbodyComponentV2 != null)
        {
            rigidbodyComponentV2.linearVelocity = Vector2.zero;
        }

        LogDebugV2("Movement stopped");
    }

    /// <summary>
    /// Verifica si llegó al destino
    /// </summary>
    public bool HasReachedDestinationV2()
    {
        if (!isMovingV2)
            return true;

        if (useNavMeshV2 && navMeshAgentV2 != null && navMeshAgentV2.isOnNavMesh)
        {
            return !navMeshAgentV2.hasPath || navMeshAgentV2.remainingDistance <= stoppingDistanceV2;
        }
        else if (!useNavMeshV2)
        {
            float distanceV2 = Vector3.Distance(transform.position, moveDestinationV2);
            return distanceV2 <= stoppingDistanceV2;
        }

        return false;
    }

    /// <summary>
    /// Obtiene la distancia al destino
    /// </summary>
    public float GetDistanceToDestinationV2()
    {
        if (useNavMeshV2 && navMeshAgentV2 != null && navMeshAgentV2.isOnNavMesh)
        {
            return navMeshAgentV2.remainingDistance;
        }
        else
        {
            return Vector3.Distance(transform.position, moveDestinationV2);
        }
    }

    /// <summary>
    /// Establece la velocidad de movimiento
    /// </summary>
    public void SetMoveSpeedV2(float speedV2)
    {
        moveSpeedV2 = speedV2;

        if (useNavMeshV2 && navMeshAgentV2 != null)
        {
            navMeshAgentV2.speed = speedV2;
        }
    }

    /// <summary>
    /// Obtiene la velocidad actual de movimiento
    /// </summary>
    public float GetMoveSpeedV2()
    {
        return moveSpeedV2;
    }

    /// <summary>
    /// Verifica si está en movimiento
    /// </summary>
    public bool IsMovingV2()
    {
        return isMovingV2;
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Log de debug
    /// </summary>
    private void LogDebugV2(string messageV2, LogType typeV2 = LogType.Log)
    {
        if (!enableDebugLogsV2)
            return;

        switch (typeV2)
        {
            case LogType.Warning:
                Debug.LogWarning($"[V2_MovementController - {gameObject.name}] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_MovementController - {gameObject.name}] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_MovementController - {gameObject.name}] {messageV2}");
                break;
        }
    }
}