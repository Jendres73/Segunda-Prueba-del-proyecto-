using UnityEngine;

/// <summary>
/// V2_PlayerCollector.cs - Unidad Recolectora del jugador
/// Especializada en recolectar recursos, también puede atacar
/// TODOS los valores son configurables en el Inspector
/// </summary>
public class V2_PlayerCollector : V2_Unit
{
    // ==================== CONFIGURACIÓN EDITABLE EN INSPECTOR ====================
    [Header("🎒 COLLECTOR CONFIGURATION - Edita estos valores en el Inspector")]
    
    [SerializeField] private float collectorMoveSpeedV2 = 3.5f;
    [SerializeField] private float collectorAttackDamageV2 = 8f;
    [SerializeField] private float collectorMaxHealthV2 = 80f;
    [SerializeField] private float collectorVisionRangeV2 = 6f;
    [SerializeField] private float collectorAttackRangeV2 = 1f;
    
    [Header("Recolección")]
    [SerializeField] private float collectionRangeV2 = 1.5f;
    [SerializeField] private float collectionSpeedV2 = 1f;
    [SerializeField] private int maxCarryAmountV2 = 50;

    [Header("Debug")]
    [SerializeField] private bool enableCollectorDebugV2 = false;


    // ==================== COMPONENTES ====================
    private V2_Collector collectorComponentV2;

    // ==================== VARIABLES DE ESTADO ====================
    private bool isPlayerCommandingV2 = false;
    private Vector3 commandedMoveDestinationV2;
    private Transform commandedResourceTargetV2;
    private bool shouldCollectResourceV2 = false;
    private bool isAttackingResourceV2 = false;

    // ==================== PROPIEDADES ====================
    public bool IsCommandingV2 => isPlayerCommandingV2;
    public int CarriedAmountV2 => collectorComponentV2 != null ? collectorComponentV2.CurrentCarriedAmountV2 : 0;

    // ==================== INICIALIZACIÓN ====================
    protected override void Awake()
    {
        base.Awake();
        InitializeCollectorV2();
    }

    protected override void Start()
    {
        base.Start();
        
        // Aplicar configuración del Recolector
        moveSpeedV2 = collectorMoveSpeedV2;
        attackDamageV2 = collectorAttackDamageV2;
        maxHealthV2 = collectorMaxHealthV2;
        visionRangeV2 = collectorVisionRangeV2;
        attackRangeV2 = collectorAttackRangeV2;
        teamAffiliationV2 = TeamAffiliation.Player;

        if (healthComponentV2 != null)
        {
            healthComponentV2.InitializeHealthV2(collectorMaxHealthV2);
        }

        LogDebugV2($"Collector initialized - Damage: {collectorAttackDamageV2}, Health: {collectorMaxHealthV2}, Speed: {collectorMoveSpeedV2}");
    }

    // ==================== COMPORTAMIENTO ====================
    protected override void UpdateBehaviorV2()
    {
        if (!isAliveV2)
            return;

        if (isPlayerCommandingV2)
        {
            UpdatePlayerCommandV2();
        }
        else
        {
            // Sin comando: detectar enemigos
            DetectEnemiesV2();
            
            if (targetEnemyV2 != null)
            {
                float distanceToEnemyV2 = GetDistanceToTargetV2(targetEnemyV2);
                
                if (distanceToEnemyV2 <= attackRangeV2)
                {
                    SetStateV2(UnitState.Attacking);
                    AttackEnemyV2();
                }
                else
                {
                    SetStateV2(UnitState.Moving);
                    PursueTargetEnemyV2();
                }
            }
            else
            {
                if (currentUnitStateV2 != UnitState.Idle)
                {
                    SetStateV2(UnitState.Idle);
                    StopMovementV2();
                }
            }
        }
    }

    /// <summary>
    /// Actualiza el comportamiento cuando el jugador da un comando
    /// </summary>
    private void UpdatePlayerCommandV2()
    {
        // Si está recolectando
        if (shouldCollectResourceV2 && commandedResourceTargetV2 != null)
        {
            float distanceToResourceV2 = GetDistanceToTargetV2(commandedResourceTargetV2);

            if (distanceToResourceV2 <= collectionRangeV2)
            {
                SetStateV2(UnitState.Collecting);
                if (collectorComponentV2 != null)
                {
                    collectorComponentV2.CollectFromResourceV2(commandedResourceTargetV2);
                }
            }
            else
            {
                SetStateV2(UnitState.Moving);
                MoveTowardsPositionV2(commandedResourceTargetV2.position);
            }

            return;
        }

        // Si está siguiendo comando de movimiento
        float distanceToDestinationV2 = Vector3.Distance(transform.position, commandedMoveDestinationV2);

        if (distanceToDestinationV2 > 0.2f)
        {
            SetStateV2(UnitState.Moving);
            MoveTowardsPositionV2(commandedMoveDestinationV2);
        }
        else
        {
            isPlayerCommandingV2 = false;
            shouldCollectResourceV2 = false;
            SetStateV2(UnitState.Idle);
            StopMovementV2();
            LogDebugV2("Reached destination");
        }
    }

    protected override void DetectEnemiesV2()
    {
        DetectEnemiesInRangeV2();
    }

    protected override void AttackEnemyV2()
    {
        if (targetEnemyV2 == null)
            return;

        V2_Unit enemyUnitV2 = targetEnemyV2.GetComponent<V2_Unit>();

        if (enemyUnitV2 != null && CanAttackV2())
        {
            DealDamageToTargetV2(enemyUnitV2);
            
            if (animatorComponentV2 != null)
            {
                animatorComponentV2.SetTrigger("isAttacking");
            }

            LogDebugV2($"Collector attacked {enemyUnitV2.gameObject.name} for {attackDamageV2} damage");
        }
    }

    // ==================== COMANDOS DEL JUGADOR ====================

    /// <summary>
    /// Ejecuta comando de movimiento
    /// </summary>
    public void ExecuteMoveCommandV2(Vector3 targetPositionV2)
    {
        commandedMoveDestinationV2 = targetPositionV2;
        isPlayerCommandingV2 = true;
        shouldCollectResourceV2 = false;
        commandedResourceTargetV2 = null;

        LogDebugV2($"Move command: {targetPositionV2}");
    }

    /// <summary>
    /// Ejecuta comando de recolectar
    /// </summary>
    public void ExecuteCollectCommandV2(Transform resourceTransformV2)
    {
        if (resourceTransformV2 == null)
            return;

        commandedResourceTargetV2 = resourceTransformV2;
        isPlayerCommandingV2 = true;
        shouldCollectResourceV2 = true;
        commandedMoveDestinationV2 = resourceTransformV2.position;

        LogDebugV2($"Collect command: {resourceTransformV2.gameObject.name}");
    }

    /// <summary>
    /// Cancela el comando actual
    /// </summary>
    public void CancelCommandV2()
    {
        isPlayerCommandingV2 = false;
        shouldCollectResourceV2 = false;
        commandedResourceTargetV2 = null;
        SetStateV2(UnitState.Idle);
        StopMovementV2();

        if (collectorComponentV2 != null)
        {
            collectorComponentV2.StopCollectingV2();
        }

        LogDebugV2("Command cancelled");
    }

    // ==================== PRIVADO ====================

    private void InitializeCollectorV2()
    {
        collectorComponentV2 = GetComponent<V2_Collector>();

        if (collectorComponentV2 == null)
        {
            LogDebugV2("V2_Collector component not found, creating one...");
            collectorComponentV2 = gameObject.AddComponent<V2_Collector>();
        }

        LogDebugV2("PlayerCollector initialized");
    }

    public override void TakeDamageV2(float damageAmountV2, DamageType damageTypeV2 = DamageType.Physical)
    {
        base.TakeDamageV2(damageAmountV2, damageTypeV2);
        
        if (animatorComponentV2 != null)
        {
            animatorComponentV2.SetTrigger("takingDamage");
        }
    }

    private void LogDebugV2(string messageV2)
    {
        if (!enableCollectorDebugV2)
            return;

        Debug.Log($"[🎒 {gameObject.name}] {messageV2}");
    }
}