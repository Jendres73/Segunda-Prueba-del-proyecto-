using UnityEngine;

/// <summary>
/// V2_EnemyWarrior.cs - Unidad enemiga guerrera con IA automática
/// Patrulla, persigue y ataca
/// TODOS los valores son configurables en el Inspector
/// </summary>
public class V2_EnemyWarrior : V2_Unit
{
    // ==================== CONFIGURACIÓN EDITABLE EN INSPECTOR ====================
    [Header("⚔️ ENEMY WARRIOR CONFIGURATION - Edita estos valores en el Inspector")]
    
    [SerializeField] private float enemyMoveSpeedV2 = 2.5f;
    [SerializeField] private float enemyAttackDamageV2 = 10f;
    [SerializeField] private float enemyMaxHealthV2 = 50f;
    [SerializeField] private float enemyVisionRangeV2 = 7f;
    [SerializeField] private float enemyAttackRangeV2 = 1.5f;

    [Header("Patrulla")]
    [SerializeField] private float patrolRadiusV2 = 10f;
    [SerializeField] private float patrolWaitTimeV2 = 2f;

    [Header("Debug")]
[SerializeField] private bool enableEnemyDebugV2 = false;

    // ==================== VARIABLES DE ESTADO ====================
    private Vector3 patrolStartPositionV2;
    private Vector3 patrolTargetPositionV2;
    private float patrolWaitTimerV2 = 0f;
    private bool isWaitingAtPatrolPointV2 = false;
    private AIState currentAIStateV2 = AIState.Patrolling;

    // ==================== ENUM PARA ESTADOS DE IA ====================
    private enum AIState
    {
        Patrolling,
        Pursuing,
        Attacking
    }

    // ==================== INICIALIZACIÓN ====================
    protected override void Awake()
    {
        base.Awake();
        InitializeEnemyWarriorV2();
    }

    protected override void Start()
    {
        base.Start();
        
        // Aplicar configuración del Enemigo
        moveSpeedV2 = enemyMoveSpeedV2;
        attackDamageV2 = enemyAttackDamageV2;
        maxHealthV2 = enemyMaxHealthV2;
        visionRangeV2 = enemyVisionRangeV2;
        attackRangeV2 = enemyAttackRangeV2;
        teamAffiliationV2 = TeamAffiliation.Enemy;

        if (healthComponentV2 != null)
        {
            healthComponentV2.InitializeHealthV2(enemyMaxHealthV2);
        }

        patrolStartPositionV2 = transform.position;
        GetNewPatrolTargetV2();

        LogDebugV2($"Enemy Warrior initialized - Damage: {enemyAttackDamageV2}, Health: {enemyMaxHealthV2}, Speed: {enemyMoveSpeedV2}");
    }

    // ==================== COMPORTAMIENTO ====================
    protected override void UpdateBehaviorV2()
    {
        if (!isAliveV2)
            return;

        DetectEnemiesV2();

        if (targetEnemyV2 != null)
        {
            // Jugador detectado
            float distanceToPlayerV2 = GetDistanceToTargetV2(targetEnemyV2);

            if (distanceToPlayerV2 <= attackRangeV2)
            {
                // Atacar
                currentAIStateV2 = AIState.Attacking;
                SetStateV2(UnitState.Attacking);
                StopMovementV2();
                AttackEnemyV2();
            }
            else
            {
                // Perseguir
                currentAIStateV2 = AIState.Pursuing;
                SetStateV2(UnitState.Moving);
                PursueTargetEnemyV2();
            }
        }
        else
        {
            // Jugador no detectado, patrullar
            currentAIStateV2 = AIState.Patrolling;
            PatrolBehaviorV2();
        }
    }

    /// <summary>
    /// Comportamiento de patrulla
    /// </summary>
    private void PatrolBehaviorV2()
    {
        if (isWaitingAtPatrolPointV2)
        {
            // Esperando en punto de patrulla
            SetStateV2(UnitState.Idle);
            StopMovementV2();
            
            patrolWaitTimerV2 -= Time.deltaTime;
            if (patrolWaitTimerV2 <= 0)
            {
                isWaitingAtPatrolPointV2 = false;
                GetNewPatrolTargetV2();
            }
        }
        else
        {
            // Moviéndose a nuevo punto de patrulla
            SetStateV2(UnitState.Moving);
            float distanceToPatrolTargetV2 = Vector3.Distance(transform.position, patrolTargetPositionV2);

            if (distanceToPatrolTargetV2 < 0.5f)
            {
                // Llegó al punto de patrulla
                isWaitingAtPatrolPointV2 = true;
                patrolWaitTimerV2 = patrolWaitTimeV2;
            }
            else
            {
                // Seguir moviéndose al punto
                MoveTowardsPositionV2(patrolTargetPositionV2);
            }
        }
    }

    protected override void DetectEnemiesV2()
    {
        // Detectar jugador
        LayerMask playerLayerMaskV2 = LayerMask.GetMask(V2_Constants.LAYER_PLAYER_V2);
        Collider2D hitV2 = Physics2D.OverlapCircle(transform.position, visionRangeV2, playerLayerMaskV2);
        targetEnemyV2 = (hitV2 != null) ? hitV2.transform : null;
    }

    protected override void AttackEnemyV2()
    {
        if (targetEnemyV2 == null)
            return;

        V2_Unit playerUnitV2 = targetEnemyV2.GetComponent<V2_Unit>();

        if (playerUnitV2 != null && CanAttackV2())
        {
            DealDamageToTargetV2(playerUnitV2);
            
            // Reproducir animación de ataque
            if (animatorComponentV2 != null)
            {
                animatorComponentV2.SetTrigger("isAttacking");
            }

            LogDebugV2($"Attacked player: {playerUnitV2.gameObject.name}");
        }
    }

    // ==================== PATRULLA ====================

    /// <summary>
    /// Obtiene un nuevo punto de patrulla aleatorio
    /// </summary>
    private void GetNewPatrolTargetV2()
    {
        Vector3 randomOffsetV2 = new Vector3(
            Random.Range(-patrolRadiusV2, patrolRadiusV2),
            Random.Range(-patrolRadiusV2, patrolRadiusV2),
            0
        );

        patrolTargetPositionV2 = patrolStartPositionV2 + randomOffsetV2;
        LogDebugV2($"New patrol target: {patrolTargetPositionV2}");
    }

    // ==================== PRIVADO ====================

    private void InitializeEnemyWarriorV2()
    {
        // Aplicar dificultad del juego
        if (V2_GameManager.InstanceV2 != null)
        {
            GameDifficulty difficultyV2 = V2_GameManager.InstanceV2.GetGameDifficultyV2();
            
            enemyMaxHealthV2 = V2_Constants.GetEnemyHealthByDifficultyV2(difficultyV2);
            enemyAttackDamageV2 = V2_Constants.GetEnemyDamageByDifficultyV2(difficultyV2);

            LogDebugV2($"Difficulty applied - Health: {enemyMaxHealthV2}, Damage: {enemyAttackDamageV2}");
        }

        LogDebugV2("EnemyWarrior initialized");
    }

    public override void TakeDamageV2(float damageAmountV2, DamageType damageTypeV2 = DamageType.Physical)
    {
        base.TakeDamageV2(damageAmountV2, damageTypeV2);
        
        if (animatorComponentV2 != null)
        {
            animatorComponentV2.SetTrigger("takingDamage");
        }

        // Si está dañado mientras patrulla, alertarse
        if (currentAIStateV2 == AIState.Patrolling && targetEnemyV2 == null)
        {
            LogDebugV2("Enemy alerted by damage!");
        }
    }

    private void LogDebugV2(string messageV2)
    {
        if (!enableEnemyDebugV2)
            return;

        Debug.Log($"[⚔️ Enemigo {gameObject.name}] {messageV2}");
    }
}