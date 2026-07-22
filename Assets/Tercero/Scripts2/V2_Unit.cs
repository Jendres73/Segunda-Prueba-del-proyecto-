using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// V2_Unit.cs - Clase base abstracta para todas las unidades del juego (ACTUALIZADA)
/// Ahora usa V2_MovementController para el movimiento
/// </summary>
public abstract class V2_Unit : MonoBehaviour
{
    // ==================== REFERENCIAS A COMPONENTES ====================
    protected Rigidbody2D rigidbodyComponentV2;
    protected Animator animatorComponentV2;
    protected Collider2D colliderComponentV2;
    protected NavMeshAgent navMeshAgentV2;
    protected V2_HealthComponent healthComponentV2;
    protected V2_MovementController movementControllerV2;

    // ==================== VARIABLES DE CONFIGURACIÓN ====================
    [Header("Unit Configuration")]
    [SerializeField] protected TeamAffiliation teamAffiliationV2 = TeamAffiliation.Player;
    [SerializeField] protected float moveSpeedV2 = V2_Constants.PLAYER_MOVE_SPEED_V2;
    [SerializeField] protected float visionRangeV2 = V2_Constants.VISION_RANGE_V2;
    [SerializeField] protected float attackRangeV2 = V2_Constants.ATTACK_RANGE_V2;
    [SerializeField] protected float attackDamageV2 = V2_Constants.ATTACK_DAMAGE_PLAYER_V2;
    [SerializeField] protected float maxHealthV2 = V2_Constants.PLAYER_MAX_HEALTH_V2;

    [Header("Debug")]
    [SerializeField] protected bool enableDebugV2 = false;

    // ==================== VARIABLES DE ESTADO ====================
    protected UnitState currentUnitStateV2 = UnitState.Idle;
    protected Transform targetEnemyV2;
    protected Vector3 moveDestinationV2;
    protected float attackCooldownTimerV2 = 0f;
    protected bool isAliveV2 = true;

    // ==================== EVENTOS ====================
    public event System.Action<UnitState> OnStateChangedV2;
    public event System.Action OnUnitDeathV2;
    public event System.Action<float> OnHealthChangedV2;

    // ==================== PROPIEDADES ====================
    public UnitState CurrentStateV2 => currentUnitStateV2;
    public TeamAffiliation TeamV2 => teamAffiliationV2;
    public bool IsAliveV2 => isAliveV2;
    public Transform TargetEnemyV2 => targetEnemyV2;
    public float CurrentHealthV2 => healthComponentV2 != null ? healthComponentV2.GetCurrentHealthV2() : maxHealthV2;

    // ==================== UNITY LIFECYCLE ====================
    protected virtual void Awake()
    {
        AwakeInitializeComponentsV2();
    }

    protected virtual void Start()
    {
        StartInitializeUnitV2();
    }

    protected virtual void Update()
    {
        if (!isAliveV2)
            return;

        UpdateAttackCooldownV2();
        UpdateBehaviorV2();
    }

    protected virtual void OnDestroy()
    {
        if (V2_GameManager.InstanceV2 != null)
        {
            V2_GameManager.InstanceV2.UnregisterUnitV2(this);
        }
    }

    // ==================== INITIALIZATION ====================
    private void AwakeInitializeComponentsV2()
    {
        // Obtener referencias a componentes
        rigidbodyComponentV2 = GetComponent<Rigidbody2D>();
        animatorComponentV2 = GetComponent<Animator>();
        colliderComponentV2 = GetComponent<Collider2D>();
        navMeshAgentV2 = GetComponent<NavMeshAgent>();
        healthComponentV2 = GetComponent<V2_HealthComponent>();
        movementControllerV2 = GetComponent<V2_MovementController>();

        // Crear V2_HealthComponent si no existe
        if (healthComponentV2 == null)
        {
            healthComponentV2 = gameObject.AddComponent<V2_HealthComponent>();
        }

        // Crear V2_MovementController si no existe
        if (movementControllerV2 == null)
        {
            movementControllerV2 = gameObject.AddComponent<V2_MovementController>();
        }

        LogDebugV2($"Unit components initialized: {gameObject.name}");
    }

    private void StartInitializeUnitV2()
    {
        // Registrar en el GameManager
        if (V2_GameManager.InstanceV2 != null)
        {
            V2_GameManager.InstanceV2.RegisterUnitV2(this);
        }

        // Suscribirse a eventos de salud
        if (healthComponentV2 != null)
        {
            healthComponentV2.OnHealthChangedV2 += HandleHealthChangedV2;
            healthComponentV2.OnDeathV2 += HandleUnitDeathV2;
        }

        // Inicializar salud
        if (healthComponentV2 != null)
        {
            healthComponentV2.InitializeHealthV2(maxHealthV2);
        }

        // Configurar velocidad de movimiento en el controller
        if (movementControllerV2 != null)
        {
            movementControllerV2.SetMoveSpeedV2(moveSpeedV2);
        }

        OnStateChangedV2?.Invoke(currentUnitStateV2);
        LogDebugV2($"Unit started: {gameObject.name} - Team: {teamAffiliationV2}");
    }

    // ==================== PUBLIC METHODS ====================
    
    /// <summary>
    /// Obtiene la afiliación de equipo de la unidad
    /// </summary>
    public TeamAffiliation GetTeamAffiliationV2()
    {
        return teamAffiliationV2;
    }

    /// <summary>
    /// Cambiar estado de la unidad
    /// </summary>
    public void SetStateV2(UnitState newStateV2)
    {
        if (currentUnitStateV2 != newStateV2)
        {
            currentUnitStateV2 = newStateV2;
            OnStateChangedV2?.Invoke(currentUnitStateV2);
            LogDebugV2($"State changed to: {currentUnitStateV2}");
        }
    }

    /// <summary>
    /// Causa daño a la unidad
    /// </summary>
    public virtual void TakeDamageV2(float damageAmountV2, DamageType damageTypeV2 = DamageType.Physical)
    {
        if (!isAliveV2)
            return;

        if (healthComponentV2 != null)
        {
            healthComponentV2.TakeDamageV2(damageAmountV2);
            LogDebugV2($"Unit took {damageAmountV2} damage. Health: {healthComponentV2.GetCurrentHealthV2()}");
        }
    }

    /// <summary>
    /// Cura a la unidad
    /// </summary>
    public virtual void HealV2(float healAmountV2)
    {
        if (!isAliveV2)
            return;

        if (healthComponentV2 != null)
        {
            healthComponentV2.HealV2(healAmountV2);
            LogDebugV2($"Unit healed by {healAmountV2}. Health: {healthComponentV2.GetCurrentHealthV2()}");
        }
    }

    /// <summary>
    /// Detiene el movimiento de la unidad
    /// </summary>
    public virtual void StopMovementV2()
    {
        if (movementControllerV2 != null)
        {
            movementControllerV2.StopMovementV2();
        }
        else if (rigidbodyComponentV2 != null)
        {
            rigidbodyComponentV2.linearVelocity = Vector2.zero;
        }

        SetStateV2(UnitState.Idle);
    }

    /// <summary>
    /// Obtiene la distancia a un objetivo
    /// </summary>
    public float GetDistanceToTargetV2(Transform targetV2)
    {
        if (targetV2 == null)
            return float.MaxValue;

        return Vector3.Distance(transform.position, targetV2.position);
    }

    // ==================== PROTECTED ABSTRACT METHODS ====================
    
    /// <summary>
    /// Método abstracto para actualizar el comportamiento específico de cada unidad
    /// </summary>
    protected abstract void UpdateBehaviorV2();

    /// <summary>
    /// Detectar enemigos cercanos
    /// </summary>
    protected abstract void DetectEnemiesV2();

    /// <summary>
    /// Atacar al enemigo
    /// </summary>
    protected abstract void AttackEnemyV2();

    // ==================== PROTECTED METHODS ====================

    /// <summary>
    /// Detecta enemigos en el rango de visión
    /// </summary>
    protected void DetectEnemiesInRangeV2()
    {
        string enemyLayerNameV2 = teamAffiliationV2 == TeamAffiliation.Player ? "Enemy" : "Player";
        LayerMask enemyLayerMaskV2 = LayerMask.GetMask(enemyLayerNameV2);
        
        Collider2D hitV2 = Physics2D.OverlapCircle(transform.position, visionRangeV2, enemyLayerMaskV2);
        targetEnemyV2 = (hitV2 != null) ? hitV2.transform : null;

        if (targetEnemyV2 != null)
        {
            LogDebugV2($"Enemy detected: {targetEnemyV2.gameObject.name}");
        }
    }

    /// <summary>
    /// Perseguir al enemigo objetivo
    /// </summary>
    protected void PursueTargetEnemyV2()
    {
        if (targetEnemyV2 == null)
            return;

        SetStateV2(UnitState.Moving);
        MoveTowardsPositionV2(targetEnemyV2.position);
    }

    /// <summary>
    /// Moverse hacia una posición
    /// </summary>
    protected void MoveTowardsPositionV2(Vector3 targetPositionV2)
    {
        if (!isAliveV2)
            return;

        if (movementControllerV2 != null)
        {
            movementControllerV2.MoveToPositionV2(targetPositionV2);
        }
    }

    /// <summary>
    /// Actualizar cooldown de ataque
    /// </summary>
    protected void UpdateAttackCooldownV2()
    {
        if (attackCooldownTimerV2 > 0)
        {
            attackCooldownTimerV2 -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Verificar si puede atacar (cooldown completado)
    /// </summary>
    protected bool CanAttackV2()
    {
        return attackCooldownTimerV2 <= 0;
    }

    /// <summary>
    /// Aplicar daño a un objetivo
    /// </summary>
    protected void DealDamageToTargetV2(V2_Unit targetUnitV2)
    {
        if (targetUnitV2 != null && CanAttackV2())
        {
            targetUnitV2.TakeDamageV2(attackDamageV2);
            attackCooldownTimerV2 = V2_Constants.ATTACK_COOLDOWN_V2;
            LogDebugV2($"Attacked {targetUnitV2.gameObject.name} for {attackDamageV2} damage");
        }
    }

    /// <summary>
    /// Manejar la muerte de la unidad
    /// </summary>
    protected virtual void HandleUnitDeathV2()
    {
        isAliveV2 = false;
        SetStateV2(UnitState.Dead);
        StopMovementV2();
        
        // Deshabilitar colisiones
        if (colliderComponentV2 != null)
        {
            colliderComponentV2.enabled = false;
        }

        // Reproducir animación de muerte
        if (animatorComponentV2 != null)
        {
            animatorComponentV2.SetTrigger("isDeath");
        }

        LogDebugV2($"Unit died: {gameObject.name}");
        OnUnitDeathV2?.Invoke();

        // Destruir la unidad después de un tiempo
        Destroy(gameObject, V2_Constants.DEATH_ANIMATION_TIME_V2);
    }

    /// <summary>
    /// Manejar cambios de salud
    /// </summary>
    private void HandleHealthChangedV2(float newHealthV2)
    {
        OnHealthChangedV2?.Invoke(newHealthV2);
    }

    /// <summary>
    /// Log de debug
    /// </summary>
    protected void LogDebugV2(string messageV2)
    {
        if (!enableDebugV2)
            return;

        Debug.Log($"[{gameObject.name}] {messageV2}");
    }

    // ==================== GIZMOS ====================
    protected virtual void OnDrawGizmosSelected()
    {
        // Dibujar rango de visión
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, visionRangeV2);

        // Dibujar rango de ataque
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRangeV2);
    }
}