using UnityEngine;

/// <summary>
/// V2_PlayerWarrior.cs - Unidad Guerrera del jugador
/// Especializada en combate, NO recolecta recursos
/// TODOS los valores son configurables en el Inspector
/// </summary>
public class V2_PlayerWarrior : V2_Unit
{
    // ==================== CONFIGURACIÓN EDITABLE EN INSPECTOR ====================
    [Header("⚔️ WARRIOR CONFIGURATION - Edita estos valores en el Inspector")]
    
    [SerializeField] private float warriorMoveSpeedV2 = 4f;
    [SerializeField] private float warriorAttackDamageV2 = 15f;
    [SerializeField] private float warriorMaxHealthV2 = 100f;
    [SerializeField] private float warriorVisionRangeV2 = 7f;
    [SerializeField] private float warriorAttackRangeV2 = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool enableWarriorDebugV2 = false;

    // ==================== VARIABLES DE ESTADO ====================
    private bool isPlayerCommandingV2 = false;
    private Vector3 commandedMoveDestinationV2;

    // ==================== PROPIEDADES ====================
    public bool IsCommandingV2 => isPlayerCommandingV2;

    // ==================== INICIALIZACIÓN ====================
    protected override void Awake()
    {
        base.Awake();
        InitializeWarriorV2();
    }

    protected override void Start()
    {
        base.Start();
        
        // Aplicar configuración del Guerrero
        moveSpeedV2 = warriorMoveSpeedV2;
        attackDamageV2 = warriorAttackDamageV2;
        maxHealthV2 = warriorMaxHealthV2;
        visionRangeV2 = warriorVisionRangeV2;
        attackRangeV2 = warriorAttackRangeV2;
        teamAffiliationV2 = TeamAffiliation.Player;

        if (healthComponentV2 != null)
        {
            healthComponentV2.InitializeHealthV2(warriorMaxHealthV2);
        }

        LogDebugV2($"Warrior initialized - Damage: {warriorAttackDamageV2}, Health: {warriorMaxHealthV2}, Speed: {warriorMoveSpeedV2}");
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
            // Sin comando: detectar y combatir enemigos
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
        float distanceToDestinationV2 = Vector3.Distance(transform.position, commandedMoveDestinationV2);

        if (distanceToDestinationV2 > 0.2f)
        {
            SetStateV2(UnitState.Moving);
            MoveTowardsPositionV2(commandedMoveDestinationV2);
        }
        else
        {
            // Llegó al destino
            isPlayerCommandingV2 = false;
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

            LogDebugV2($"Warrior attacked {enemyUnitV2.gameObject.name} for {attackDamageV2} damage");
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
        LogDebugV2($"Move command: {targetPositionV2}");
    }

    /// <summary>
    /// Cancela el comando actual
    /// </summary>
    public void CancelCommandV2()
    {
        isPlayerCommandingV2 = false;
        SetStateV2(UnitState.Idle);
        StopMovementV2();
        LogDebugV2("Command cancelled");
    }

    // ==================== PRIVADO ====================

    private void InitializeWarriorV2()
    {
        LogDebugV2("PlayerWarrior initialized");
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
        if (!enableWarriorDebugV2)
            return;

        Debug.Log($"[⚔️ {gameObject.name}] {messageV2}");
    }
}