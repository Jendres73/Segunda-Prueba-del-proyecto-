using UnityEngine;

/// <summary>
/// V2_Enums.cs - Definiciones de enumeraciones para el sistema refactorizado
/// Contiene todos los tipos y estados utilizados en el juego
/// </summary>

/// <summary>
/// Estados posibles de una unidad en el juego
/// </summary>
public enum UnitState
{
    Idle = 0,           // Unidad sin hacer nada
    Moving = 1,         // Unidad moviéndose hacia un destino
    Attacking = 2,      // Unidad atacando a un enemigo
    Collecting = 3,     // Unidad recolectando recursos
    Dead = 4            // Unidad muerta
}

/// <summary>
/// Equipos/Facciones del juego
/// </summary>
public enum TeamAffiliation
{
    Player = 0,         // Unidades del jugador
    Enemy = 1,          // Unidades enemigas
    Neutral = 2         // Unidades neutrales (recursos, etc)
}

/// <summary>
/// Tipos de órdenes que puede recibir una unidad del jugador
/// </summary>
public enum CommandType
{
    Move = 0,           // Orden de movimiento
    Attack = 1,         // Orden de ataque
    Collect = 2,        // Orden de recolectar
    Stop = 3,           // Orden de parar
    Follow = 4          // Orden de seguir unidad
}

/// <summary>
/// Tipos de eventos del juego para comunicación entre sistemas
/// </summary>
public enum GameEventType
{
    UnitSelected = 0,
    UnitDeselected = 1,
    UnitDeath = 2,
    CombatStart = 3,
    CombatEnd = 4,
    ResourceCollected = 5,
    ResourceDepleted = 6,
    GameWon = 7,
    GameLost = 8
}

/// <summary>
/// Tipos de daño en el juego
/// </summary>
public enum DamageType
{
    Physical = 0,       // Daño físico
    Fire = 1,           // Daño por fuego
    Poison = 2,         // Daño por veneno
    Magic = 3           // Daño mágico
}

/// <summary>
/// Tipos de recursos en el juego
/// </summary>
public enum ResourceType
{
    Wood = 0,           // Madera
    Stone = 1,          // Piedra
    Gold = 2,           // Oro
    Food = 3            // Comida
}

/// <summary>
/// Dificultad del juego
/// </summary>
public enum GameDifficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
    Impossible = 3
}