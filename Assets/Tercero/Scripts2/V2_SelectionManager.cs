using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// V2_SelectionManager.cs - Maneja la selección de unidades del jugador
/// Dibuja caja de selección y detecta unidades dentro del rectángulo
/// </summary>
public class V2_SelectionManager : MonoBehaviour
{
    // ==================== REFERENCIAS ====================
    [SerializeField] private RectTransform selectionBoxUIV2;
    [SerializeField] private Image selectionBoxImageV2;
    [SerializeField] private Color selectionBoxColorV2 = new Color(0, 1, 0, 0.3f);
    [SerializeField] private Color selectionBoxBorderColorV2 = new Color(0, 1, 0, 1f);

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = false;

    // Estado de selección
    private Vector2 selectionStartPositionV2;
    private Vector2 selectionCurrentPositionV2;
    private Rect selectionRectV2;
    private bool isSelectingV2 = false;
    private bool isDraggingV2 = false;

    // Referencias a sistemas
    private V2_InputManager inputManagerV2;
    private V2_GameManager gameManagerV2;
    private Camera mainCameraV2;

    // ==================== PROPIEDADES ====================
    public bool IsSelectingV2 => isSelectingV2;

    // ==================== UNITY LIFECYCLE ====================
    private void AwakeInitializeV2()
    {
        // Configurar caja de selección UI
        if (selectionBoxUIV2 != null)
        {
            selectionBoxUIV2.gameObject.SetActive(false);
        }

        LogDebugV2("SelectionManager initialized");
    }

    private void StartFindReferencesV2()
    {
        inputManagerV2 = V2_InputManager.InstanceV2;
        gameManagerV2 = V2_GameManager.InstanceV2;
        mainCameraV2 = Camera.main;

        // Suscribirse a eventos de input
        if (inputManagerV2 != null)
        {
            inputManagerV2.OnLeftClickV2 += HandleLeftClickV2;
            inputManagerV2.OnDragStartV2 += HandleDragStartV2;
            inputManagerV2.OnDragEndV2 += HandleDragEndV2;
            inputManagerV2.OnCancelSelectionV2 += HandleCancelSelectionV2;
            inputManagerV2.OnRightClickV2 += HandleRightClickCommandV2;
        }

        if (gameManagerV2 == null)
            LogDebugV2("GameManager not found!", LogType.Error);
    }

    private void UpdateSelectionBoxV2()
    {
        if (!isDraggingV2)
            return;

        selectionCurrentPositionV2 = inputManagerV2.GetMouseScreenPositionV2();
        UpdateSelectionRectV2();
        UpdateSelectionBoxUIV2();
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
        UpdateSelectionBoxV2();
    }

    private void OnDestroy()
    {
        // Desuscribirse de eventos
        if (inputManagerV2 != null)
        {
            inputManagerV2.OnLeftClickV2 -= HandleLeftClickV2;
            inputManagerV2.OnDragStartV2 -= HandleDragStartV2;
            inputManagerV2.OnDragEndV2 -= HandleDragEndV2;
            inputManagerV2.OnCancelSelectionV2 -= HandleCancelSelectionV2;
            inputManagerV2.OnRightClickV2 -= HandleRightClickCommandV2;
        }
    }

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Deselecciona todas las unidades
    /// </summary>
    public void DeselectAllUnitsV2()
    {
        if (gameManagerV2 != null)
        {
            gameManagerV2.DeselectAllUnitsV2();
        }
        LogDebugV2("All units deselected");
    }

    /// <summary>
    /// Obtiene las unidades seleccionadas
    /// </summary>
    public List<V2_Unit> GetSelectedUnitsV2()
    {
        if (gameManagerV2 != null)
        {
            return gameManagerV2.GetSelectedUnitsV2();
        }
        return new List<V2_Unit>();
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Maneja inicio de click izquierdo
    /// </summary>
    private void HandleLeftClickV2(Vector3 clickPositionV2)
    {
        selectionStartPositionV2 = inputManagerV2.GetMouseScreenPositionV2();
        isSelectingV2 = true;

        // Si no se mantiene Shift, deseleccionar todo
        if (!inputManagerV2.IsMultiSelectActiveV2())
        {
            DeselectAllUnitsV2();
        }

        LogDebugV2($"Selection started at: {selectionStartPositionV2}");
    }

    /// <summary>
    /// Maneja inicio de drag
    /// </summary>
    private void HandleDragStartV2()
    {
        isDraggingV2 = true;

        if (selectionBoxUIV2 != null)
        {
            selectionBoxUIV2.gameObject.SetActive(true);
        }

        LogDebugV2("Drag started");
    }

    /// <summary>
    /// Maneja fin de drag
    /// </summary>
    private void HandleDragEndV2()
    {
        isDraggingV2 = false;
        isSelectingV2 = false;

        if (selectionBoxUIV2 != null)
        {
            selectionBoxUIV2.gameObject.SetActive(false);
        }

        // Detectar si fue un click simple o un drag
        float dragDistanceV2 = Vector2.Distance(selectionStartPositionV2, selectionCurrentPositionV2);

        if (dragDistanceV2 < V2_Constants.SELECTION_BOX_MIN_SIZE_V2)
        {
            HandleSingleClickSelectionV2();
        }
        else
        {
            HandleDragSelectionV2();
        }

        LogDebugV2("Drag ended");
    }

    /// <summary>
    /// Maneja selección de un solo click
    /// </summary>
    private void HandleSingleClickSelectionV2()
    {
        Vector3 mouseWorldPosV2 = inputManagerV2.GetMouseWorldPositionV2();
        RaycastHit2D hitV2 = Physics2D.Raycast(mouseWorldPosV2, Vector2.zero);

        if (hitV2.collider != null)
        {
            V2_Unit unitV2 = hitV2.collider.GetComponent<V2_Unit>();
            if (unitV2 != null && unitV2.GetTeamAffiliationV2() == TeamAffiliation.Player)
            {
                gameManagerV2.SelectPlayerUnitV2(unitV2, inputManagerV2.IsMultiSelectActiveV2());
                LogDebugV2($"Single click selected: {unitV2.gameObject.name}");
            }
        }
    }

    /// <summary>
    /// Maneja selección por drag
    /// </summary>
    private void HandleDragSelectionV2()
    {
        V2_Unit[] allUnitsV2 = FindObjectsByType<V2_Unit>(FindObjectsSortMode.None);

        foreach (V2_Unit unitV2 in allUnitsV2)
        {
            if (unitV2.GetTeamAffiliationV2() != TeamAffiliation.Player)
                continue;

            Vector3 unitScreenPosV2 = mainCameraV2.WorldToScreenPoint(unitV2.transform.position);

            if (selectionRectV2.Contains(unitScreenPosV2))
            {
                gameManagerV2.SelectPlayerUnitV2(unitV2, true);
            }
        }

        LogDebugV2($"Drag selection: {gameManagerV2.SelectedUnitsCountV2} units selected");
    }

    /// <summary>
    /// Maneja cancelación de selección
    /// </summary>
    private void HandleCancelSelectionV2()
    {
        DeselectAllUnitsV2();
    }

    /// <summary>
    /// Maneja comando de click derecho
    /// </summary>
    private void HandleRightClickCommandV2(Vector3 commandPositionV2)
    {
        List<V2_Unit> selectedUnitsV2 = GetSelectedUnitsV2();

        if (selectedUnitsV2.Count == 0)
        {
            LogDebugV2("No units selected for command");
            return;
        }

        // Verificar si clickeó en un recurso
        RaycastHit2D hitV2 = Physics2D.Raycast(commandPositionV2, Vector2.zero);

        if (hitV2.collider != null && hitV2.collider.CompareTag(V2_Constants.TAG_RESOURCE_V2))
        {
            // Comando de recolectar
            foreach (V2_Unit unitV2 in selectedUnitsV2)
            {
                // Intentar castear a Warrior
V2_PlayerWarrior warriorV2 = unitV2 as V2_PlayerWarrior;
if (warriorV2 != null)
{
    warriorV2.ExecuteMoveCommandV2(commandPositionV2);
    LogDebugV2($"Warrior {unitV2.gameObject.name} ordered to move");
    return;
}
            }
        }
        else
        {
            // Comando de movimiento
            foreach (V2_Unit unitV2 in selectedUnitsV2)
            {
                V2_PlayerCollector collectorV2 = unitV2 as V2_PlayerCollector;
if (collectorV2 != null)
{
    if (hitV2.collider != null && hitV2.collider.CompareTag(V2_Constants.TAG_RESOURCE_V2))
    {
        collectorV2.ExecuteCollectCommandV2(hitV2.transform);
        LogDebugV2($"Collector {unitV2.gameObject.name} ordered to collect");
    }
    else
    {
        collectorV2.ExecuteMoveCommandV2(commandPositionV2);
        LogDebugV2($"Collector {unitV2.gameObject.name} ordered to move");
    }
}
            }
        }
    }

    /// <summary>
    /// Actualiza el rectángulo de selección
    /// </summary>
    private void UpdateSelectionRectV2()
    {
        float xMinV2 = Mathf.Min(selectionStartPositionV2.x, selectionCurrentPositionV2.x);
        float xMaxV2 = Mathf.Max(selectionStartPositionV2.x, selectionCurrentPositionV2.x);
        float yMinV2 = Mathf.Min(selectionStartPositionV2.y, selectionCurrentPositionV2.y);
        float yMaxV2 = Mathf.Max(selectionStartPositionV2.y, selectionCurrentPositionV2.y);

        selectionRectV2 = new Rect(xMinV2, yMinV2, xMaxV2 - xMinV2, yMaxV2 - yMinV2);
    }

    /// <summary>
    /// Actualiza el visual de la caja de selección
    /// </summary>
    private void UpdateSelectionBoxV2()
{
    if (!isDraggingV2)
    {
        // Si NO está haciendo drag, ocultar la caja
        if (selectionBoxUIV2 != null && selectionBoxUIV2.gameObject.activeSelf)
        {
            selectionBoxUIV2.gameObject.SetActive(false);
        }
        return;
    }

    // Si está haciendo drag, actualizar la caja
    selectionCurrentPositionV2 = inputManagerV2.GetMouseScreenPositionV2();
    UpdateSelectionRectV2();
    UpdateSelectionBoxUIV2();
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
                Debug.LogWarning($"[V2_SelectionManager] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_SelectionManager] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_SelectionManager] {messageV2}");
                break;
        }
    }
}