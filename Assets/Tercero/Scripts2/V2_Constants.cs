using UnityEngine;

/// <summary>
/// V2_Constants.cs - Constantes globales para el sistema refactorizado
/// Define valores configurables del juego en un solo lugar
/// </summary>
public static class V2_Constants
{
    // ==================== VELOCIDADES DE MOVIMIENTO ====================
    public const float PLAYER_MOVE_SPEED_V2 = 4f;
    public const float ENEMY_MOVE_SPEED_V2 = 2.5f;
    public const float PATROL_SPEED_V2 = 2f;

    // ==================== RANGOS Y DETECCIÓN ====================
    public const float VISION_RANGE_V2 = 7f;
    public const float ATTACK_RANGE_V2 = 1.5f;
    public const float DETECTION_RANGE_V2 = 8f;

    // ==================== COMBATE ====================
    public const float ATTACK_DAMAGE_PLAYER_V2 = 15f;
    public const float ATTACK_DAMAGE_ENEMY_V2 = 10f;
    public const float ATTACK_RATE_V2 = 1f;              // Ataques por segundo
    public const float ATTACK_COOLDOWN_V2 = 1f / ATTACK_RATE_V2;  // Tiempo entre ataques

    // ==================== SALUD ====================
    public const float PLAYER_MAX_HEALTH_V2 = 100f;
    public const float ENEMY_MAX_HEALTH_V2 = 50f;
    public const float HEALTH_REGENERATION_V2 = 0.5f;   // Por segundo
    public const float DAMAGE_KNOCKBACK_V2 = 0.5f;

    // ==================== RECURSOS ====================
    public const float COLLECTION_RANGE_V2 = 1.5f;
    public const float COLLECTION_SPEED_V2 = 1f;        // Recursos por segundo
    public const int MAX_RESOURCE_AMOUNT_V2 = 100;
    public const float RESOURCE_RESPAWN_TIME_V2 = 30f;  // Segundos

    // ==================== PATRULLA ====================
    public const float PATROL_RADIUS_V2 = 10f;
    public const float PATROL_WAIT_TIME_V2 = 2f;       // Segundos esperando
    public const float PATROL_WAYPOINT_DISTANCE_V2 = 0.5f;

    // ==================== ANIMACIÓN Y VISUAL ====================
    public const float ANIMATION_TRANSITION_TIME_V2 = 0.1f;
    public const float SELECTION_BOX_MIN_SIZE_V2 = 2f;  // Píxeles mínimos para drag
    public const float DEATH_ANIMATION_TIME_V2 = 1f;

    // ==================== CAPAS (LAYERS) ====================
    public const string LAYER_PLAYER_V2 = "Player";
    public const string LAYER_ENEMY_V2 = "Enemy";
    public const string LAYER_RESOURCE_V2 = "Resource";
    public const string LAYER_OBSTACLE_V2 = "Obstacle";

    // ==================== TAGS ====================
    public const string TAG_PLAYER_UNIT_V2 = "PlayerUnit";
    public const string TAG_ENEMY_UNIT_V2 = "EnemyUnit";
    public const string TAG_RESOURCE_V2 = "Resource";
    public const string TAG_BUILDING_V2 = "Building";

    // ==================== PHYSICS ====================
    public const float RIGIDBODY_DRAG_V2 = 0.1f;
    public const float RIGIDBODY_ANGULAR_DRAG_V2 = 0.05f;
    public const float COLLISION_DETECTION_DISTANCE_V2 = 0.5f;

    // ==================== TIMING ====================
    public const float FRAME_TIME_DELTA_V2 = 0.016f;    // ~60 FPS
    public const float FIXED_TIMESTEP_V2 = 0.02f;       // Physics timestep

    // ==================== DIFICULTAD ESCALABLE ====================
    public static float GetEnemyHealthByDifficultyV2(GameDifficulty difficultyV2)
    {
        return difficultyV2 switch
        {
            GameDifficulty.Easy => ENEMY_MAX_HEALTH_V2 * 0.75f,
            GameDifficulty.Normal => ENEMY_MAX_HEALTH_V2,
            GameDifficulty.Hard => ENEMY_MAX_HEALTH_V2 * 1.5f,
            GameDifficulty.Impossible => ENEMY_MAX_HEALTH_V2 * 2.5f,
            _ => ENEMY_MAX_HEALTH_V2
        };
    }

    public static float GetEnemyDamageByDifficultyV2(GameDifficulty difficultyV2)
    {
        return difficultyV2 switch
        {
            GameDifficulty.Easy => ATTACK_DAMAGE_ENEMY_V2 * 0.7f,
            GameDifficulty.Normal => ATTACK_DAMAGE_ENEMY_V2,
            GameDifficulty.Hard => ATTACK_DAMAGE_ENEMY_V2 * 1.3f,
            GameDifficulty.Impossible => ATTACK_DAMAGE_ENEMY_V2 * 2f,
            _ => ATTACK_DAMAGE_ENEMY_V2
        };
    }
}