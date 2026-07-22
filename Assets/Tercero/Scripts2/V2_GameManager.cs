using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// V2_GameManager.cs - Gestor central del juego (versión refactorizada)
/// Maneja:
/// - Inicialización del juego
/// - Gestión de unidades (jugador y enemigas)
/// - Estado global del juego
/// - Comunicación entre sistemas
/// </summary>
public class V2_GameManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    public static V2_GameManager InstanceV2 { get; private set; }

    // ==================== VARIABLES PÚBLICAS ====================
    [Header("Game Configuration")]
    [SerializeField] private GameDifficulty gameDifficultyV2 = GameDifficulty.Normal;
    [SerializeField] private float gameTimeScaleV2 = 1f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = true;

    // ==================== VARIABLES PRIVADAS ====================
    // Colecciones de unidades
    private List<V2_Unit> allUnitsV2 = new List<V2_Unit>();
    private List<V2_Unit> playerUnitsV2 = new List<V2_Unit>();
    private List<V2_Unit> enemyUnitsV2 = new List<V2_Unit>();
    private List<V2_Unit> selectedPlayerUnitsV2 = new List<V2_Unit>();

    // Estado del juego
    private bool isGameRunningV2 = false;
    private bool isGamePausedV2 = false;
    private float gameElapsedTimeV2 = 0f;

    // ==================== EVENTOS ====================
    public event System.Action OnGameStartedV2;
    public event System.Action OnGamePausedV2;
    public event System.Action OnGameResumedV2;
    public event System.Action OnGameEndedV2;
    public event System.Action<V2_Unit> OnUnitSpawnedV2;
    public event System.Action<V2_Unit> OnUnitDestroyedV2;
    public event System.Action<V2_Unit> OnUnitSelectedV2;
    public event System.Action<V2_Unit> OnUnitDeselectedV2;

    // ==================== PROPIEDADES ====================
    public bool IsGameRunningV2 => isGameRunningV2;
    public bool IsGamePausedV2 => isGamePausedV2;
    public float GameElapsedTimeV2 => gameElapsedTimeV2;
    public GameDifficulty CurrentDifficultyV2 => gameDifficultyV2;
    public int PlayerUnitsCountV2 => playerUnitsV2.Count;
    public int EnemyUnitsCountV2 => enemyUnitsV2.Count;
    public int SelectedUnitsCountV2 => selectedPlayerUnitsV2.Count;

    // ==================== UNITY LIFECYCLE ====================
    private void Awake()
    {
        AwakeInitializeV2();
    }

    private void Start()
    {
        StartInitializeGameV2();
    }

    private void Update()
    {
        UpdateGameStateV2();
    }

    private void AwakeInitializeV2()
    {
        // Singleton pattern
        if (InstanceV2 != null && InstanceV2 != this)
        {
            LogDebugV2("GameManager instance already exists. Destroying duplicate.", LogType.Warning);
            Destroy(gameObject);
            return;
        }
        InstanceV2 = this;
        DontDestroyOnLoad(gameObject);
        LogDebugV2("GameManager initialized as Singleton");
    }

    private void StartInitializeGameV2()
    {
        LogDebugV2("Starting game...");
        isGameRunningV2 = true;
        gameElapsedTimeV2 = 0f;
        OnGameStartedV2?.Invoke();
    }

    private void UpdateGameStateV2()
    {
        if (!isGameRunningV2 || isGamePausedV2)
            return;

        gameElapsedTimeV2 += Time.deltaTime * gameTimeScaleV2;

        // Verificar condiciones de victoria/derrota
        CheckGameConditionsV2();
    }

    // ==================== PUBLIC METHODS ====================
    
    /// <summary>
    /// Registra una unidad en el GameManager
    /// </summary>
    public void RegisterUnitV2(V2_Unit unitV2)
    {
        if (unitV2 == null)
            return;

        if (!allUnitsV2.Contains(unitV2))
        {
            allUnitsV2.Add(unitV2);
            LogDebugV2($"Unit registered: {unitV2.gameObject.name}");
        }

        // Registrar en lista específica de equipo
        if (unitV2.GetTeamAffiliationV2() == TeamAffiliation.Player)
        {
            if (!playerUnitsV2.Contains(unitV2))
                playerUnitsV2.Add(unitV2);
        }
        else if (unitV2.GetTeamAffiliationV2() == TeamAffiliation.Enemy)
        {
            if (!enemyUnitsV2.Contains(unitV2))
                enemyUnitsV2.Add(unitV2);
        }

        OnUnitSpawnedV2?.Invoke(unitV2);
    }

    /// <summary>
    /// Desregistra una unidad del GameManager
    /// </summary>
    public void UnregisterUnitV2(V2_Unit unitV2)
    {
        if (unitV2 == null)
            return;

        allUnitsV2.Remove(unitV2);
        playerUnitsV2.Remove(unitV2);
        enemyUnitsV2.Remove(unitV2);
        selectedPlayerUnitsV2.Remove(unitV2);

        LogDebugV2($"Unit unregistered: {unitV2.gameObject.name}");
        OnUnitDestroyedV2?.Invoke(unitV2);
    }

    /// <summary>
    /// Selecciona una unidad del jugador
    /// </summary>
    public void SelectPlayerUnitV2(V2_Unit unitV2, bool multiSelectV2 = false)
    {
        if (unitV2 == null || unitV2.GetTeamAffiliationV2() != TeamAffiliation.Player)
            return;

        if (!multiSelectV2)
            DeselectAllUnitsV2();

        if (!selectedPlayerUnitsV2.Contains(unitV2))
        {
            selectedPlayerUnitsV2.Add(unitV2);
            LogDebugV2($"Unit selected: {unitV2.gameObject.name}");
            OnUnitSelectedV2?.Invoke(unitV2);
        }
    }

    /// <summary>
    /// Deselecciona una unidad del jugador
    /// </summary>
    public void DeselectPlayerUnitV2(V2_Unit unitV2)
    {
        if (unitV2 == null)
            return;

        if (selectedPlayerUnitsV2.Remove(unitV2))
        {
            LogDebugV2($"Unit deselected: {unitV2.gameObject.name}");
            OnUnitDeselectedV2?.Invoke(unitV2);
        }
    }

    /// <summary>
    /// Deselecciona todas las unidades
    /// </summary>
    public void DeselectAllUnitsV2()
    {
        List<V2_Unit> unitsToDeselectV2 = new List<V2_Unit>(selectedPlayerUnitsV2);
        foreach (V2_Unit unitV2 in unitsToDeselectV2)
        {
            DeselectPlayerUnitV2(unitV2);
        }
    }

    /// <summary>
    /// Obtiene las unidades seleccionadas del jugador
    /// </summary>
    public List<V2_Unit> GetSelectedUnitsV2()
    {
        return new List<V2_Unit>(selectedPlayerUnitsV2);
    }

    /// <summary>
    /// Obtiene todas las unidades del juego
    /// </summary>
    public List<V2_Unit> GetAllUnitsV2()
    {
        return new List<V2_Unit>(allUnitsV2);
    }

    /// <summary>
    /// Obtiene todas las unidades del jugador
    /// </summary>
    public List<V2_Unit> GetPlayerUnitsV2()
    {
        return new List<V2_Unit>(playerUnitsV2);
    }

    /// <summary>
    /// Obtiene todas las unidades enemigas
    /// </summary>
    public List<V2_Unit> GetEnemyUnitsV2()
    {
        return new List<V2_Unit>(enemyUnitsV2);
    }

    /// <summary>
    /// Pausa el juego
    /// </summary>
    public void PauseGameV2()
    {
        if (isGameRunningV2 && !isGamePausedV2)
        {
            isGamePausedV2 = true;
            Time.timeScale = 0f;
            LogDebugV2("Game paused");
            OnGamePausedV2?.Invoke();
        }
    }

    /// <summary>
    /// Reanuda el juego
    /// </summary>
    public void ResumeGameV2()
    {
        if (isGamePausedV2)
        {
            isGamePausedV2 = false;
            Time.timeScale = gameTimeScaleV2;
            LogDebugV2("Game resumed");
            OnGameResumedV2?.Invoke();
        }
    }
    /// <summary>
    /// Establece la dificultad del juego
    /// </summary>
    public void SetGameDifficultyV2(GameDifficulty newDifficultyV2)
    {
        gameDifficultyV2 = newDifficultyV2;
        LogDebugV2($"Game difficulty set to: {gameDifficultyV2}");
    }
    /// <summary>
    /// Obtiene la dificultad actual
    /// </summary>
    public GameDifficulty GetGameDifficultyV2()
    {
        return gameDifficultyV2;
    }
    // ==================== PRIVATE METHODS ====================
    private void CheckGameConditionsV2()
    {
        // Victoria: todos los enemigos destruidos
        if (enemyUnitsV2.Count == 0 && playerUnitsV2.Count > 0)
        {
            EndGameV2(true);
        }

        // Derrota: todos los jugadores destruidos
        if (playerUnitsV2.Count == 0)
        {
            EndGameV2(false);
        }
    }
    private void EndGameV2(bool playerWonV2)
    {
        isGameRunningV2 = false;
        string resultMessageV2 = playerWonV2 ? "GAME WON!" : "GAME LOST!";
        LogDebugV2($"{resultMessageV2} Elapsed time: {gameElapsedTimeV2:F2}s");
        OnGameEndedV2?.Invoke();
    }
    private void LogDebugV2(string messageV2, LogType typeV2 = LogType.Log)
    {
        if (!enableDebugLogsV2)
            return;

        switch (typeV2)
        {
            case LogType.Warning:
                Debug.LogWarning($"[V2_GameManager] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_GameManager] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_GameManager] {messageV2}");
                break;
        }
    }
}