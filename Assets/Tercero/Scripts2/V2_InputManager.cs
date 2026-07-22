using UnityEngine;

/// <summary>
/// V2_InputManager.cs - Gestor centralizado de entrada del jugador
/// Detecta clics, teclas y envía comandos a otros sistemas
/// </summary>
public class V2_InputManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    public static V2_InputManager InstanceV2 { get; private set; }

    // ==================== VARIABLES ====================
    [Header("Input Configuration")]
    [SerializeField] private KeyCode multiSelectKeyV2 = KeyCode.LeftShift;
    [SerializeField] private KeyCode cancelSelectionKeyV2 = KeyCode.Escape;
    [SerializeField] private KeyCode pauseGameKeyV2 = KeyCode.P;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = false;

    // Referencias
    private V2_SelectionManager selectionManagerV2;
    private V2_GameManager gameManagerV2;
    private Camera mainCameraV2;

    // Estado
    private bool isInputEnabledV2 = true;

    // ==================== EVENTOS ====================
    public event System.Action<Vector3> OnRightClickV2;      // Click derecho en posición
    public event System.Action<Vector3> OnLeftClickV2;       // Click izquierdo en posición
    public event System.Action OnDragStartV2;                // Inicio de drag
    public event System.Action OnDragEndV2;                  // Fin de drag
    public event System.Action OnCancelSelectionV2;          // Cancelar selección
    public event System.Action OnPauseGameV2;                // Pausar juego

    // ==================== PROPIEDADES ====================
    public bool IsInputEnabledV2 => isInputEnabledV2;
    public bool IsMultiSelectKeyPressedV2 => Input.GetKey(multiSelectKeyV2);

    // ==================== UNITY LIFECYCLE ====================
    private void AwakeInitializeV2()
    {
        if (InstanceV2 != null && InstanceV2 != this)
        {
            LogDebugV2("InputManager instance already exists. Destroying duplicate.", LogType.Warning);
            Destroy(gameObject);
            return;
        }
        InstanceV2 = this;
        DontDestroyOnLoad(gameObject);
        LogDebugV2("InputManager initialized as Singleton");
    }

    private void StartFindReferencesV2()
    {
        mainCameraV2 = Camera.main;
        selectionManagerV2 = FindObjectOfType<V2_SelectionManager>();
        gameManagerV2 = V2_GameManager.InstanceV2;

        if (mainCameraV2 == null)
            LogDebugV2("Main camera not found!", LogType.Error);
        if (selectionManagerV2 == null)
            LogDebugV2("SelectionManager not found in scene", LogType.Warning);
        if (gameManagerV2 == null)
            LogDebugV2("GameManager not found in scene", LogType.Warning);
    }

    private void UpdateProcessInputV2()
    {
        if (!isInputEnabledV2)
            return;

        // Tecla de pausa
        if (Input.GetKeyDown(pauseGameKeyV2))
        {
            HandlePauseGameV2();
        }

        // Tecla de cancelar selección
        if (Input.GetKeyDown(cancelSelectionKeyV2))
        {
            HandleCancelSelectionV2();
        }

        // Click izquierdo - Selección
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClickV2();
        }

        // Click derecho - Comando
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClickV2();
        }
    }

    private void Awake()
    {
        AwakeInitializeV2();
    }

    private void Start()
    {
        StartFindReferencesV2();
    }

    private void Update()
    {
        UpdateProcessInputV2();
    }

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Habilita/deshabilita la entrada del usuario
    /// </summary>
    public void SetInputEnabledV2(bool enabledV2)
    {
        isInputEnabledV2 = enabledV2;
        LogDebugV2($"Input {(enabledV2 ? "enabled" : "disabled")}");
    }

    /// <summary>
    /// Obtiene la posición del ratón en el mundo
    /// </summary>
    public Vector3 GetMouseWorldPositionV2()
    {
        if (mainCameraV2 == null)
            return Vector3.zero;

        Vector3 mouseScreenPosV2 = Input.mousePosition;
        mouseScreenPosV2.z = 10f;
        Vector3 mouseWorldPosV2 = mainCameraV2.ScreenToWorldPoint(mouseScreenPosV2);
        return mouseWorldPosV2;
    }

    /// <summary>
    /// Obtiene la posición del ratón en pantalla
    /// </summary>
    public Vector3 GetMouseScreenPositionV2()
    {
        return Input.mousePosition;
    }

    /// <summary>
    /// Verifica si la tecla de multi-selección está presionada
    /// </summary>
    public bool IsMultiSelectActiveV2()
    {
        return Input.GetKey(multiSelectKeyV2);
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Maneja click izquierdo (selección)
    /// </summary>
    private void HandleLeftClickV2()
    {
        Vector3 clickPositionV2 = GetMouseWorldPositionV2();
        LogDebugV2($"Left click at: {clickPositionV2}");

        OnLeftClickV2?.Invoke(clickPositionV2);
        OnDragStartV2?.Invoke();
    }

    /// <summary>
    /// Maneja click derecho (comandos)
    /// </summary>
    private void HandleRightClickV2()
    {
        Vector3 clickPositionV2 = GetMouseWorldPositionV2();
        LogDebugV2($"Right click at: {clickPositionV2}");

        // Raycast para detectar si clickeó en un recurso
        RaycastHit2D hitV2 = Physics2D.Raycast(clickPositionV2, Vector2.zero);

        if (hitV2.collider != null)
        {
            // Clickeó en algo - verificar qué es
            if (hitV2.collider.CompareTag(V2_Constants.TAG_RESOURCE_V2))
            {
                LogDebugV2($"Clicked on resource: {hitV2.collider.gameObject.name}");
                // Las unidades seleccionadas intentarán recolectar
            }
        }

        OnRightClickV2?.Invoke(clickPositionV2);
    }

    /// <summary>
    /// Maneja cancelación de selección
    /// </summary>
    private void HandleCancelSelectionV2()
    {
        LogDebugV2("Selection cancelled");
        OnCancelSelectionV2?.Invoke();
    }

    /// <summary>
    /// Maneja pausa del juego
    /// </summary>
    private void HandlePauseGameV2()
    {
        if (gameManagerV2 == null)
            return;

        if (gameManagerV2.IsGamePausedV2)
        {
            gameManagerV2.ResumeGameV2();
        }
        else
        {
            gameManagerV2.PauseGameV2();
        }

        LogDebugV2($"Game {(gameManagerV2.IsGamePausedV2 ? "paused" : "resumed")}");
        OnPauseGameV2?.Invoke();
    }

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
                Debug.LogWarning($"[V2_InputManager] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_InputManager] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_InputManager] {messageV2}");
                break;
        }
    }
}